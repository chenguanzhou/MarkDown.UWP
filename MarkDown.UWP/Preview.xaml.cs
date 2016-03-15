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
    }
}
