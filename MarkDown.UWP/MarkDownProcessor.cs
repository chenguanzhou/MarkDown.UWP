using CommonMark;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarkDown.UWP
{
    public class MarkDownProcessor
    {
        public static async Task<string> MD2HTML(string mdContent) => 
            await Task.Run(() =>
            {
                using (var reader = new StringReader(mdContent))
                using (var writer = new StringWriter())
                {
                    CommonMarkConverter.Convert(reader, writer);
                    return writer.ToString();
                }
            });
    }
}
