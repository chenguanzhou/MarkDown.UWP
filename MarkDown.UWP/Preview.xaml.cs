using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
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
    public sealed partial class Preview : UserControl
    {
        public Preview()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// DependencyProperty for the TextEditor SelectionStart property. 
        /// </summary>
        public static readonly DependencyProperty PreviewHTMLProperty =
             DependencyProperty.Register("PreviewHTML", typeof(string), typeof(Preview),
             new PropertyMetadata(default(string), (obj, args) =>
             {
                 Preview preview = (Preview)obj;
                 if ((string)args.OldValue == (string)args.NewValue)
                     return;
                 preview.PreviewHTML = args.NewValue.ToString();
                 preview.preview.NavigateToString(preview.PreviewHTML);
             }));

        /// <summary>
        /// Access to the SelectionStart property.
        /// </summary>
        public string PreviewHTML
        {
            get { return GetValue(PreviewHTMLProperty).ToString(); }
            set { SetValue(PreviewHTMLProperty, value); }
        }

        /// <summary>
        /// DependencyProperty for the ScrollOffsetRatio binding. 
        /// </summary>
        public static DependencyProperty ScrollOffsetRatioProperty =
            DependencyProperty.Register("ScrollOffsetRatio", typeof(double), typeof(Preview),
            new PropertyMetadata(default(double), async (obj, args) =>
            {
                Preview target = (Preview)obj;
                target.ScrollOffsetRatio = (double)args.NewValue;

                await target.preview.InvokeScriptAsync("eval" , new string[] { $"scrollTo(0, {target.ScrollOffsetRatio} * (document.body.scrollHeight - window.innerHeight))" });
            }));

        /// <summary>
        /// Provide access to the ScrollOffsetRatio.
        /// </summary>
        public double ScrollOffsetRatio
        {
            get { return (double)GetValue(ScrollOffsetRatioProperty); }
            set { SetValue(ScrollOffsetRatioProperty, value); }
        }

        private async void preview_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            if (args.Uri != null)
            {
                args.Cancel = true;
                await Launcher.LaunchUriAsync(args.Uri);
            }
        }

        private async void preview_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            await preview.InvokeScriptAsync("eval", new string[] { $"scrollTo(0, {ScrollOffsetRatio} * (document.body.scrollHeight - window.innerHeight))" });
        }
    }
}
