using log4net.Core;
using log4net.Util;
using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Threading;
namespace log4net.Appender
{
	public class RollingFileAppender : FileAppender
	{
		public enum RollingMode
		{
			Once,
			Size,
			Date,
			Composite
		}
		protected enum RollPoint
		{
			InvalidRollPoint = -1,
			TopOfMinute,
			TopOfHour,
			HalfDay,
			TopOfDay,
			TopOfWeek,
			TopOfMonth
		}
		public interface IDateTime
		{
			DateTime Now
			{
				get;
			}
		}
		private class DefaultDateTime : RollingFileAppender.IDateTime
		{
			public DateTime Now
			{
				get
				{
					return DateTime.Now;
				}
			}
		}
		private RollingFileAppender.IDateTime m_dateTime = null;
		private string m_datePattern = ".yyyy-MM-dd";
		private string m_scheduledFilename = null;
		private DateTime m_nextCheck = DateTime.MaxValue;
		private DateTime m_now;
		private RollingFileAppender.RollPoint m_rollPoint;
		private long m_maxFileSize = 10485760L;
		private int m_maxSizeRollBackups = 0;
		private int m_curSizeRollBackups = 0;
		private int m_countDirection = -1;
		private RollingFileAppender.RollingMode m_rollingStyle = RollingFileAppender.RollingMode.Composite;
		private bool m_rollDate = true;
		private bool m_rollSize = true;
		private bool m_staticLogFileName = true;
		private string m_baseFileName;
		private static readonly DateTime s_date1970 = new DateTime(1970, 1, 1);
		public string DatePattern
		{
			get
			{
				return this.m_datePattern;
			}
			set
			{
				this.m_datePattern = value;
			}
		}
		public int MaxSizeRollBackups
		{
			get
			{
				return this.m_maxSizeRollBackups;
			}
			set
			{
				this.m_maxSizeRollBackups = value;
			}
		}
		public long MaxFileSize
		{
			get
			{
				return this.m_maxFileSize;
			}
			set
			{
				this.m_maxFileSize = value;
			}
		}
		public string MaximumFileSize
		{
			get
			{
				return this.m_maxFileSize.ToString(NumberFormatInfo.InvariantInfo);
			}
			set
			{
				this.m_maxFileSize = OptionConverter.ToFileSize(value, this.m_maxFileSize + 1L);
			}
		}
		public int CountDirection
		{
			get
			{
				return this.m_countDirection;
			}
			set
			{
				this.m_countDirection = value;
			}
		}
		public RollingFileAppender.RollingMode RollingStyle
		{
			get
			{
				return this.m_rollingStyle;
			}
			set
			{
				this.m_rollingStyle = value;
				switch (this.m_rollingStyle)
				{
				case RollingFileAppender.RollingMode.Once:
					this.m_rollDate = false;
					this.m_rollSize = false;
					base.AppendToFile = false;
					break;
				case RollingFileAppender.RollingMode.Size:
					this.m_rollDate = false;
					this.m_rollSize = true;
					break;
				case RollingFileAppender.RollingMode.Date:
					this.m_rollDate = true;
					this.m_rollSize = false;
					break;
				case RollingFileAppender.RollingMode.Composite:
					this.m_rollDate = true;
					this.m_rollSize = true;
					break;
				}
			}
		}
		public bool StaticLogFileName
		{
			get
			{
				return this.m_staticLogFileName;
			}
			set
			{
				this.m_staticLogFileName = value;
			}
		}
		public RollingFileAppender()
		{
			this.m_dateTime = new RollingFileAppender.DefaultDateTime();
		}
		protected override void SetQWForFiles(TextWriter writer)
		{
			base.QuietWriter = new CountingQuietTextWriter(writer, this.ErrorHandler);
		}
		protected override void Append(LoggingEvent loggingEvent)
		{
			this.AdjustFileBeforeAppend();
			base.Append(loggingEvent);
		}
		protected override void Append(LoggingEvent[] loggingEvents)
		{
			this.AdjustFileBeforeAppend();
			base.Append(loggingEvents);
		}
		protected virtual void AdjustFileBeforeAppend()
		{
			if (this.m_rollDate)
			{
				DateTime now = this.m_dateTime.Now;
				if (now >= this.m_nextCheck)
				{
					this.m_now = now;
					this.m_nextCheck = this.NextCheckDate(this.m_now, this.m_rollPoint);
					this.RollOverTime(true);
				}
			}
			if (this.m_rollSize)
			{
				if (this.File != null && ((CountingQuietTextWriter)base.QuietWriter).Count >= this.m_maxFileSize)
				{
					this.RollOverSize();
				}
			}
		}
		protected override void OpenFile(string fileName, bool append)
		{
			Monitor.Enter(this);
			try
			{
				fileName = this.GetNextOutputFileName(fileName);
				long count = 0L;
				if (append)
				{
					using (base.SecurityContext.Impersonate(this))
					{
						if (System.IO.File.Exists(fileName))
						{
							count = new FileInfo(fileName).Length;
						}
					}
				}
				else
				{
					if (LogLog.IsErrorEnabled)
					{
						if (this.m_maxSizeRollBackups != 0 && this.FileExists(fileName))
						{
							LogLog.Error("RollingFileAppender: INTERNAL ERROR. Append is False but OutputFile [" + fileName + "] already exists.");
						}
					}
				}
				if (!this.m_staticLogFileName)
				{
					this.m_scheduledFilename = fileName;
				}
				base.OpenFile(fileName, append);
				((CountingQuietTextWriter)base.QuietWriter).Count = count;
			}
			finally
			{
				Monitor.Exit(this);
			}
		}
		protected string GetNextOutputFileName(string fileName)
		{
			if (!this.m_staticLogFileName)
			{
				fileName = fileName.Trim();
				if (this.m_rollDate)
				{
					fileName += this.m_now.ToString(this.m_datePattern, DateTimeFormatInfo.InvariantInfo);
				}
				if (this.m_countDirection >= 0)
				{
					fileName = fileName + '.' + this.m_curSizeRollBackups;
				}
			}
			return fileName;
		}
		private void DetermineCurSizeRollBackups()
		{
			this.m_curSizeRollBackups = 0;
			string text = null;
			string baseFile = null;
			using (base.SecurityContext.Impersonate(this))
			{
				text = Path.GetFullPath(this.m_baseFileName);
				baseFile = Path.GetFileName(text);
			}
			ArrayList existingFiles = this.GetExistingFiles(text);
			this.InitializeRollBackups(baseFile, existingFiles);
			LogLog.Debug("RollingFileAppender: curSizeRollBackups starts at [" + this.m_curSizeRollBackups + "]");
		}
		private static string GetWildcardPatternForFile(string baseFileName)
		{
			return baseFileName + '*';
		}
		private ArrayList GetExistingFiles(string baseFilePath)
		{
			ArrayList arrayList = new ArrayList();
			string text = null;
			using (base.SecurityContext.Impersonate(this))
			{
				string fullPath = Path.GetFullPath(baseFilePath);
				text = Path.GetDirectoryName(fullPath);
				if (Directory.Exists(text))
				{
					string fileName = Path.GetFileName(fullPath);
					string[] files = Directory.GetFiles(text, RollingFileAppender.GetWildcardPatternForFile(fileName));
					if (files != null)
					{
						for (int i = 0; i < files.Length; i++)
						{
							string fileName2 = Path.GetFileName(files[i]);
							if (fileName2.StartsWith(fileName))
							{
								arrayList.Add(fileName2);
							}
						}
					}
				}
			}
			LogLog.Debug("RollingFileAppender: Searched for existing files in [" + text + "]");
			return arrayList;
		}
		private void RollOverIfDateBoundaryCrossing()
		{
			if (this.m_staticLogFileName && this.m_rollDate)
			{
				if (this.FileExists(this.m_baseFileName))
				{
					DateTime lastWriteTime;
					using (base.SecurityContext.Impersonate(this))
					{
						lastWriteTime = System.IO.File.GetLastWriteTime(this.m_baseFileName);
					}
					LogLog.Debug(string.Concat(new string[]
					{
						"RollingFileAppender: [",
						lastWriteTime.ToString(this.m_datePattern, DateTimeFormatInfo.InvariantInfo),
						"] vs. [",
						this.m_now.ToString(this.m_datePattern, DateTimeFormatInfo.InvariantInfo),
						"]"
					}));
					if (!lastWriteTime.ToString(this.m_datePattern, DateTimeFormatInfo.InvariantInfo).Equals(this.m_now.ToString(this.m_datePattern, DateTimeFormatInfo.InvariantInfo)))
					{
						this.m_scheduledFilename = this.m_baseFileName + lastWriteTime.ToString(this.m_datePattern, DateTimeFormatInfo.InvariantInfo);
						LogLog.Debug("RollingFileAppender: Initial roll over to [" + this.m_scheduledFilename + "]");
						this.RollOverTime(false);
						LogLog.Debug("RollingFileAppender: curSizeRollBackups after rollOver at [" + this.m_curSizeRollBackups + "]");
					}
				}
			}
		}
		protected void ExistingInit()
		{
			this.DetermineCurSizeRollBackups();
			this.RollOverIfDateBoundaryCrossing();
			if (!base.AppendToFile)
			{
				bool flag = false;
				string nextOutputFileName = this.GetNextOutputFileName(this.m_baseFileName);
				using (base.SecurityContext.Impersonate(this))
				{
					flag = System.IO.File.Exists(nextOutputFileName);
				}
				if (flag)
				{
					if (this.m_maxSizeRollBackups == 0)
					{
						LogLog.Debug("RollingFileAppender: Output file [" + nextOutputFileName + "] already exists. MaxSizeRollBackups is 0; cannot roll. Overwriting existing file.");
					}
					else
					{
						LogLog.Debug("RollingFileAppender: Output file [" + nextOutputFileName + "] already exists. Not appending to file. Rolling existing file out of the way.");
						this.RollOverRenameFiles(nextOutputFileName);
					}
				}
			}
		}
		private void InitializeFromOneFile(string baseFile, string curFileName)
		{
			if (curFileName.StartsWith(baseFile))
			{
				if (!curFileName.Equals(baseFile))
				{
					int num = curFileName.LastIndexOf(".");
					if (-1 != num)
					{
						if (this.m_staticLogFileName)
						{
							int num2 = curFileName.Length - num;
							if (baseFile.Length + num2 != curFileName.Length)
							{
								return;
							}
						}
						if (this.m_rollDate && !this.m_staticLogFileName)
						{
							if (!curFileName.StartsWith(baseFile + this.m_dateTime.Now.ToString(this.m_datePattern, DateTimeFormatInfo.InvariantInfo)))
							{
								LogLog.Debug("RollingFileAppender: Ignoring file [" + curFileName + "] because it is from a different date period");
								return;
							}
						}
						try
						{
							int num3;
							if (SystemInfo.TryParse(curFileName.Substring(num + 1), out num3))
							{
								if (num3 > this.m_curSizeRollBackups)
								{
									if (0 != this.m_maxSizeRollBackups)
									{
										if (-1 == this.m_maxSizeRollBackups)
										{
											this.m_curSizeRollBackups = num3;
										}
										else
										{
											if (this.m_countDirection >= 0)
											{
												this.m_curSizeRollBackups = num3;
											}
											else
											{
												if (num3 <= this.m_maxSizeRollBackups)
												{
													this.m_curSizeRollBackups = num3;
												}
											}
										}
									}
									LogLog.Debug(string.Concat(new object[]
									{
										"RollingFileAppender: File name [",
										curFileName,
										"] moves current count to [",
										this.m_curSizeRollBackups,
										"]"
									}));
								}
							}
						}
						catch (FormatException)
						{
							LogLog.Debug("RollingFileAppender: Encountered a backup file not ending in .x [" + curFileName + "]");
						}
					}
				}
			}
		}
		private void InitializeRollBackups(string baseFile, ArrayList arrayFiles)
		{
			if (null != arrayFiles)
			{
				string baseFile2 = baseFile.ToLower(CultureInfo.InvariantCulture);
				foreach (string text in arrayFiles)
				{
					this.InitializeFromOneFile(baseFile2, text.ToLower(CultureInfo.InvariantCulture));
				}
			}
		}
		private RollingFileAppender.RollPoint ComputeCheckPeriod(string datePattern)
		{
			string text = RollingFileAppender.s_date1970.ToString(datePattern, DateTimeFormatInfo.InvariantInfo);
			RollingFileAppender.RollPoint result;
			for (int i = 0; i <= 5; i++)
			{
				string text2 = this.NextCheckDate(RollingFileAppender.s_date1970, (RollingFileAppender.RollPoint)i).ToString(datePattern, DateTimeFormatInfo.InvariantInfo);
				LogLog.Debug(string.Concat(new object[]
				{
					"RollingFileAppender: Type = [",
					i,
					"], r0 = [",
					text,
					"], r1 = [",
					text2,
					"]"
				}));
				if (text != null && text2 != null && !text.Equals(text2))
				{
					result = (RollingFileAppender.RollPoint)i;
					return result;
				}
			}
			result = RollingFileAppender.RollPoint.InvalidRollPoint;
			return result;
		}
		public override void ActivateOptions()
		{
			if (this.m_rollDate && this.m_datePattern != null)
			{
				this.m_now = this.m_dateTime.Now;
				this.m_rollPoint = this.ComputeCheckPeriod(this.m_datePattern);
				if (this.m_rollPoint == RollingFileAppender.RollPoint.InvalidRollPoint)
				{
					throw new ArgumentException("Invalid RollPoint, unable to parse [" + this.m_datePattern + "]");
				}
				this.m_nextCheck = this.NextCheckDate(this.m_now, this.m_rollPoint);
			}
			else
			{
				if (this.m_rollDate)
				{
					this.ErrorHandler.Error("Either DatePattern or rollingStyle options are not set for [" + base.Name + "].");
				}
			}
			if (base.SecurityContext == null)
			{
				base.SecurityContext = SecurityContextProvider.DefaultProvider.CreateSecurityContext(this);
			}
			using (base.SecurityContext.Impersonate(this))
			{
				base.File = FileAppender.ConvertToFullPath(base.File.Trim());
				this.m_baseFileName = base.File;
			}
			if (this.m_rollDate && this.File != null && this.m_scheduledFilename == null)
			{
				this.m_scheduledFilename = this.File + this.m_now.ToString(this.m_datePattern, DateTimeFormatInfo.InvariantInfo);
			}
			this.ExistingInit();
			base.ActivateOptions();
		}
		protected void RollOverTime(bool fileIsOpen)
		{
			if (this.m_staticLogFileName)
			{
				if (this.m_datePattern == null)
				{
					this.ErrorHandler.Error("Missing DatePattern option in rollOver().");
					return;
				}
				string text = this.m_now.ToString(this.m_datePattern, DateTimeFormatInfo.InvariantInfo);
				if (this.m_scheduledFilename.Equals(this.File + text))
				{
					this.ErrorHandler.Error(string.Concat(new string[]
					{
						"Compare ",
						this.m_scheduledFilename,
						" : ",
						this.File,
						text
					}));
					return;
				}
				if (fileIsOpen)
				{
					base.CloseFile();
				}
				for (int i = 1; i <= this.m_curSizeRollBackups; i++)
				{
					string fromFile = this.File + '.' + i;
					string toFile = this.m_scheduledFilename + '.' + i;
					this.RollFile(fromFile, toFile);
				}
				this.RollFile(this.File, this.m_scheduledFilename);
			}
			this.m_curSizeRollBackups = 0;
			this.m_scheduledFilename = this.File + this.m_now.ToString(this.m_datePattern, DateTimeFormatInfo.InvariantInfo);
			if (fileIsOpen)
			{
				this.SafeOpenFile(this.m_baseFileName, false);
			}
		}
		protected void RollFile(string fromFile, string toFile)
		{
			if (this.FileExists(fromFile))
			{
				this.DeleteFile(toFile);
				try
				{
					LogLog.Debug(string.Concat(new string[]
					{
						"RollingFileAppender: Moving [",
						fromFile,
						"] -> [",
						toFile,
						"]"
					}));
					using (base.SecurityContext.Impersonate(this))
					{
						System.IO.File.Move(fromFile, toFile);
					}
				}
				catch (Exception e)
				{
					this.ErrorHandler.Error(string.Concat(new string[]
					{
						"Exception while rolling file [",
						fromFile,
						"] -> [",
						toFile,
						"]"
					}), e, ErrorCode.GenericFailure);
				}
			}
			else
			{
				LogLog.Warn(string.Concat(new string[]
				{
					"RollingFileAppender: Cannot RollFile [",
					fromFile,
					"] -> [",
					toFile,
					"]. Source does not exist"
				}));
			}
		}
		protected bool FileExists(string path)
		{
			bool result;
			using (base.SecurityContext.Impersonate(this))
			{
				result = System.IO.File.Exists(path);
			}
			return result;
		}
		protected void DeleteFile(string fileName)
		{
			if (this.FileExists(fileName))
			{
				string text = fileName;
				string text2 = string.Concat(new object[]
				{
					fileName,
					".",
					Environment.TickCount,
					".DeletePending"
				});
				try
				{
					using (base.SecurityContext.Impersonate(this))
					{
						System.IO.File.Move(fileName, text2);
					}
					text = text2;
				}
				catch (Exception exception)
				{
					LogLog.Debug(string.Concat(new string[]
					{
						"RollingFileAppender: Exception while moving file to be deleted [",
						fileName,
						"] -> [",
						text2,
						"]"
					}), exception);
				}
				try
				{
					using (base.SecurityContext.Impersonate(this))
					{
						System.IO.File.Delete(text);
					}
					LogLog.Debug("RollingFileAppender: Deleted file [" + fileName + "]");
				}
				catch (Exception ex)
				{
					if (text == fileName)
					{
						this.ErrorHandler.Error("Exception while deleting file [" + text + "]", ex, ErrorCode.GenericFailure);
					}
					else
					{
						LogLog.Debug("RollingFileAppender: Exception while deleting temp file [" + text + "]", ex);
					}
				}
			}
		}
		protected void RollOverSize()
		{
			base.CloseFile();
			LogLog.Debug("RollingFileAppender: rolling over count [" + ((CountingQuietTextWriter)base.QuietWriter).Count + "]");
			LogLog.Debug("RollingFileAppender: maxSizeRollBackups [" + this.m_maxSizeRollBackups + "]");
			LogLog.Debug("RollingFileAppender: curSizeRollBackups [" + this.m_curSizeRollBackups + "]");
			LogLog.Debug("RollingFileAppender: countDirection [" + this.m_countDirection + "]");
			this.RollOverRenameFiles(this.File);
			if (!this.m_staticLogFileName && this.m_countDirection >= 0)
			{
				this.m_curSizeRollBackups++;
			}
			this.SafeOpenFile(this.m_baseFileName, false);
		}
		protected void RollOverRenameFiles(string baseFileName)
		{
			if (this.m_maxSizeRollBackups != 0)
			{
				if (this.m_countDirection < 0)
				{
					if (this.m_curSizeRollBackups == this.m_maxSizeRollBackups)
					{
						this.DeleteFile(baseFileName + '.' + this.m_maxSizeRollBackups);
						this.m_curSizeRollBackups--;
					}
					for (int i = this.m_curSizeRollBackups; i >= 1; i--)
					{
						this.RollFile(baseFileName + "." + i, baseFileName + '.' + (i + 1));
					}
					this.m_curSizeRollBackups++;
					this.RollFile(baseFileName, baseFileName + ".1");
				}
				else
				{
					if (this.m_curSizeRollBackups >= this.m_maxSizeRollBackups && this.m_maxSizeRollBackups > 0)
					{
						int num = this.m_curSizeRollBackups - this.m_maxSizeRollBackups;
						if (this.m_staticLogFileName)
						{
							num++;
						}
						string text = baseFileName;
						if (!this.m_staticLogFileName)
						{
							int num2 = text.LastIndexOf(".");
							if (num2 >= 0)
							{
								text = text.Substring(0, num2);
							}
						}
						this.DeleteFile(text + '.' + num);
					}
					if (this.m_staticLogFileName)
					{
						this.m_curSizeRollBackups++;
						this.RollFile(baseFileName, baseFileName + '.' + this.m_curSizeRollBackups);
					}
				}
			}
		}
		protected DateTime NextCheckDate(DateTime currentDateTime, RollingFileAppender.RollPoint rollPoint)
		{
			DateTime result = currentDateTime;
			switch (rollPoint)
			{
			case RollingFileAppender.RollPoint.TopOfMinute:
				result = result.AddMilliseconds((double)(-(double)result.Millisecond));
				result = result.AddSeconds((double)(-(double)result.Second));
				result = result.AddMinutes(1.0);
				break;
			case RollingFileAppender.RollPoint.TopOfHour:
				result = result.AddMilliseconds((double)(-(double)result.Millisecond));
				result = result.AddSeconds((double)(-(double)result.Second));
				result = result.AddMinutes((double)(-(double)result.Minute));
				result = result.AddHours(1.0);
				break;
			case RollingFileAppender.RollPoint.HalfDay:
				result = result.AddMilliseconds((double)(-(double)result.Millisecond));
				result = result.AddSeconds((double)(-(double)result.Second));
				result = result.AddMinutes((double)(-(double)result.Minute));
				if (result.Hour < 12)
				{
					result = result.AddHours((double)(12 - result.Hour));
				}
				else
				{
					result = result.AddHours((double)(-(double)result.Hour));
					result = result.AddDays(1.0);
				}
				break;
			case RollingFileAppender.RollPoint.TopOfDay:
				result = result.AddMilliseconds((double)(-(double)result.Millisecond));
				result = result.AddSeconds((double)(-(double)result.Second));
				result = result.AddMinutes((double)(-(double)result.Minute));
				result = result.AddHours((double)(-(double)result.Hour));
				result = result.AddDays(1.0);
				break;
			case RollingFileAppender.RollPoint.TopOfWeek:
				result = result.AddMilliseconds((double)(-(double)result.Millisecond));
				result = result.AddSeconds((double)(-(double)result.Second));
				result = result.AddMinutes((double)(-(double)result.Minute));
				result = result.AddHours((double)(-(double)result.Hour));
				result = result.AddDays((double)((DayOfWeek)7 - result.DayOfWeek));
				break;
			case RollingFileAppender.RollPoint.TopOfMonth:
				result = result.AddMilliseconds((double)(-(double)result.Millisecond));
				result = result.AddSeconds((double)(-(double)result.Second));
				result = result.AddMinutes((double)(-(double)result.Minute));
				result = result.AddHours((double)(-(double)result.Hour));
				result = result.AddDays((double)(1 - result.Day));
				result = result.AddMonths(1);
				break;
			}
			return result;
		}
	}
}
