using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace MarkDown.UWP
{
    public sealed partial class SourceEditor : UserControl
    {
        public SourceEditor()
        {
            this.InitializeComponent();
        }

        private void srcView_ScriptNotify(object sender, NotifyEventArgs e)
        {
            
        }

        //public async void SetContent(string content)
        //{
        //    await sourceEditor.InvokeScriptAsync("setContent", new string[] { content });
        //}

        /// <summary>
        /// DependencyProperty for the TextEditor SelectionStart property. 
        /// </summary>
        public static readonly DependencyProperty CodeContentProperty =
             DependencyProperty.Register("CodeContent", typeof(string), typeof(SourceEditor),
             new PropertyMetadata(default(string),async (obj, args) =>
             {
                 SourceEditor editor = (SourceEditor)obj;
                 if ((string)args.OldValue == (string)args.NewValue)
                     return;
                 editor.CodeContent = args.NewValue.ToString();

                 try
                 {
                     await editor.sourceEditor.InvokeScriptAsync("setContent", new string[] { editor.CodeContent });
                 }
                 catch (Exception ex)
                 {

                 }
             }));

        /// <summary>
        /// Access to the SelectionStart property.
        /// </summary>
        public string CodeContent
        {
            get { return GetValue(CodeContentProperty).ToString(); }
            set { SetValue(CodeContentProperty, value); }
        }
    }
}
