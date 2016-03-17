using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;

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
            if (e.Value == "change")
            {
                OnCodeContentChanged();
            }
            else if (e.Value == "scroll")
            {
                OnScrollChanged();
            }
        }

        private async void OnCodeContentChanged()
        {
            UpdateToHtml = false;
            CodeContent = await sourceEditor.InvokeScriptAsync("getContent", null);
            UpdateToHtml = true;
        }

        private async void OnScrollChanged()
        {
            var ret = await sourceEditor.InvokeScriptAsync("getScrollRatio", null);
            ScrollRatio = double.Parse(ret);
        }

        /// <summary>
        /// DependencyProperty for the SourceEditor CodeContent property. 
        /// </summary>
        private bool UpdateToHtml = true;
        public static readonly DependencyProperty CodeContentProperty =
             DependencyProperty.Register("CodeContent", typeof(string), typeof(SourceEditor),
             new PropertyMetadata(default(string), async (obj, args) =>
             {
                 SourceEditor editor = (SourceEditor)obj;
                 if ((string)args.OldValue == (string)args.NewValue)
                     return;

                 try
                 {
                     if (editor.UpdateToHtml)
                     {
                         if (editor.IsLoaded)
                             await editor.sourceEditor.InvokeScriptAsync("setContent", new string[] { editor.CodeContent });
                         else
                             editor.IsLoadContentDelay = true;
                     }
                 }
                 catch (Exception ex)
                 {

                 }
             }));


        /// <summary>
        /// Access to the CodeContent property.
        /// </summary>
        public string CodeContent
        {
            get { return GetValue(CodeContentProperty).ToString(); }
            set
            {
                if(CodeContent != value)
                    SetValue(CodeContentProperty, value);
            }
        }

        /// <summary>
        /// DependencyProperty for the SourceEditor ScrollRatio property. 
        /// </summary>
        public static readonly DependencyProperty ScrollRatioProperty =
             DependencyProperty.Register("ScrollRatio", typeof(double), typeof(SourceEditor),
             new PropertyMetadata(0.0));

        /// <summary>
        /// Access to the ScrollRatio property.
        /// </summary>
        public double ScrollRatio
        {
            get { return (double)GetValue(ScrollRatioProperty); }
            set
            {
                SetValue(ScrollRatioProperty, value);
            }
        }

        

        public bool IsLoadContentDelay { get; set; } = false;

        public bool IsLoaded { get; set; } = false;
        private async void sourceEditor_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            IsLoaded = true;
            if (IsLoadContentDelay)
            {
                await sourceEditor.InvokeScriptAsync("setContent", new string[] { CodeContent });
                IsLoadContentDelay = false;
            }

        }
    }
}
