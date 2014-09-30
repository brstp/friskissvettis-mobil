using ImageResizer.Caching;
using ImageResizer.Configuration;
using ImageResizer.Encoding;
using ImageResizer.Plugins;
using ImageResizer.Plugins.Basic;
using ImageResizer.Util;
using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Security.Principal;
using System.Web;
using System.Web.Hosting;
using System.Web.Security;
namespace ImageResizer
{
	[ComVisible(true)]
	public class InterceptModule : IHttpModule
	{
		protected IPipelineConfig conf
		{
			get
			{
				return Config.Current.Pipeline;
			}
		}
		void IHttpModule.Init(HttpApplication context)
		{
			context.PostAuthorizeRequest -= new EventHandler(this.CheckRequest_PostAuthorizeRequest);
			context.PostAuthorizeRequest += new EventHandler(this.CheckRequest_PostAuthorizeRequest);
			context.PreSendRequestHeaders -= new EventHandler(this.context_PreSendRequestHeaders);
			context.PreSendRequestHeaders += new EventHandler(this.context_PreSendRequestHeaders);
		}
		void IHttpModule.Dispose()
		{
		}
		protected virtual void CheckRequest_PostAuthorizeRequest(object sender, EventArgs e)
		{
			HttpApplication httpApplication = sender as HttpApplication;
			if (httpApplication == null)
			{
				return;
			}
			if (httpApplication.Context == null)
			{
				return;
			}
			if (httpApplication.Context.Request == null)
			{
				return;
			}
			this.conf.FirePostAuthorizeRequest(this, httpApplication.Context);
			string preRewritePath = this.conf.PreRewritePath;
			string text = this.conf.TrimFakeExtensions(preRewritePath);
			if (this.conf.SkipFileTypeCheck || this.conf.IsAcceptedImageType(text))
			{
				NameValueCollection nameValueCollection = this.conf.ModifiedQueryString;
				UrlEventArgs urlEventArgs = new UrlEventArgs(text, nameValueCollection);
				this.conf.FireRewritingEvents(this, httpApplication.Context, urlEventArgs);
				string virtualPath = this.fixPath(urlEventArgs.VirtualPath);
				nameValueCollection = urlEventArgs.QueryString;
				this.conf.ModifiedQueryString = nameValueCollection;
				if (this.conf.HasPipelineDirective(nameValueCollection))
				{
					IPrincipal principal = httpApplication.Context.User;
					if (principal == null)
					{
						principal = new GenericPrincipal(new GenericIdentity(string.Empty, string.Empty), new string[0]);
					}
					bool flag = SecurityManager.IsGranted(new SecurityPermission(PermissionState.Unrestricted));
					bool allowAccess = true;
					if (flag)
					{
						allowAccess = UrlAuthorizationModule.CheckUrlAccessForPrincipal(virtualPath, principal, "GET");
					}
					IUrlAuthorizationEventArgs urlAuthorizationEventArgs = new UrlAuthorizationEventArgs(virtualPath, new NameValueCollection(nameValueCollection), allowAccess);
					this.conf.FireAuthorizeImage(this, httpApplication.Context, urlAuthorizationEventArgs);
					if (!urlAuthorizationEventArgs.AllowAccess)
					{
						throw new ImageProcessingException(403, "Access denied", "Access denied");
					}
					bool flag2 = this.conf.VppUsage != VppUsageOption.Always && File.Exists(HostingEnvironment.MapPath(virtualPath));
					bool flag3 = this.conf.VppUsage != VppUsageOption.Never && !flag2 && this.conf.FileExists(virtualPath, nameValueCollection);
					object vf = flag3 ? this.conf.GetFile(virtualPath, nameValueCollection) : null;
					if (!flag2)
					{
						if (!flag3)
						{
							goto IL_228;
						}
					}
					try
					{
						this.HandleRequest(httpApplication.Context, virtualPath, nameValueCollection, vf);
						return;
					}
					catch (FileNotFoundException innerException)
					{
						this.FileMissing(httpApplication.Context, virtualPath, nameValueCollection);
						throw new ImageMissingException("The specified resource could not be located", "File not found", innerException);
					}
					IL_228:
					this.FileMissing(httpApplication.Context, virtualPath, nameValueCollection);
				}
			}
		}
		protected void FileMissing(HttpContext httpContext, string virtualPath, NameValueCollection q)
		{
			this.conf.FireImageMissing(this, httpContext, new UrlEventArgs(virtualPath, new NameValueCollection(q)));
			httpContext.Items[this.conf.ResponseArgsKey] = null;
		}
		protected string fixPath(string virtualPath)
		{
			if (virtualPath.StartsWith("~"))
			{
				return HostingEnvironment.ApplicationVirtualPath.TrimEnd(new char[]
				{
					'/'
				}) + "/" + virtualPath.TrimStart(new char[]
				{
					'/'
				});
			}
			if (!virtualPath.StartsWith("/"))
			{
				return HostingEnvironment.ApplicationVirtualPath.TrimEnd(new char[]
				{
					'/'
				}) + "/" + virtualPath;
			}
			return virtualPath;
		}
		protected virtual void HandleRequest(HttpContext context, string virtualPath, NameValueCollection queryString, object vf)
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			ResizeSettings settings = new ResizeSettings(queryString);
			bool flag = settings.Cache == ServerCacheMode.Always;
			bool isProcessing = settings.Process == ProcessWhen.Always;
			if (settings.Process == ProcessWhen.Default)
			{
				NameValueCollection nameValueCollection = new NameValueCollection(queryString);
				nameValueCollection.Remove("cache");
				nameValueCollection.Remove("process");
				nameValueCollection.Remove("useresizingpipeline");
				isProcessing = (this.conf.IsAcceptedImageType(virtualPath) && this.conf.HasPipelineDirective(nameValueCollection));
			}
			if (settings.Cache == ServerCacheMode.Default && isProcessing)
			{
				flag = true;
			}
			if (!flag)
			{
				settings.Cache = ServerCacheMode.No;
			}
			if (!isProcessing && !flag)
			{
				return;
			}
			context.Items[this.conf.ResponseArgsKey] = "";
			bool hasModifiedDate = vf == null || vf is IVirtualFileWithModifiedDate;
			DateTime modDate = DateTime.MinValue;
			if (hasModifiedDate && vf != null)
			{
				modDate = ((IVirtualFileWithModifiedDate)vf).ModifiedDateUTC;
				if (modDate == DateTime.MinValue || modDate == DateTime.MaxValue)
				{
					hasModifiedDate = false;
				}
			}
			IEncoder encoder = null;
			if (isProcessing)
			{
				encoder = this.conf.GetImageBuilder().EncoderProvider.GetEncoder(settings, virtualPath);
				if (encoder == null)
				{
					throw new ImageProcessingException("Image Resizer: No image encoder was found for the request.");
				}
			}
			string text = PathUtils.GetFullExtension(virtualPath).TrimStart(new char[]
			{
				'.'
			});
			if (!this.conf.IsAcceptedImageType(virtualPath))
			{
				text = "unknown";
			}
			string text2 = "application/octet-stream";
			ImageFormat imageFormatFromExtension = DefaultEncoder.GetImageFormatFromExtension(text);
			if (imageFormatFromExtension != null)
			{
				text2 = DefaultEncoder.GetContentTypeFromImageFormat(imageFormatFromExtension);
			}
			ResponseArgs responseArgs = new ResponseArgs();
			responseArgs.RequestKey = virtualPath + PathUtils.BuildQueryString(queryString);
			responseArgs.RewrittenQuerystring = settings;
			responseArgs.ResponseHeaders.ContentType = (isProcessing ? encoder.MimeType : text2);
			responseArgs.SuggestedExtension = (isProcessing ? encoder.Extension : text);
			responseArgs.HasModifiedDate = hasModifiedDate;
			responseArgs.GetModifiedDateUTC = delegate
			{
				if (vf == null)
				{
					return File.GetLastWriteTimeUtc(HostingEnvironment.MapPath(virtualPath));
				}
				if (hasModifiedDate)
				{
					return modDate;
				}
				return DateTime.MinValue;
			};
			responseArgs.ResizeImageToStream = delegate(Stream stream)
			{
				try
				{
					if (!isProcessing)
					{
						using (Stream stream2 = (vf != null) ? ((vf is IVirtualFile) ? ((IVirtualFile)vf).Open() : ((VirtualFile)vf).Open()) : File.Open(HostingEnvironment.MapPath(virtualPath), FileMode.Open, FileAccess.Read, FileShare.Read))
						{
							Utils.copyStream(stream2, stream);
							goto IL_D6;
						}
					}
					if (vf != null)
					{
						this.conf.GetImageBuilder().Build(vf, stream, settings);
					}
					else
					{
						this.conf.GetImageBuilder().Build(HostingEnvironment.MapPath(virtualPath), stream, settings);
					}
					IL_D6:;
				}
				catch (FileNotFoundException innerException)
				{
					this.FileMissing(context, virtualPath, queryString);
					throw new ImageMissingException("The specified resource could not be located", "File not found", innerException);
				}
			};
			context.Items[this.conf.ResponseArgsKey] = responseArgs;
			this.conf.FirePreHandleImage(this, context, responseArgs);
			ICache cachingSystem = this.conf.GetCacheProvider().GetCachingSystem(context, responseArgs);
			if (cachingSystem == null)
			{
				throw new ImageProcessingException("Image Resizer: No caching plugin was found for the request");
			}
			cachingSystem.Process(context, responseArgs);
			stopwatch.Stop();
			context.Items["ResizingTime"] = stopwatch.ElapsedMilliseconds;
		}
		protected void context_PreSendRequestHeaders(object sender, EventArgs e)
		{
			HttpApplication httpApplication = sender as HttpApplication;
			if (httpApplication == null)
			{
				return;
			}
			if (httpApplication.Context == null)
			{
				return;
			}
			if (httpApplication.Context.Items == null)
			{
				return;
			}
			if (httpApplication.Context.Request == null)
			{
				return;
			}
			HttpContext context = httpApplication.Context;
			if (context.Items[this.conf.ResponseArgsKey] == null)
			{
				return;
			}
			IResponseArgs responseArgs = context.Items[this.conf.ResponseArgsKey] as IResponseArgs;
			if (responseArgs == null)
			{
				return;
			}
			if (responseArgs.ResponseHeaders.ApplyDuringPreSendRequestHeaders)
			{
				responseArgs.ResponseHeaders.ApplyToResponse(responseArgs.ResponseHeaders, httpApplication.Context);
			}
		}
	}
}
