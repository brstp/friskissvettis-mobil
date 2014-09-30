using MoMA.Helpers;
using MoMA.Helpers.Url;
using MoMA.Mobile.Barcode;
using MoMA.Mobile.Cache;
using MoMA.Mobile.Configuration;
using MoMA.Mobile.Device;
using MoMA.Mobile.Html;
using MoMA.Mobile.Properties;
using MoMA.Mobile.UI.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
namespace MoMA.Mobile.Pages
{
	public class MobilePage : Page
	{
		private DeviceInfo device = new DeviceInfo();
		private HtmlDocumentWrapper doc;
		private static int RequestCount;
		public bool HideBrowserAddressbar
		{
			get;
			set;
		}
		public bool AutoAddMobileMetaTags
		{
			get;
			set;
		}
		public bool UseCache
		{
			get;
			set;
		}
		internal string Html5PositionScript
		{
			get;
			set;
		}
		internal string Javascript
		{
			get;
			set;
		}
		internal static CachedFileCollection CachedFiles
		{
			get;
			set;
		}
		internal static string CachePath
		{
			get
			{
				return HttpContext.Current.Server.MapPath(PathConfiguration.GetSection().ExternalImageCache);
			}
		}
		private int FilesUploaded
		{
			get
			{
				DirectoryInfo directoryInfo = new DirectoryInfo(MobilePage.CachePath);
				return directoryInfo.GetFiles().Count<FileInfo>();
			}
		}
		private long CacheSize
		{
			get
			{
				DirectoryInfo directoryInfo = new DirectoryInfo(MobilePage.CachePath);
				List<FileInfo> list = directoryInfo.GetFiles().ToList<FileInfo>();
				long num = 0L;
				foreach (FileInfo current in list)
				{
					num += current.Length;
				}
				return num;
			}
		}
		public MobilePage()
		{
			this.HideBrowserAddressbar = true;
			this.AutoAddMobileMetaTags = true;
			this.UseCache = false;
		}
		protected override void OnLoad(EventArgs e)
		{
			if (ContextHelper.GetValue<string>("mode", "html").Equals("html"))
			{
				base.OnLoad(e);
			}
		}
		public static void Log(string text)
		{
		}
		protected override void Render(HtmlTextWriter writer)
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			string value = ContextHelper.GetValue<string>("mode", "html");
			int displayWidth = DeviceInfo.CurrentDevice.DisplayWidth;
			string text = this.AsHtml();
			string originalContent = Regex.Replace(text, "action=\"[^\"]+\"", "action=\"\"");
			if (value.Equals("html"))
			{
				MobilePage.Log(string.Concat(new object[]
				{
					Environment.NewLine,
					Environment.NewLine,
					"hashHtml: ",
					stopwatch.Elapsed
				}));
				stopwatch.Restart();
			}
			string text2 = "";
			CachedOutput cachedOutput = CachedOutput.Get(originalContent, displayWidth, value);
			if (value.Equals("html"))
			{
				MobilePage.Log("cacheItem: " + stopwatch.Elapsed);
				stopwatch.Restart();
			}
			if (value.Equals("html"))
			{
				MobilePage.Log("license: " + stopwatch.Elapsed);
				stopwatch.Restart();
			}
			this.SetupMobileControls();
			if (value.Equals("html"))
			{
				MobilePage.Log("SetupMobileControls: " + stopwatch.Elapsed);
				stopwatch.Restart();
			}
			this.doc = new HtmlDocumentWrapper(text, this);
			base.Response.Clear();
			if (value.Equals("html"))
			{
				MobilePage.Log("HtmlDocumentWrapper: " + stopwatch.Elapsed);
				stopwatch.Restart();
			}
			string key;
			switch (key = value)
			{
			case "html":
				base.Response.ContentType = "text/html";
				if (cachedOutput != null)
				{
					text2 = cachedOutput.Content;
				}
				else
				{
					this.doc.SetupHtml();
					if (value.Equals("html"))
					{
						MobilePage.Log("doc.SetupHtml(): " + stopwatch.Elapsed);
						stopwatch.Restart();
					}
					text2 += this.doc.ToString();
					if (this.UseCache)
					{
						CachedOutput.Set(originalContent, displayWidth, value, text2);
					}
					CachedOutput.Set(originalContent, displayWidth, "css", this.doc.GeneratedCSS);
					if (ContextHelper.GetValue<bool>("debug", false))
					{
						text2 = "<!-- Output from SDK -->\n" + text2;
						text2 = "<!-- Phone: " + DeviceInfo.CurrentDevice.DeviceModel + " -->\n" + text2;
						text2 = "<!-- Width: " + DeviceInfo.CurrentDevice.DisplayWidth.ToString() + " -->\n" + text2;
						text2 = "<!-- Height: " + DeviceInfo.CurrentDevice.DisplayHeight.ToString() + " -->\n" + text2;
					}
				}
				break;
			case "js":
				base.Response.ContentType = "text/js";
				break;
			case "css":
				base.Response.ContentType = "text/css";
				if (cachedOutput != null)
				{
					text2 = cachedOutput.Content;
				}
				else
				{
					text2 += this.doc.SetupCSS().Trim();
					if (this.UseCache)
					{
						CachedOutput.Set(originalContent, displayWidth, value, text2);
					}
				}
				break;
			case "controlcss":
				base.Response.ContentType = "text/css";
				if (cachedOutput != null)
				{
					text2 = cachedOutput.Content;
				}
				else
				{
					text2 += this.GetControlCSS();
					if (this.UseCache)
					{
						CachedOutput.Set(originalContent, displayWidth, value, text2);
					}
				}
				break;
			case "resource":
			{
				string value2 = ContextHelper.GetValue<string>("file", "");
				string value3 = ContextHelper.GetValue<string>("type", "image/png");
				this.OutputResource(value2, value3);
				break;
			}
			case "barcode":
			{
				string value4 = ContextHelper.GetValue<string>("barcodeType", "image/png");
				string value5 = ContextHelper.GetValue<string>("code", "");
				this.OutputBarcode(value4, value5);
				break;
			}
			case "scale":
			{
				string value6 = ContextHelper.GetValue<string>("url", "");
				this.ImageScale(value6);
				break;
			}
			case "imageCreator":
			{
				int value7 = ContextHelper.GetValue<int>("width", 1);
				int value8 = ContextHelper.GetValue<int>("height", 1);
				string value9 = ContextHelper.GetValue<string>("htmlColor", "#000000");
				this.ImageCreator(value7, value8, value9);
				break;
			}
			}
			writer.Write(text2);
		}
		private void ImageCreator(int width, int height, string htmlColor)
		{
			HttpContext.Current.Response.ContentType = "image/jpeg";
			if (!htmlColor.Contains("#"))
			{
				htmlColor = "#" + htmlColor;
			}
			this.DrawRectangle(HttpContext.Current, width, height, htmlColor);
		}
		public void DrawGradient(HttpContext context, int Width, int Height, string HtmlStartColor, string HtmlEndColor)
		{
		}
		public void DrawRectangle(HttpContext context, int Width, int Height, string HtmlColor)
		{
			if (Height == 0)
			{
				Height = 1;
			}
			RectangleF rect = new RectangleF(0f, 0f, (float)Width, (float)Height);
			Bitmap bitmap = new Bitmap(Width, Height, PixelFormat.Format24bppRgb);
			Graphics graphics = Graphics.FromImage(bitmap);
			Color color = Color.Black;
			try
			{
				color = ColorTranslator.FromHtml(HtmlColor);
			}
			catch
			{
			}
			graphics.FillRectangle(new SolidBrush(color), rect);
			bitmap.Save(context.Response.OutputStream, ImageFormat.Jpeg);
			graphics.Dispose();
			bitmap.Dispose();
		}
		private void ImageScale(string url)
		{
			if (MobilePage.CachedFiles == null)
			{
				MobilePage.CachedFiles = new CachedFileCollection();
				MobilePage.CachedFiles.Load();
			}
			MobilePage.RequestCount++;
			url.Contains("ashx");
			if (!string.IsNullOrEmpty(url))
			{
				string text = "";
				int value = ContextHelper.GetValue<int>("width", new DeviceInfo().DisplayWidth);
				Image image = this.GetCachedFile(url, out text);
				string value2 = ContextHelper.GetValue<string>("crop", "");
				if (image == null)
				{
					return;
				}
				HttpContext.Current.Response.ContentType = FileHelper.MimeType(url);
				HttpContext.Current.Response.Clear();
				HttpContext.Current.Response.BufferOutput = true;
				ImageFormat imageFormat = FileHelper.GetImageFormat(url);
				if (!string.IsNullOrEmpty(value2))
				{
					Rectangle rect = default(Rectangle);
					List<string> list = value2.Split(new char[]
					{
						','
					}).ToList<string>();
					if (list.Count == 4)
					{
						int num = int.Parse(list[0]);
						int num2 = int.Parse(list[2]);
						int num3 = int.Parse(list[1]);
						int num4 = int.Parse(list[3]);
						rect = new Rectangle(num, num3, num2 - num, num4 - num3);
						Bitmap bitmap = new Bitmap(image).Clone(rect, image.PixelFormat);
						image.Dispose();
						image = bitmap;
					}
				}
				Image image2 = ImageHelper.ResizeImage(image, value);
				if (imageFormat == ImageFormat.Png)
				{
					MemoryStream memoryStream = new MemoryStream();
					image2.Save(memoryStream, ImageFormat.Png);
					memoryStream.WriteTo(HttpContext.Current.Response.OutputStream);
					memoryStream.Dispose();
				}
				else
				{
					image2.Save(HttpContext.Current.Response.OutputStream, imageFormat);
				}
				image2.Dispose();
				image.Dispose();
			}
		}
		public Image GetCachedFile(string url, out string path)
		{
			path = "";
			if (!Directory.Exists(MobilePage.CachePath))
			{
				return null;
			}
			string hash = EncryptHelper.GetMd5Sum(url);
			string text = string.Concat(new string[]
			{
				MobilePage.CachePath,
				"\\",
				hash,
				".",
				ExtendedUrl.GetExtension(url)
			});
			if (File.Exists(text))
			{
				CachedFile cachedFile = (
					from f in MobilePage.CachedFiles
					where f.Hash.Equals(hash)
					select f).FirstOrDefault<CachedFile>();
				if (cachedFile != null)
				{
					cachedFile.LastRequested = DateTime.Now;
				}
				MobilePage.CachedFiles.Save();
			}
			else
			{
				if (DownloadHelper.Download(url, text))
				{
					FileInfo fileInfo = new FileInfo(text);
					CachedFile cachedFile2 = new CachedFile();
					cachedFile2.Url = url;
					cachedFile2.Hash = EncryptHelper.GetMd5Sum(cachedFile2.Url);
					cachedFile2.Added = DateTime.Now;
					cachedFile2.LastRequested = DateTime.Now;
					cachedFile2.Size = fileInfo.Length;
					cachedFile2.Path = text;
					MobilePage.CachedFiles.Add(cachedFile2);
					MobilePage.CachedFiles.Save();
				}
			}
			path = text;
			if (File.Exists(text))
			{
				return Image.FromFile(text);
			}
			return null;
		}
		private void OutputBarcode(string barcodeType, string rawCode)
		{
			HttpContext.Current.Response.ContentType = "image/jpeg";
			if (string.IsNullOrEmpty(rawCode))
			{
				string value = ContextHelper.GetValue<string>("template", "AXXX-NNAA");
				Code code = new Code(value);
				rawCode = code.ToString();
			}
			QR qR = new QR();
			qR.Initialize(rawCode);
			Image image = qR.GetImage();
			image.Save(HttpContext.Current.Response.OutputStream, ImageFormat.Jpeg);
			image.Dispose();
		}
		private bool OutputResource(string resource, string contentType)
		{
			string assemblyFile = Assembly.GetExecutingAssembly().CodeBase.Replace("file:///", "");
			HttpContext.Current.Response.Clear();
			HttpContext.Current.Response.ContentType = contentType;
			Assembly assembly = Assembly.LoadFrom(assemblyFile);
			if (assembly != null)
			{
				using (Stream manifestResourceStream = assembly.GetManifestResourceStream("MoMA.Mobile.Resources." + resource.Replace("/", ".")))
				{
					if (manifestResourceStream != null)
					{
						int value = ContextHelper.GetValue<int>("width", 0);
						if (contentType != null)
						{
							if (!(contentType == "image/png"))
							{
								if (!(contentType == "image/jpeg"))
								{
									if (!(contentType == "text/js"))
									{
										goto IL_1E4;
									}
									goto IL_1AD;
								}
							}
							else
							{
								Image image2;
								Image image = image2 = Image.FromStream(manifestResourceStream);
								try
								{
									using (MemoryStream memoryStream = new MemoryStream())
									{
										if (value > 0)
										{
											Image image3 = ImageHelper.ResizeImage(image, value);
											image3.Save(memoryStream, ImageFormat.Png);
											memoryStream.WriteTo(base.Response.OutputStream);
											image3.Dispose();
										}
										else
										{
											image.Save(memoryStream, ImageFormat.Png);
											memoryStream.WriteTo(base.Response.OutputStream);
										}
									}
									goto IL_1E4;
								}
								finally
								{
									if (image2 != null)
									{
										((IDisposable)image2).Dispose();
									}
								}
							}
							using (new StreamReader(manifestResourceStream))
							{
								Image image4;
								Image image = image4 = Image.FromStream(manifestResourceStream);
								try
								{
									image.Save(HttpContext.Current.Response.OutputStream, ImageFormat.Jpeg);
								}
								finally
								{
									if (image4 != null)
									{
										((IDisposable)image4).Dispose();
									}
								}
								goto IL_1E4;
							}
							IL_1AD:
							using (StreamReader streamReader2 = new StreamReader(manifestResourceStream))
							{
								string value2 = streamReader2.ReadToEnd();
								base.Response.Output.Write(value2);
							}
						}
					}
					IL_1E4:;
				}
			}
			return false;
		}
		internal string AsHtml()
		{
			StringBuilder stringBuilder = new StringBuilder();
			HtmlTextWriter writer = new HtmlTextWriter(new StringWriter(stringBuilder));
			base.Render(writer);
			return stringBuilder.ToString();
		}
		private void SetupMobileControls()
		{
			ExtendedUrl extendedUrl = new ExtendedUrl(base.Request.Url.ToString());
			extendedUrl.Querystring.Update("mode", "controlcss");
			string content = "<link href=\"" + extendedUrl.ToString() + "\" rel=\"stylesheet\" type=\"text/css\" momasdk=\"true\" />";
			PageHelper.AddHeadControl(this, 0, content);
			this.RegisterControlJs();
			if (!string.IsNullOrEmpty(this.Html5PositionScript))
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add("scripts", this.Html5PositionScript);
				this.Javascript += JavascriptHelper.CreateScript(Resources.html5_positioning, dictionary, false);
			}
			if (!string.IsNullOrWhiteSpace(this.Javascript))
			{
				Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
				dictionary2.Add("javascript", this.Javascript);
				this.Javascript = JavascriptHelper.CreateScript(Resources.onload, dictionary2, false);
				base.ClientScript.RegisterClientScriptBlock(base.GetType(), "moma_js", this.Javascript, true);
			}
		}
		private void RegisterControlJs()
		{
			List<MobileControl> allMobileControls = this.GetAllMobileControls();
			foreach (MobileControl current in allMobileControls)
			{
				current.RegisterJsIncludes();
				current.RegisterJsScripts();
			}
		}
		private string GetControlCSS()
		{
			StringBuilder stringBuilder = new StringBuilder();
			List<MobileControl> allMobileControls = this.GetAllMobileControls();
			foreach (MobileControl current in allMobileControls)
			{
				stringBuilder.Append(current.BuildCSS(null));
			}
			return stringBuilder.ToString();
		}
		private List<MobileControl> GetAllMobileControls()
		{
			List<MobileControl> result = new List<MobileControl>();
			this.GetAllMobileControls(this.Controls, ref result);
			return result;
		}
		private void GetAllMobileControls(ControlCollection controls, ref List<MobileControl> mobileControls)
		{
			foreach (Control control in controls)
			{
				if (control.Visible)
				{
					MobileControl mobileControl = control as MobileControl;
					if (mobileControl != null)
					{
						mobileControls.Add(mobileControl);
					}
					if (control.HasControls())
					{
						this.GetAllMobileControls(control.Controls, ref mobileControls);
					}
				}
			}
		}
	}
}
