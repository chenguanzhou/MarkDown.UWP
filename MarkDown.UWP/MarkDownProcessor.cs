using CommonMark;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace MarkDown.UWP
{
	public class MarkDownProcessor
	{
		public async Task<string> MD2HTML(string MarkdownContent, bool IsMobile) =>
			await Task.Run(async () =>
			{
				var body = md2body(MarkdownContent);
				List<string> js_list = new List<string>(), css_list = new List<string>();

				Windows.ApplicationModel.Package package = Windows.ApplicationModel.Package.Current;

				// Markdown CSS
				var cssFolder = await package.InstalledLocation.GetFolderAsync("css");
				string fileName = "Default.css";
				if (ApplicationData.Current.LocalSettings.Values.Keys.Contains("UseLightTheme") &&
					!(bool)ApplicationData.Current.LocalSettings.Values["UseLightTheme"])
					fileName = "Retro.css";
				var cssFile = await cssFolder.GetFileAsync(fileName);
				var css = await FileIO.ReadTextAsync(cssFile);
				css_list.Add(css);

				// Highlight JS/CSS
				if (body.Contains("<pre><code class=\"language-"))
				{
					StorageFolder highlightJSFolder = await package.InstalledLocation.GetFolderAsync("highlighting.js");
					var highlightJSFile = await highlightJSFolder.GetFileAsync("highlight.js");
					var js = await FileIO.ReadTextAsync(highlightJSFile);
					js_list.Add(js);
					js_list.Add("hljs.initHighlightingOnLoad();");

					var highlightJSCSSFile = await (await highlightJSFolder.GetFolderAsync("styles")).GetFileAsync("github.css");
					var js_css = await FileIO.ReadTextAsync(highlightJSCSSFile);
					css_list.Add(js_css);
				}

				return merge(
					body,
					css_list.Count != 0 ? css_list.ToArray() : null,
					js_list.Count != 0 ? js_list.ToArray() : null);
			});

		private string md2body(string src)
		{
			using (var reader = new StringReader(src))
			using (var writer = new StringWriter())
			{
				var setting = CommonMarkSettings.Default.Clone();
				setting.AdditionalFeatures = CommonMarkAdditionalFeatures.None;
				setting.RenderSoftLineBreaksAsLineBreaks = true;
				CommonMarkConverter.Convert(reader, writer, setting);
				return writer.ToString();
			}
		}

		private string merge(string body, string[] css = null, string[] js = null)
		{
			var cssStr = css == null ? "" : string.Join("\n", css.Select(s => $"<style type=\"text/css\">\n{s}\n</style>"));
			var jsStr = js == null ? "" : string.Join("\n", js.Select(s => $"<script async>\n{s}\n</script>"));

var html = 
            $@"
<html>
    <head>
        <meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"" />
        <script type=""text/x-mathjax-config"">
            MathJax.Hub.Config({{ tex2jax: {{ inlineMath: [['$', '$'], ['\\(','\\)']]}} }});
        </script>
		<script async src=""https://cdn.mathjax.org/mathjax/latest/MathJax.js?config=TeX-AMS_HTML""></script>
		<title>Markdown Preview</title>    
        {cssStr}
        {jsStr}
    </head>
    <body>
        {body}
    </body>        
</html>
            ";
            return html;
        }
    }
}
