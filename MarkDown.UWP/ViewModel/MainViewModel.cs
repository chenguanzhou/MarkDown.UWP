using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace MarkDown.UWP.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            
        }

        public override void Cleanup()
        {
            base.Cleanup();
        }

        private string content = "";
        public string Content
        {
            get { return content; }
            set
            {
                if (content == value)
                    return;
                content = value;
                RaisePropertyChanged("Content");
            }
        }

        public ICommand OpenCommand => new RelayCommand(async () => 
        {
            var picker = new FileOpenPicker();
            picker.ViewMode = PickerViewMode.List;
            picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            picker.FileTypeFilter.Add(".md");
            picker.FileTypeFilter.Add(".markdown");
            var file = await picker.PickSingleFileAsync();
            if (file == null)
                return;
            var buffer = await FileIO.ReadBufferAsync(file);
            var reader = new StreamReader(buffer.AsStream());

            Content = reader.ReadToEnd();
        });
    }
}