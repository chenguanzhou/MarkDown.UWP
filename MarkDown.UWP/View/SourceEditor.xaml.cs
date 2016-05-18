﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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
    public sealed partial class SourceEditor : Page
    {
        public SourceEditor()
        {
            this.InitializeComponent();
            sourceEditor.NavigationCompleted += SourceEditor_NavigationCompleted;
        }

        public bool IsLoaded { get; set; } = false;
        public bool ShouldDelayLoad { get; set; } = false;
        private void sourceEditor_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            IsLoaded = false;
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
                ShouldDelayLoad = false;
            }

        }

        private async void srcView_ScriptNotify(object sender, NotifyEventArgs e)
        {
			switch (e.Value)
			{
			case "change":
				OnCodeContentChanged();
				break;

			case "scroll":
				OnScrollChanged();
				break;

			case "save":
				ViewModel.ViewModelLocator.Main.SaveCommand.Execute(null);
				break;

			case "open":
				ViewModel.ViewModelLocator.Main.OpenCommand.Execute(null);
				break;

			case "new":
				ViewModel.ViewModelLocator.Main.NewCommand.Execute(null);
				break;

			case "find":
				elemSearchBox.Focus(FocusState.Pointer);
				break;

			case "replace":
				// Todo: find a way to expend cmd bar
				elemSearchBox.Focus(FocusState.Pointer);
				break;

			default:
				await new MessageDialog(e.Value).ShowAsync();
				break;
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

        private async void SourceEditor_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            string theme = "default";
            if (ApplicationData.Current.LocalSettings.Values.Keys.Contains("UseLightTheme") && !(bool)ApplicationData.Current.LocalSettings.Values["UseLightTheme"])
                theme = "monokai";

            await sender.InvokeScriptAsync("setTheme", new string[] { theme });
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
                 catch (Exception)
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
                if (CodeContent != value)
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
                    await editor.sourceEditor.InvokeScriptAsync("setLineWrapping", new string[] { editor.IsLineWrapping ? "true" : "" });
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
		/// DependencyProperty for the LineNumber binding. 
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
		/// Provide access to the LineNumber.
		/// </summary>
		public bool IsShowLineNumber
        {
            get { return (bool)GetValue(IsShowLineNumberProperty); }
            set { SetValue(IsShowLineNumberProperty, value); }
        }

		/// <summary>
		/// DependencyProperty for the ActiveLine binding. 
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
		/// Provide access to the ActiveLine.
		/// </summary>
		public bool StyleActiveLine
        {
            get { return (bool)GetValue(StyleActiveLineProperty); }
            set { SetValue(StyleActiveLineProperty, value); }
        }

        /// <summary>
        /// DependencyProperty for the IsSearchMatchCase binding. 
        /// </summary>
        public static DependencyProperty IsSearchMatchCaseProperty =
            DependencyProperty.Register("IsSearchMatchCase", typeof(bool), typeof(SourceEditor),
                new PropertyMetadata(default(bool), async (obj, args) => 
                    {
                        SourceEditor editor = (SourceEditor)obj;
                        if (editor.IsLoaded)
                            await editor.sourceEditor.InvokeScriptAsync("setSearchMatchCase", new string[] { editor.IsSearchMatchCase ? "true" : "" });
                    }));

        /// <summary>
        /// Provide access to the IsSearchMatchCase.
        /// </summary>
        public bool IsSearchMatchCase
        {
            get { return (bool)GetValue(IsSearchMatchCaseProperty); }
            set { SetValue(IsSearchMatchCaseProperty, value); }
        }

        /// <summary>
        /// DependencyProperty for the IsMatchWholeWord binding. 
        /// </summary>
        public static DependencyProperty IsMatchWholeWordProperty =
            DependencyProperty.Register("IsMatchWholeWord", typeof(bool), typeof(SourceEditor),
                new PropertyMetadata(default(bool), async (obj, args) =>
                {
                    SourceEditor editor = (SourceEditor)obj;
                    if (editor.IsLoaded)
                        await editor.sourceEditor.InvokeScriptAsync("setIsMatchWholeWord", new string[] { editor.IsMatchWholeWord ? "true" : "" });
                }));

        /// <summary>
        /// Provide access to the IsMatchWholeWord.
        /// </summary>
        public bool IsMatchWholeWord
        {
            get { return (bool)GetValue(IsMatchWholeWordProperty); }
            set { SetValue(IsMatchWholeWordProperty, value); }
        }

        /// <summary>
        /// DependencyProperty for the UseWildcard binding. 
        /// </summary>
        public static DependencyProperty UseWildcardProperty =
            DependencyProperty.Register("UseWildcard", typeof(bool), typeof(SourceEditor),
                new PropertyMetadata(default(bool), async (obj, args) =>
                {
                    SourceEditor editor = (SourceEditor)obj;
                    if (editor.IsLoaded)
                        await editor.sourceEditor.InvokeScriptAsync("setUseWildcard", new string[] { editor.UseWildcard ? "true" : "" });
                }));

        /// <summary>
        /// Provide access to the UseWildcard.
        /// </summary>
        public bool UseWildcard
        {
            get { return (bool)GetValue(UseWildcardProperty); }
            set { SetValue(UseWildcardProperty, value); }
        }

        /// <summary>
        /// DependencyProperty for the UseRegularExpression binding. 
        /// </summary>
        public static DependencyProperty UseRegularExpressionProperty =
            DependencyProperty.Register("UseRegularExpression", typeof(bool), typeof(SourceEditor),
                new PropertyMetadata(default(bool), async (obj, args) =>
                {
                    SourceEditor editor = (SourceEditor)obj;
                    if (editor.IsLoaded)
                        await editor.sourceEditor.InvokeScriptAsync("setUseRegularExpression", new string[] { editor.UseRegularExpression ? "true" : "" });
                }));

        /// <summary>
        /// Provide access to the UseRegularExpression.
        /// </summary>
        public bool UseRegularExpression
        {
            get { return (bool)GetValue(UseRegularExpressionProperty); }
            set { SetValue(UseRegularExpressionProperty, value); }
        }

        /// <summary>
        /// DependencyProperty for the SearchText binding. 
        /// </summary>
        public static DependencyProperty SearchTextProperty =
            DependencyProperty.Register("SearchText", typeof(string), typeof(SourceEditor),
            new PropertyMetadata(default(string), async (obj, args) =>
            {
                SourceEditor editor = (SourceEditor)obj;
                if (editor.IsLoaded)
                    await editor.sourceEditor.InvokeScriptAsync("setSearchText", new string[] { editor.SearchText});
            }));

        /// <summary>
        /// Provide access to the FontFamily.
        /// </summary>
        public string SearchText
        {
            get { return (string)GetValue(SearchTextProperty); }
            set { SetValue(SearchTextProperty, value); }
        }

        /// <summary>
        /// DependencyProperty for the ReplaceText binding. 
        /// </summary>
        public static DependencyProperty ReplaceTextProperty =
            DependencyProperty.Register("ReplaceText", typeof(string), typeof(SourceEditor),
            new PropertyMetadata(default(string), async (obj, args) =>
            {
                SourceEditor editor = (SourceEditor)obj;
                if (editor.IsLoaded)
                    await editor.sourceEditor.InvokeScriptAsync("setReplaceText", new string[] { editor.ReplaceText });
            }));

        /// <summary>
        /// Provide access to the ReplaceText.
        /// </summary>
        public string ReplaceText
        {
            get { return (string)GetValue(ReplaceTextProperty); }
            set { SetValue(ReplaceTextProperty, value); }
        }

        private async void FindPrevious_Click(object sender, RoutedEventArgs e)
        {
            await sourceEditor.InvokeScriptAsync("findPrevious", new string[] { "" });
        }

        private async void FindNext_Click(object sender, RoutedEventArgs e)
        {
            await sourceEditor.InvokeScriptAsync("findNext", new string[] { "" });
        }

        private async void Replace_Click(object sender, RoutedEventArgs e)
        {
            await sourceEditor.InvokeScriptAsync("replace", new string[] { "" });
        }

        private async void ReplaceAll_Click(object sender, RoutedEventArgs e)
        {
            await sourceEditor.InvokeScriptAsync("replaceAll", new string[] { "" });
        }
    }
}
