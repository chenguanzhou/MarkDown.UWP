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
        public async Task<string> MD2HTML(string mdContent) => 
            await Task.Run(async () =>
            {
                var body = md2body(mdContent);

                Windows.ApplicationModel.Package package = Windows.ApplicationModel.Package.Current;

                StorageFolder highlightJSFolder = await package.InstalledLocation.GetFolderAsync("highlighting.js");
                var highlightJSFile = await highlightJSFolder.GetFileAsync("highlight.js") ;
                var js = await FileIO.ReadTextAsync(highlightJSFile);

                var highlightJSCSSFile = await (await highlightJSFolder.GetFolderAsync("styles")).GetFileAsync("default.css");
                var js_css = await FileIO.ReadTextAsync(highlightJSCSSFile);

                var cssFolder = await package.InstalledLocation.GetFolderAsync("css");
                var cssFile = await cssFolder.GetFileAsync("Default.css");
                var css = await FileIO.ReadTextAsync(cssFile);


                return merge(body, new string[] { js_css, css }, new string[] { js, "hljs.initHighlightingOnLoad();" });
            });

        private string md2body(string src)
        {
            using (var reader = new StringReader(src))
            using (var writer = new StringWriter())
            {
                var setting = CommonMarkSettings.Default.Clone();
                setting.AdditionalFeatures = CommonMarkAdditionalFeatures.All;
                setting.RenderSoftLineBreaksAsLineBreaks = true;
                CommonMarkConverter.Convert(reader, writer, setting);
                return writer.ToString();
            }
        }

        private string merge(string body, string[] css = null, string[] js = null)
        {
            var cssStr = css == null ? "" : string.Join("\n", css.Select(s => $"<style type=\"text/css\">\n{s}\n</style>"));
            var jsStr = js == null ? "" : string.Join("\n", js.Select(s => $"<script>\n{s}\n</script>"));
            var html = 
            $@"
<html>
    <head>
        <meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"" />
        <title></title>    
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
