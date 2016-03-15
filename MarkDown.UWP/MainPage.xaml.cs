using MarkdownSharp;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MarkDown.UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            MarkdownOptions option = new MarkdownOptions();
            option.AutoHyperlink = true;
            option.AutoNewlines = true;
            option.LinkEmails = true;
            option.StrictBoldItalic = false;

            processor = new Markdown(option);
        }

        private Markdown processor;

        //private void WebView_ScriptNotify(object sender, NotifyEventArgs e)
        //{
        //    var src = e.Value;
        //    var rawHTML = processor.Transform(src);            
        //    preView.NavigateToString(rawHTML);
        //}

        //private async void ButtonOpen_Click(object sender, RoutedEventArgs e)
        //{
        //    var picker = new FileOpenPicker();
        //    picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.List;
        //    picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
        //    picker.FileTypeFilter.Add(".md");
        //    picker.FileTypeFilter.Add(".markdown");
        //    var file = await picker.PickSingleFileAsync();
        //    var buffer = await FileIO.ReadBufferAsync(file);
        //    var reader = new StreamReader(buffer.AsStream());
        //    var content = reader.ReadToEnd();
        //}
    }
}
