using log4net.Core;
using log4net.Util;
using System;
using System.Collections;
using System.Data;
using System.Globalization;
using System.IO;
namespace log4net.Appender
{
	public class AdoNetAppender : BufferingAppenderSkeleton
	{
		protected bool m_usePreparedCommand;
		protected ArrayList m_parameters;
		private SecurityContext m_securityContext;
		private IDbConnection m_dbConnection;
		private IDbCommand m_dbCommand;
		private string m_connectionString;
		private string m_connectionType;
		private string m_commandText;
		private CommandType m_commandType;
		private bool m_useTransactions;
		private bool m_reconnectOnError;
		public string ConnectionString
		{
			get
			{
				return this.m_connectionString;
			}
			set
			{
				this.m_connectionString = value;
			}
		}
		public string ConnectionType
		{
			get
			{
				return this.m_connectionType;
			}
			set
			{
				this.m_connectionType = value;
			}
		}
		public string CommandText
		{
			get
			{
				return this.m_commandText;
			}
			set
			{
				this.m_commandText = value;
			}
		}
		public CommandType CommandType
		{
			get
			{
				return this.m_commandType;
			}
			set
			{
				this.m_commandType = value;
			}
		}
		public bool UseTransactions
		{
			get
			{
				return this.m_useTransactions;
			}
			set
			{
				this.m_useTransactions = value;
			}
		}
		public SecurityContext SecurityContext
		{
			get
			{
				return this.m_securityContext;
			}
			set
			{
				this.m_securityContext = value;
			}
		}
		public bool ReconnectOnError
		{
			get
			{
				return this.m_reconnectOnError;
			}
			set
			{
				this.m_reconnectOnError = value;
			}
		}
		protected IDbConnection Connection
		{
			get
			{
				return this.m_dbConnection;
			}
			set
			{
				this.m_dbConnection = value;
			}
		}
		public AdoNetAppender()
		{
			this.m_connectionType = "System.Data.OleDb.OleDbConnection, System.Data, Version=1.0.3300.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
			this.m_useTransactions = true;
			this.m_commandType = CommandType.Text;
			this.m_parameters = new ArrayList();
			this.m_reconnectOnError = false;
		}
		public override void ActivateOptions()
		{
			base.ActivateOptions();
			this.m_usePreparedCommand = (this.m_commandText != null && this.m_commandText.Length > 0);
			if (this.m_securityContext == null)
			{
				this.m_securityContext = SecurityContextProvider.DefaultProvider.CreateSecurityContext(this);
			}
			this.InitializeDatabaseConnection();
			this.InitializeDatabaseCommand();
		}
		protected override void OnClose()
		{
			base.OnClose();
			if (this.m_dbCommand != null)
			{
				try
				{
					this.m_dbCommand.Dispose();
				}
				catch (Exception exception)
				{
					LogLog.Warn("AdoNetAppender: Exception while disposing cached command object", exception);
				}
				this.m_dbCommand = null;
			}
			if (this.m_dbConnection != null)
			{
				try
				{
					this.m_dbConnection.Close();
				}
				catch (Exception exception)
				{
					LogLog.Warn("AdoNetAppender: Exception while disposing cached connection object", exception);
				}
				this.m_dbConnection = null;
			}
		}
		protected override void SendBuffer(LoggingEvent[] events)
		{
			if (this.m_reconnectOnError && (this.m_dbConnection == null || this.m_dbConnection.State != ConnectionState.Open))
			{
				LogLog.Debug("AdoNetAppender: Attempting to reconnect to database. Current Connection State: " + ((this.m_dbConnection == null) ? "<null>" : this.m_dbConnection.State.ToString()));
				this.InitializeDatabaseConnection();
				this.InitializeDatabaseCommand();
			}
			if (this.m_dbConnection != null && this.m_dbConnection.State == ConnectionState.Open)
			{
				if (this.m_useTransactions)
				{
					IDbTransaction dbTransaction = null;
					try
					{
						dbTransaction = this.m_dbConnection.BeginTransaction();
						this.SendBuffer(dbTransaction, events);
						dbTransaction.Commit();
					}
					catch (Exception e)
					{
						if (dbTransaction != null)
						{
							try
							{
								dbTransaction.Rollback();
							}
							catch (Exception)
							{
							}
						}
						this.ErrorHandler.Error("Exception while writing to database", e);
					}
				}
				else
				{
					this.SendBuffer(null, events);
				}
			}
		}
		public void AddParameter(AdoNetAppenderParameter parameter)
		{
			this.m_parameters.Add(parameter);
		}
		protected virtual void SendBuffer(IDbTransaction dbTran, LoggingEvent[] events)
		{
			if (this.m_usePreparedCommand)
			{
				if (this.m_dbCommand != null)
				{
					if (dbTran != null)
					{
						this.m_dbCommand.Transaction = dbTran;
					}
					LoggingEvent[] array = events;
					for (int i = 0; i < array.Length; i++)
					{
						LoggingEvent loggingEvent = array[i];
						foreach (AdoNetAppenderParameter adoNetAppenderParameter in this.m_parameters)
						{
							adoNetAppenderParameter.FormatValue(this.m_dbCommand, loggingEvent);
						}
						this.m_dbCommand.ExecuteNonQuery();
					}
				}
			}
			else
			{
				using (IDbCommand dbCommand = this.m_dbConnection.CreateCommand())
				{
					if (dbTran != null)
					{
						dbCommand.Transaction = dbTran;
					}
					LoggingEvent[] array = events;
					for (int i = 0; i < array.Length; i++)
					{
						LoggingEvent loggingEvent = array[i];
						string logStatement = this.GetLogStatement(loggingEvent);
						LogLog.Debug("AdoNetAppender: LogStatement [" + logStatement + "]");
						dbCommand.CommandText = logStatement;
						dbCommand.ExecuteNonQuery();
					}
				}
			}
		}
		protected virtual string GetLogStatement(LoggingEvent logEvent)
		{
			string result;
			if (this.Layout == null)
			{
				this.ErrorHandler.Error("ADOAppender: No Layout specified.");
				result = "";
			}
			else
			{
				StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);
				this.Layout.Format(stringWriter, logEvent);
				result = stringWriter.ToString();
			}
			return result;
		}
		private void InitializeDatabaseConnection()
		{
			try
			{
				if (this.m_dbCommand != null)
				{
					try
					{
						this.m_dbCommand.Dispose();
					}
					catch (Exception exception)
					{
						LogLog.Warn("AdoNetAppender: Exception while disposing cached command object", exception);
					}
					this.m_dbCommand = null;
				}
				if (this.m_dbConnection != null)
				{
					try
					{
						this.m_dbConnection.Close();
					}
					catch (Exception exception)
					{
						LogLog.Warn("AdoNetAppender: Exception while disposing cached connection object", exception);
					}
					this.m_dbConnection = null;
				}
				this.m_dbConnection = (IDbConnection)Activator.CreateInstance(this.ResolveConnectionType());
				this.m_dbConnection.ConnectionString = this.m_connectionString;
				using (this.SecurityContext.Impersonate(this))
				{
					this.m_dbConnection.Open();
				}
			}
			catch (Exception e)
			{
				this.ErrorHandler.Error("Could not open database connection [" + this.m_connectionString + "]", e);
				this.m_dbConnection = null;
			}
		}
		protected virtual Type ResolveConnectionType()
		{
			Type typeFromString;
			try
			{
				typeFromString = SystemInfo.GetTypeFromString(this.m_connectionType, true, false);
			}
			catch (Exception e)
			{
				this.ErrorHandler.Error("Failed to load connection type [" + this.m_connectionType + "]", e);
				throw;
			}
			return typeFromString;
		}
		private void InitializeDatabaseCommand()
		{
			if (this.m_dbConnection != null && this.m_usePreparedCommand)
			{
				try
				{
					if (this.m_dbCommand != null)
					{
						try
						{
							this.m_dbCommand.Dispose();
						}
						catch (Exception exception)
						{
							LogLog.Warn("AdoNetAppender: Exception while disposing cached command object", exception);
						}
						this.m_dbCommand = null;
					}
					this.m_dbCommand = this.m_dbConnection.CreateCommand();
					this.m_dbCommand.CommandText = this.m_commandText;
					this.m_dbCommand.CommandType = this.m_commandType;
				}
				catch (Exception e)
				{
					this.ErrorHandler.Error("Could not create database command [" + this.m_commandText + "]", e);
					if (this.m_dbCommand != null)
					{
						try
						{
							this.m_dbCommand.Dispose();
						}
						catch
						{
						}
						this.m_dbCommand = null;
					}
				}
				if (this.m_dbCommand != null)
				{
					try
					{
						foreach (AdoNetAppenderParameter adoNetAppenderParameter in this.m_parameters)
						{
							try
							{
								adoNetAppenderParameter.Prepare(this.m_dbCommand);
							}
							catch (Exception e)
							{
								this.ErrorHandler.Error("Could not add database command parameter [" + adoNetAppenderParameter.ParameterName + "]", e);
								throw;
							}
						}
					}
					catch
					{
						try
						{
							this.m_dbCommand.Dispose();
						}
						catch
						{
						}
						this.m_dbCommand = null;
					}
				}
				if (this.m_dbCommand != null)
				{
					try
					{
						this.m_dbCommand.Prepare();
					}
					catch (Exception e)
					{
						this.ErrorHandler.Error("Could not prepare database command [" + this.m_commandText + "]", e);
						try
						{
							this.m_dbCommand.Dispose();
						}
						catch
						{
						}
						this.m_dbCommand = null;
					}
				}
			}
		}
	}
}
