using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
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

        public bool IsLoaded { get; set; } = false;
        public bool ShouldDelayLoad { get; set; } = false;
        private void sourceEditor_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            IsLoaded = false;
            IsEnabled = false;
        }
        private async void sourceEditor_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            IsLoaded = true;
            if (ShouldDelayLoad)
            {
                await sourceEditor.InvokeScriptAsync("setContent", new string[] { CodeContent });
                await sourceEditor.InvokeScriptAsync("setFontFamily", new string[] { FontFamily }); 
                await sourceEditor.InvokeScriptAsync("setLineWrapping", new string[] { IsLineWrapping ? "true" : "" });
                await sourceEditor.InvokeScriptAsync("setShowLineNumber", new string[] { IsShowLineNumber ? "true" : "" });
                await sourceEditor.InvokeScriptAsync("setStyleActiveLine", new string[] { StyleActiveLine ? "true" : "" });
                IsEnabled = true;
                ShouldDelayLoad = false;
            }

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
                             editor.ShouldDelayLoad = true;
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

        /// <summary>
        /// DependencyProperty for the FontFamily binding. 
        /// </summary>
        public new static DependencyProperty FontFamilyProperty =
            DependencyProperty.Register("FontFamily", typeof(string), typeof(SourceEditor),
            new PropertyMetadata(default(string), async (obj, args) =>
            {
                SourceEditor editor = (SourceEditor)obj;
                if (editor.IsLoaded)
                    await editor.sourceEditor.InvokeScriptAsync("setFontFamily", new string[] { editor.FontFamily });
            }));

        /// <summary>
        /// Provide access to the FontFamily.
        /// </summary>
        public new string FontFamily
        {
            get { return (string)GetValue(FontFamilyProperty); }
            set { SetValue(FontFamilyProperty, value); }
        }

        /// <summary>
        /// DependencyProperty for the LineWrapping binding. 
        /// </summary>
        public static DependencyProperty IsLineWrappingProperty =
            DependencyProperty.Register("IsLineWrapping", typeof(bool), typeof(SourceEditor),
            new PropertyMetadata(default(bool), async (obj, args) =>
            {
                SourceEditor editor = (SourceEditor)obj;
                if (editor.IsLoaded)
                    await editor.sourceEditor.InvokeScriptAsync("setLineWrapping", new string[] { editor.IsLineWrapping ? "true":"" });
            }));

        /// <summary>
        /// Provide access to the LineWrapping.
        /// </summary>
        public bool IsLineWrapping
        {
            get { return (bool)GetValue(IsLineWrappingProperty); }
            set { SetValue(IsLineWrappingProperty, value); }
        }

        /// <summary>
        /// DependencyProperty for the LineWrapping binding. 
        /// </summary>
        public static DependencyProperty IsShowLineNumberProperty =
            DependencyProperty.Register("IsShowLineNumber", typeof(bool), typeof(SourceEditor),
            new PropertyMetadata(default(bool), async (obj, args) =>
            {
                SourceEditor editor = (SourceEditor)obj;
                if (editor.IsLoaded)
                    await editor.sourceEditor.InvokeScriptAsync("setShowLineNumber", new string[] { editor.IsShowLineNumber ? "true" : "" });
            }));

        /// <summary>
        /// Provide access to the LineWrapping.
        /// </summary>
        public bool IsShowLineNumber
        {
            get { return (bool)GetValue(IsShowLineNumberProperty); }
            set { SetValue(IsShowLineNumberProperty, value); }
        }        
        
        /// <summary>
        /// DependencyProperty for the LineWrapping binding. 
        /// </summary>
        public static DependencyProperty StyleActiveLineProperty =
            DependencyProperty.Register("StyleActiveLine", typeof(bool), typeof(SourceEditor),
            new PropertyMetadata(default(bool), async (obj, args) =>
            {
                SourceEditor editor = (SourceEditor)obj;
                if (editor.IsLoaded)
                    await editor.sourceEditor.InvokeScriptAsync("setStyleActiveLine", new string[] { editor.StyleActiveLine ? "true" : "" });
            }));

        /// <summary>
        /// Provide access to the LineWrapping.
        /// </summary>
        public bool StyleActiveLine
        {
            get { return (bool)GetValue(StyleActiveLineProperty); }
            set { SetValue(StyleActiveLineProperty, value); }
        }
    }
}
