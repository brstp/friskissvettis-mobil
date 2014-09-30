using HtmlAgilityPack;
using MoMA.Helpers;
using MoMA.Helpers.Url;
using MoMA.Mobile.Configuration;
using MoMA.Mobile.CSS;
using MoMA.Mobile.CSS.Properties;
using MoMA.Mobile.Device;
using MoMA.Mobile.Extensions;
using MoMA.Mobile.Pages;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Yahoo.Yui.Compressor;
namespace MoMA.Mobile.Html
{
	internal class HtmlDocumentWrapper
	{
		private IDeviceInfo device = DeviceInfo.CurrentDevice;
		private int idSeed = 1001;
		private ExtendedHtmlDocument HtmlDocument
		{
			get;
			set;
		}
		internal CSSStylesheetCollection CSSStylesheets
		{
			get;
			set;
		}
		internal MobilePage Page
		{
			get;
			set;
		}
		public HtmlNodeDimensionCollection CachedNodeDimensions
		{
			get;
			set;
		}
		public string GeneratedCSS
		{
			get;
			set;
		}
		public HtmlDocumentWrapper(string html, MobilePage page)
		{
			this.Page = page;
			this.CachedNodeDimensions = new HtmlNodeDimensionCollection();
			this.HtmlDocument = new ExtendedHtmlDocument(this);
			this.HtmlDocument.LoadHtml(html);
			this.SetNodeIdentifiers();
		}
		public void RemoveNode(HtmlNode removeNode)
		{
			removeNode.Attributes.Add("removed", "true");
		}
		public void ReinitXPath()
		{
			this.HtmlDocument.LoadHtml(this.HtmlDocument.DocumentNode.OuterHtml);
		}
		public void MergeAllJS()
		{
		}
		public void MinimizeAllJS()
		{
			HtmlNodeCollection nodesByCssSelector = this.GetNodesByCssSelector("script");
			if (nodesByCssSelector != null)
			{
				foreach (HtmlNode current in (IEnumerable<HtmlNode>)nodesByCssSelector)
				{
					if (!string.IsNullOrWhiteSpace(current.InnerText))
					{
						current.InnerHtml = JavaScriptCompressor.Compress(current.InnerText, false, false, false, false, 100);
					}
				}
			}
		}
		public string SetupJS()
		{
			this.MergeAllJS();
			this.MinimizeAllJS();
			return "";
		}
		public string SetupCSS()
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			string value = ContextHelper.GetValue<string>("mode", "html");
			this.AddInlineCSSToStyleTag();
			if (value.Equals("html"))
			{
				MobilePage.Log("    AddInlineCSSToStyleTag: " + stopwatch.Elapsed);
				stopwatch.Restart();
			}
			this.ReinitCSSStylesheets();
			if (value.Equals("html"))
			{
				MobilePage.Log("    ReinitCSSStylesheets: " + stopwatch.Elapsed);
				stopwatch.Restart();
			}
			if (this.CSSStylesheets.Count == 0)
			{
				HtmlNode nodeByCssSelector = this.GetNodeByCssSelector("head");
				HtmlNode node = HtmlNode.CreateNode("<style type=\"text/css\"></style>");
				if (nodeByCssSelector != null)
				{
					nodeByCssSelector.ChildNodes.Add(node);
					this.ReinitCSSStylesheets();
				}
			}
			if (value.Equals("html"))
			{
				MobilePage.Log("    ReinitCSSStylesheets: " + stopwatch.Elapsed);
				stopwatch.Restart();
			}
			if (value.Equals("html"))
			{
				MobilePage.Log("    MergeCSSPropertyParts: " + stopwatch.Elapsed);
				stopwatch.Restart();
			}
			this.FixFormProperties();
			if (value.Equals("html"))
			{
				MobilePage.Log("    FixFormProperties: " + stopwatch.Elapsed);
				stopwatch.Restart();
			}
			if (value.Equals("html"))
			{
				MobilePage.Log("    SetHundredPercentWidthDefaultInCSS: " + stopwatch.Elapsed);
				stopwatch.Restart();
			}
			if (value.Equals("html"))
			{
				MobilePage.Log("    CacheCalculations: " + stopwatch.Elapsed);
				stopwatch.Restart();
			}
			this.ReplaceExternalImageToLocalInCSS();
			if (value.Equals("html"))
			{
				MobilePage.Log("    ReplaceExternalImageToLocalInCSS: " + stopwatch.Elapsed);
				stopwatch.Restart();
			}
			this.GeneratedCSS = this.UpdateCSS();
			if (value.Equals("html"))
			{
				MobilePage.Log("    UpdateCSS: " + stopwatch.Elapsed);
				stopwatch.Restart();
			}
			return this.GeneratedCSS;
		}
		private void AddInlineCSSToStyleTag()
		{
			StringBuilder stringBuilder = new StringBuilder();
			HtmlNodeCollection allNodesInBody = this.GetAllNodesInBody();
			if (allNodesInBody != null)
			{
				foreach (HtmlNode current in (IEnumerable<HtmlNode>)allNodesInBody)
				{
					if (current.HasAttributes && current.Attributes["style"] != null)
					{
						string text = current.Attributes["style"].Value.Trim();
						if (!text.EndsWith(";"))
						{
							text += ";";
						}
						stringBuilder.Append(string.Concat(new string[]
						{
							"#",
							current.Id,
							"{",
							text,
							"}"
						}));
						current.Attributes["style"].Remove();
					}
				}
			}
			string text2 = stringBuilder.ToString();
			if (!string.IsNullOrEmpty(text2))
			{
				HtmlNode nodeByCssSelector = this.GetNodeByCssSelector("head");
				HtmlNode node = HtmlNode.CreateNode("<style type=\"text/css\">" + text2 + "</style>");
				if (nodeByCssSelector != null)
				{
					nodeByCssSelector.ChildNodes.Add(node);
				}
			}
		}
		public string ReplaceExternalImageUrlToLocal(string url)
		{
			if (ExtendedUrl.IsLocalhost(url))
			{
				return url;
			}
			ExtendedUrl extendedUrl = new ExtendedUrl(HttpContext.Current.Request.Url.ToString());
			extendedUrl.Querystring.Update("mode", "scale");
			extendedUrl.Querystring.Update("url", HttpContext.Current.Server.UrlEncode(url));
			return extendedUrl.ToString();
		}
		private void ReplaceExternalImageToLocalInCSS()
		{
			List<string> list = new List<string>
			{
				"background",
				"background-image"
			};
			foreach (CSSStylesheet current in this.CSSStylesheets)
			{
				foreach (CSSSection current2 in current)
				{
					foreach (CSSProperty current3 in current2.Properties)
					{
						if (list.Contains(current3.Name.ToLower().Trim()) && current3.ContainUrl())
						{
							string text = CssHelper.ExtractUrl(current3.Value);
							if (ExtendedUrl.IsExternal(text))
							{
								current3.Value = current3.Value.Replace(text, this.ReplaceExternalImageUrlToLocal(text));
							}
						}
					}
				}
			}
		}
		private void SetHundredPercentWidthDefaultInCSS()
		{
			HtmlNodeCollection allNodesInBody = this.GetAllNodesInBody();
			if (allNodesInBody != null)
			{
				foreach (HtmlNode current in (IEnumerable<HtmlNode>)allNodesInBody)
				{
					if (!current.IsCheckbox() && !current.IsRadio() && !current.IsLink() && !current.IsViewstate() && !current.HasAspNetHiddenClass() && !current.IsCenter() && this.GetCSSProperty(current, "width", null).Value == null)
					{
						CSSStylesheet cSSStylesheet = this.CSSStylesheets.First<CSSStylesheet>();
						CSSSection cSSSection = new CSSSection(cSSStylesheet);
						cSSSection.Selector = "#" + current.Id;
						cSSSection.AddProperty("width", "100CW%");
						cSSStylesheet.Add(cSSSection);
					}
				}
			}
		}
		private void FixFormProperties()
		{
			HtmlNodeCollection nodesByCssSelector = this.GetNodesByCssSelector("input[type=text]");
			if (nodesByCssSelector != null)
			{
				foreach (HtmlNode current in (IEnumerable<HtmlNode>)nodesByCssSelector)
				{
					if (this.GetCSSProperty(current, "border-width", null).Value == null)
					{
						CSSStylesheet cSSStylesheet = this.CSSStylesheets.First<CSSStylesheet>();
						CSSSection cSSSection = new CSSSection(cSSStylesheet);
						cSSSection.Selector = "#" + current.Id;
						cSSSection.AddProperty("border-width", "2px");
						cSSStylesheet.Add(cSSSection);
					}
				}
			}
		}
		private void MergeCSSPropertyParts()
		{
			foreach (CSSStylesheet current in this.CSSStylesheets)
			{
				foreach (CSSSection current2 in current)
				{
					Dictionary<string, CSSProperty> dictionary = new Dictionary<string, CSSProperty>();
					Dictionary<string, int> dictionary2 = new Dictionary<string, int>();
					Dictionary<string, CSSSimpleMeasure> dictionary3 = new Dictionary<string, CSSSimpleMeasure>();
					List<CSSProperty> list = new List<CSSProperty>();
					List<CSSProperty> list2 = new List<CSSProperty>();
					foreach (CSSProperty current3 in current2.Properties)
					{
						string text = current3.IsPropertyPart();
						bool flag = !string.IsNullOrEmpty(text);
						if (flag)
						{
							string text2 = current3.Name.Replace("-" + text, "").Trim();
							if (!text2.EqualsIgnoreCase("border"))
							{
								int num;
								CSSSimpleMeasure cSSSimpleMeasure;
								if (dictionary.ContainsKey(text2))
								{
									CSSProperty cSSProperty = dictionary[text2];
									num = dictionary2[text2];
									cSSSimpleMeasure = dictionary3[text2];
								}
								else
								{
									CSSProperty cSSProperty = current2.GetProperty(text2);
									cSSProperty = ((cSSProperty != null) ? cSSProperty : new CSSProperty(current2)
									{
										Name = text2
									});
									dictionary.Add(text2, cSSProperty);
									num = current2.GetPropertyIndex(text2);
									dictionary2.Add(text2, num);
									cSSSimpleMeasure = new CSSSimpleMeasure(cSSProperty.Value);
									dictionary3.Add(text2, cSSSimpleMeasure);
								}
								int propertyIndex = current2.GetPropertyIndex(current3.Name);
								string a;
								if (propertyIndex > num && (a = text) != null)
								{
									if (!(a == "top"))
									{
										if (!(a == "right"))
										{
											if (!(a == "bottom"))
											{
												if (a == "left")
												{
													cSSSimpleMeasure.Left = current3.Value;
												}
											}
											else
											{
												cSSSimpleMeasure.Bottom = current3.Value;
											}
										}
										else
										{
											cSSSimpleMeasure.Right = current3.Value;
										}
									}
									else
									{
										cSSSimpleMeasure.Top = current3.Value;
									}
								}
								list.Add(current3);
							}
						}
					}
					foreach (KeyValuePair<string, CSSProperty> current4 in dictionary)
					{
						string key = current4.Key;
						string value = dictionary3[key].ToString();
						if (!string.IsNullOrEmpty(value))
						{
							current2.UpdateProperty(key, value);
						}
					}
					foreach (CSSProperty current5 in list)
					{
						current2.Properties.Remove(current5);
					}
					foreach (CSSProperty current6 in list2)
					{
						current2.Properties.Add(current6);
					}
				}
			}
		}
		private string UpdateCSS()
		{
			string text = "";
			List<ICSSPropertyHelper> list = new List<ICSSPropertyHelper>
			{
				new CSSBackgroundCrop(),
				new CSSBackgroundHeight(),
				new CSSBackgroundLinearGradient(),
				new CSSBackgroundWidth(),
				new CSSBorderRadius(),
				new CSSBoxShadow(),
				new CSSBoxSizing(),
				new CSSTextShadow()
			};
			foreach (CSSStylesheet current in this.CSSStylesheets)
			{
				foreach (CSSSection current2 in current)
				{
					Dictionary<int, List<CSSProperty>> dictionary = new Dictionary<int, List<CSSProperty>>();
					foreach (CSSProperty current3 in current2.Properties)
					{
						foreach (ICSSPropertyHelper current4 in list)
						{
							if (current3.Name.Equals(current4.PropertyName))
							{
								List<CSSProperty> allProperties = current4.GetAllProperties(current3, null);
								if (allProperties.Count > 0)
								{
									CSSPropertyCollection properties = current2.Properties;
									int num = properties.IndexOf(current3);
									if (num >= 0)
									{
										dictionary.Add(num, allProperties);
									}
								}
							}
						}
					}
					foreach (KeyValuePair<int, List<CSSProperty>> current5 in dictionary)
					{
						current2.Properties.RemoveAt(current5.Key);
						current2.Properties.InsertRange(current5.Key, current5.Value);
					}
				}
				Regex regex = new Regex("[-]{0,1}\\d+(\\.(\\d)+){0,1}(DW%|DH%)", RegexOptions.Singleline);
				string text2 = regex.Replace(current.ToString(), delegate(Match match)
				{
					string text3 = match.ToString();
					bool flag = text3.Contains("W");
					string s = text3.Replace("DW%", "").Replace("DH%", "").Replace(".", ",");
					float num2 = 0f;
					float.TryParse(s, out num2);
					int displayWidth;
					if (flag)
					{
						displayWidth = DeviceInfo.CurrentDevice.DisplayWidth;
					}
					else
					{
						displayWidth = DeviceInfo.CurrentDevice.DisplayWidth;
					}
					return Math.Floor((double)num2 / 100.0 * (double)displayWidth).ToString() + "px";
				});
				text2 = CSSMeasure.Calculate(text2);
				text += text2;
				HtmlNode stylesheetHtmlNode = current.StylesheetHtmlNode;
				this.RemoveNode(stylesheetHtmlNode);
			}
			text = Environment.NewLine + "  " + text.Trim();
			if (MinimizeConfiguration.GetSection().CSS)
			{
				text = Environment.NewLine + CssCompressor.Compress(text, 400, CssCompressionType.MichaelAshRegexEnhancements, true).Trim() + Environment.NewLine;
			}
			if (!ExternalConfiguration.GetSection().CSS)
			{
				HtmlNode htmlNode = HtmlNode.CreateNode("<style type=\"text/css\"></style>");
				htmlNode.InnerHtml = text;
				HtmlNode nodeByCssSelector = this.GetNodeByCssSelector("head");
				nodeByCssSelector.ChildNodes.Add(htmlNode);
			}
			return text;
		}
		public void ReinitCSSStylesheets()
		{
			this.CSSStylesheets = new CSSStylesheetCollection(this);
			this.CSSStylesheets.AddRange(this);
			this.ReinitXPath();
		}
		public CSSProperty GetCSSProperty(string selector, string propertyName, string defaultValue)
		{
			string xpath = CssHelper.SelectorToXPath(selector);
			HtmlNodeCollection htmlNodeCollection = this.HtmlDocument.DocumentNode.SelectNodes(xpath);
			if (htmlNodeCollection != null && htmlNodeCollection.Count > 0)
			{
				HtmlNode node = htmlNodeCollection.First<HtmlNode>();
				return this.GetCSSProperty(node, propertyName, defaultValue);
			}
			return null;
		}
		public List<CSSProperty> GetCSSProperties(HtmlNode node, string propertyName)
		{
			return new List<CSSProperty>
			{
				this.CSSStylesheets.GetProperty(node, new List<string>
				{
					propertyName
				})
			};
		}
		public CSSProperty GetCSSProperty(HtmlNode node, string propertyName, string defaultValue)
		{
			List<CSSProperty> cSSProperties = this.GetCSSProperties(node, propertyName);
			List<CSSProperty> list = new List<CSSProperty>();
			foreach (CSSProperty current in cSSProperties)
			{
				if (current != null)
				{
					list.Add(current);
				}
			}
			if (list.Count > 0)
			{
				return list.First<CSSProperty>();
			}
			return new CSSProperty(null)
			{
				Name = propertyName,
				Value = defaultValue
			};
		}
		public void SetupHtml()
		{
			this.SetupHtml(true, true);
		}
		public void SetupHtml(bool setupCss, bool setupJs)
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			string value = ContextHelper.GetValue<string>("mode", "html");
			if (setupCss)
			{
				this.SetupCSS();
			}
			if (value.Equals("html"))
			{
				MobilePage.Log("  SetupCSS(): " + stopwatch.Elapsed);
				stopwatch.Restart();
			}
			this.ReplaceRelativeImageUrlToAbsoluteInImageTags();
			if (value.Equals("html"))
			{
				MobilePage.Log("  ReplaceRelativeImageUrlToAbsoluteInImageTags(): " + stopwatch.Elapsed);
				stopwatch.Restart();
			}
			if (value.Equals("html"))
			{
				MobilePage.Log("  SetupImageTags(): " + stopwatch.Elapsed);
				stopwatch.Restart();
			}
			this.SetupLinks();
			if (value.Equals("html"))
			{
				MobilePage.Log("  SetupLinks(): " + stopwatch.Elapsed);
				stopwatch.Restart();
			}
			if (this.Page.HideBrowserAddressbar)
			{
				this.HideAddressbar();
			}
			if (value.Equals("html"))
			{
				MobilePage.Log("  HideAddressbar(): " + stopwatch.Elapsed);
				stopwatch.Restart();
			}
			if (this.Page.AutoAddMobileMetaTags)
			{
				this.AddMobileMetaTags();
			}
			if (value.Equals("html"))
			{
				MobilePage.Log("  AddMobileMetaTags(): " + stopwatch.Elapsed);
				stopwatch.Restart();
			}
			if (ExternalConfiguration.GetSection().CSS)
			{
				this.RemoveAllStylingNodes();
				HtmlNode nodeByCssSelector = this.GetNodeByCssSelector("head");
				if (nodeByCssSelector != null)
				{
					ExtendedUrl extendedUrl = new ExtendedUrl(HttpContext.Current.Request.Url.ToString());
					extendedUrl.Querystring.Update("mode", "css");
					HtmlNode htmlNode = new HtmlNode(HtmlNodeType.Element, this.HtmlDocument, 0);
					htmlNode.Name = "link";
					htmlNode.Attributes.Add("rel", "stylesheet");
					htmlNode.Attributes.Add("type", "text/css");
					htmlNode.Attributes.Add("href", extendedUrl.ToString());
					nodeByCssSelector.ChildNodes.Insert(0, htmlNode);
				}
			}
			if (value.Equals("html"))
			{
				MobilePage.Log("  AddCSSLink(): " + stopwatch.Elapsed);
				stopwatch.Restart();
			}
		}
		private void AddMobileMetaTags()
		{
			HtmlNode nodeByCssSelector = this.GetNodeByCssSelector("head");
			if (!DeviceInfo.CurrentDevice.IsWindows && !DeviceInfo.CurrentDevice.IsWindowsPhone)
			{
				HtmlNode htmlNode = this.HtmlDocument.CreateElement("meta");
				htmlNode.Attributes.Add("name", "viewport");
				htmlNode.Attributes.Add("content", "width=device-width, target-densityDpi=device-dpi, initial-scale=1.0");
				nodeByCssSelector.ChildNodes.Add(htmlNode);
				return;
			}
			HtmlNode htmlNode2 = this.HtmlDocument.CreateElement("meta");
			htmlNode2.Attributes.Add("name", "viewport");
			htmlNode2.Attributes.Add("content", "width=" + DeviceInfo.CurrentDevice.DisplayWidth.ToString() + ", target-densityDpi=device-dpi, initial-scale=1.0");
			nodeByCssSelector.ChildNodes.Add(htmlNode2);
			HtmlNode htmlNode3 = this.HtmlDocument.CreateElement("meta");
			htmlNode3.Attributes.Add("name", "MobileOptimized");
			htmlNode3.Attributes.Add("content", DeviceInfo.CurrentDevice.DisplayWidth.ToString());
			nodeByCssSelector.ChildNodes.Add(htmlNode3);
			HtmlNode htmlNode4 = this.HtmlDocument.CreateElement("meta");
			htmlNode4.Attributes.Add("name", "HandheldFriendly");
			htmlNode4.Attributes.Add("content", "true");
			nodeByCssSelector.ChildNodes.Add(htmlNode4);
			HtmlNode htmlNode5 = this.HtmlDocument.CreateElement("meta");
			htmlNode5.Attributes.Add("http-equiv", "X-UA-Compatible");
			htmlNode5.Attributes.Add("content", "IE=8");
			nodeByCssSelector.ChildNodes.Add(htmlNode5);
		}
		private void HideAddressbar()
		{
			string text = "setTimeout(function() { window.scrollTo(0, 1) }, 100);";
			string xpath = CssHelper.SelectorToXPath("body");
			HtmlNode htmlNode = this.HtmlDocument.DocumentNode.SelectNodes(xpath).FirstOrDefault<HtmlNode>();
			if (htmlNode != null)
			{
				if (htmlNode.Attributes["onload"] == null)
				{
					htmlNode.Attributes.Add("onload", text);
					return;
				}
				HtmlAttribute expr_65 = htmlNode.Attributes["onload"];
				expr_65.Value += text;
			}
		}
		private void SetNodeIdentifiers()
		{
			HtmlNodeCollection nodesByCssSelector = this.GetNodesByCssSelector("body, body *");
			if (nodesByCssSelector != null)
			{
				foreach (HtmlNode current in (IEnumerable<HtmlNode>)nodesByCssSelector)
				{
					if (!current.Attributes.Contains("id") && !current.Attributes.Contains("Id"))
					{
						string value = string.IsNullOrEmpty(current.Id) ? ("id" + this.idSeed++) : current.Id;
						current.Attributes.Add("id", value);
					}
				}
			}
		}
		private void CacheCalculations()
		{
			HtmlNodeCollection nodesByCssSelector = this.GetNodesByCssSelector("body, body *");
			if (nodesByCssSelector != null)
			{
				foreach (HtmlNode current in (IEnumerable<HtmlNode>)nodesByCssSelector)
				{
					current.GetCalculatedWidth();
				}
			}
		}
		public HtmlNodeCollection GetAllStylingNodes()
		{
			List<HtmlNode> list = new List<HtmlNode>();
			foreach (HtmlNode current in (IEnumerable<HtmlNode>)this.GetAllNodesInHeadAndBody())
			{
				if (current.Name.Equals("style"))
				{
					list.Add(current);
				}
			}
			HtmlNodeCollection nodesByCssSelector = this.GetNodesByCssSelector("link[rel=\"stylesheet\"][momasdk=\"true\"]");
			for (int i = 0; i < list.Count; i++)
			{
				nodesByCssSelector.Insert(i, list[i]);
			}
			return nodesByCssSelector;
		}
		public void RemoveAllStylingNodes()
		{
			HtmlNodeCollection nodesByCssSelector = this.GetNodesByCssSelector("link, style");
			HtmlNode nodeByCssSelector = this.GetNodeByCssSelector("head");
			foreach (HtmlNode current in (IEnumerable<HtmlNode>)nodesByCssSelector)
			{
				if (current.Attributes["momasdk"] != null)
				{
					if (bool.Parse(current.Attributes["momasdk"].Value))
					{
						nodeByCssSelector.RemoveChild(current);
					}
				}
				else
				{
					if (Regex.Match(current.Name, "style", RegexOptions.IgnoreCase).Success)
					{
						nodeByCssSelector.RemoveChild(current);
					}
				}
			}
		}
		public HtmlNode GetNodeByCssSelector(string CssSelector)
		{
			string xpath = CssHelper.SelectorToXPath(CssSelector);
			HtmlNodeCollection htmlNodeCollection = this.HtmlDocument.DocumentNode.SelectNodes(xpath);
			if (htmlNodeCollection != null)
			{
				return htmlNodeCollection.FirstOrDefault<HtmlNode>();
			}
			return null;
		}
		public HtmlNodeCollection GetNodesByCssSelector(string CssSelector)
		{
			string xpath = CssHelper.SelectorToXPath(CssSelector);
			HtmlNodeCollection htmlNodeCollection = this.HtmlDocument.DocumentNode.SelectNodes(xpath);
			if (htmlNodeCollection == null)
			{
				return new HtmlNodeCollection(this.HtmlDocument.DocumentNode);
			}
			return htmlNodeCollection;
		}
		public HtmlNodeCollection GetAllNodesInBody()
		{
			return this.GetNodesByCssSelector("body *");
		}
		public HtmlNodeCollection GetAllNodesInHeadAndBody()
		{
			HtmlNodeCollection nodesByCssSelector = this.GetNodesByCssSelector("body *");
			foreach (HtmlNode current in (IEnumerable<HtmlNode>)this.HtmlDocument.DocumentNode.SelectNodes("//head")[0].ChildNodes)
			{
				nodesByCssSelector.Add(current);
			}
			return nodesByCssSelector;
		}
		private void SetupLinks()
		{
			string xpath = CssHelper.SelectorToXPath("a");
			HtmlNodeCollection htmlNodeCollection = this.HtmlDocument.DocumentNode.SelectNodes(xpath);
			if (htmlNodeCollection != null)
			{
				foreach (HtmlNode current in (IEnumerable<HtmlNode>)htmlNodeCollection)
				{
					if (current.Attributes.Contains("href"))
					{
						HtmlAttribute htmlAttribute = current.Attributes["href"];
						if ((htmlAttribute.Value.StartsWith("sms:") || htmlAttribute.Value.StartsWith("smsto:")) && !this.device.UriSchemeSmsTo && !this.device.UriSchemeSms)
						{
							if (current.Attributes.Contains("alt"))
							{
								current.InnerHtml = current.Attributes["alt"].Value;
							}
							htmlAttribute.Value = "#";
						}
						else
						{
							if (htmlAttribute.Value.StartsWith("sms:"))
							{
								if (this.device.UriSchemeSmsTo)
								{
									htmlAttribute.Value = htmlAttribute.Value.Replace("sms:", "smsto:");
								}
							}
							else
							{
								if (htmlAttribute.Value.StartsWith("smsto:") && this.device.UriSchemeSms)
								{
									htmlAttribute.Value = htmlAttribute.Value.Replace("smsto:", "sms:");
								}
							}
						}
						if (htmlAttribute.Value.StartsWith("tel:") && !this.device.UriSchemeTel)
						{
							htmlAttribute.Value = htmlAttribute.Value.Replace("tel:", "wtai://wp/mc;");
						}
						if (htmlAttribute.Value.StartsWith("wtai://wp/mc;") && this.device.UriSchemeTel)
						{
							htmlAttribute.Value = htmlAttribute.Value.Replace("wtai://wp/mc;", "tel:");
						}
					}
				}
			}
		}
		private void ReplaceRelativeImageUrlToAbsoluteInImageTags()
		{
			HtmlNodeCollection nodesByCssSelector = this.GetNodesByCssSelector("img");
			if (nodesByCssSelector != null)
			{
				foreach (HtmlNode current in (IEnumerable<HtmlNode>)nodesByCssSelector)
				{
					if (current.Attributes.Contains("src"))
					{
						string value = current.Attributes["src"].Value;
						Uri uri = null;
						if (Uri.TryCreate(value, UriKind.Relative, out uri))
						{
							current.Attributes["src"].Value = ExtendedUrl.GetCompleteUrl(uri);
						}
					}
				}
			}
		}
		private void SetupImageTags()
		{
			HtmlNodeCollection nodesByCssSelector = this.GetNodesByCssSelector("img");
			if (nodesByCssSelector != null)
			{
				foreach (HtmlNode current in (IEnumerable<HtmlNode>)nodesByCssSelector)
				{
					CSSProperty cSSProperty = this.GetCSSProperty(current, "width", current.GetCalculatedWidth().ToString());
					CSSProperty cSSProperty2 = this.GetCSSProperty(current, "height", null);
					int calculatedWidth = current.ParentNode.GetCalculatedWidth();
					int calculatedWidth2 = current.ParentNode.GetCalculatedWidth();
					int intValue = CSSMeasure.GetIntValue(cSSProperty.Value, calculatedWidth, calculatedWidth2);
					int? num = null;
					if (cSSProperty2.Value != null)
					{
						num = new int?(CSSMeasure.GetIntValue(cSSProperty2.Value, calculatedWidth, calculatedWidth2));
					}
					current.Map(this);
					if (current.Attributes.Contains("src"))
					{
						HtmlAttribute htmlAttribute = current.Attributes["src"];
						if (current.GetAttribute("scale", "true").Equals("true"))
						{
							ExtendedUrl extendedUrl;
							if (ExtendedUrl.IsExternal(htmlAttribute.Value) && !htmlAttribute.Value.Contains("googleapis"))
							{
								extendedUrl = new ExtendedUrl(HttpContext.Current.Request.Url.ToString());
								extendedUrl.Querystring.Update("mode", "scale");
								extendedUrl.Querystring.Update("url", HttpContext.Current.Server.UrlEncode(htmlAttribute.Value));
								extendedUrl.Querystring.Update("width", intValue.ToString());
							}
							else
							{
								extendedUrl = new ExtendedUrl(htmlAttribute.Value);
								extendedUrl.Querystring.Update("width", intValue.ToString());
								if (cSSProperty2.Value != null)
								{
									extendedUrl.Querystring.Update("height", num.ToString());
								}
							}
							extendedUrl.Querystring.Update("scale", "both");
							htmlAttribute.Value = extendedUrl.ToString();
						}
					}
				}
			}
		}
		public override string ToString()
		{
			string text = this.HtmlDocument.DocumentNode.InnerHtml;
			HtmlDocument htmlDocument = new HtmlDocument();
			htmlDocument.LoadHtml(text);
			HtmlNodeCollection htmlNodeCollection = htmlDocument.DocumentNode.SelectNodes("//*[@removed='true']");
			if (htmlNodeCollection != null)
			{
				foreach (HtmlNode current in (IEnumerable<HtmlNode>)htmlNodeCollection)
				{
					text = text.Replace(current.OuterHtml, "");
				}
			}
			text = text.Replace("mode=css\">", "mode=css\" />");
			return text.Trim();
		}
	}
}
