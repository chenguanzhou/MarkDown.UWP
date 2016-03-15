using CommonMark;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
//using MarkdownSharp;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Windows.ApplicationModel.Resources;
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
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        public override void Cleanup()
        {
            base.Cleanup();
        }

        static ResourceLoader loader = new ResourceLoader();

        static public string AppName => loader.GetString("AppName");

        public async Task BackUp()
        {
            

            JObject obj = new JObject();
            obj["Content"] = Content;
            obj["IsModified"] = IsModified;
            if (DocumentFile != null)
            {
                var tokon = Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.Add(DocumentFile);
                obj["Token"] = tokon;
            }


            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            StorageFile file = await localFolder.CreateFileAsync("BackUp.txt", CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(file, obj.ToString());

        }

        public async Task Restore()
        {
            try
            {
                StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                StorageFile file = await localFolder.GetFileAsync("BackUp.txt");
                var backup = await FileIO.ReadTextAsync(file);
                JObject obj = JObject.Parse(backup);
                Content = obj["Content"].ToString();
                IsModified = (bool)obj["IsModified"];

                var token = obj?["Token"]?.ToString();
                if (token != null)
                {
                    DocumentFile = await Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.GetFileAsync(token);
                    DocumentTitle = DocumentFile.Name;
                }
            }
            catch (Exception)
            {

            }
        }

        public string Title => DocumentTitle + (IsModified ? "(*)" : "");

        public string documentTitle = loader.GetString("UntitledTitle");
        public string DocumentTitle
        {
            get { return documentTitle; }
            set
            {
                Set(ref documentTitle, value);
                RaisePropertyChanged("Title");
            }
        }

        public StorageFile documentFile = null;
        public StorageFile DocumentFile
        {
            get { return documentFile; }
            set
            {
                Set(ref documentFile, value);
            }
        }

        public bool isModified = false;
        public bool IsModified
        {
            get { return isModified; }
            set
            {
                Set(ref isModified, value);
                RaisePropertyChanged("Title");
            }
        }

        private string content = "";
        public string Content
        {
            get { return content; }
            set
            {
                if (content == value)
                    return;
                Set(ref content, value);
                IsModified = true;

                using (var reader = new StringReader(content))
                using (var writer = new StringWriter())
                {
                    CommonMarkConverter.Convert(reader, writer);
                    PreviewHTML = writer.ToString();
                }
            }
        }

        private string previewHTML = "";
        public string PreviewHTML
        {
            get { return previewHTML; }
            set
            {
                Set(ref previewHTML, value);
            }
        }

        private double scrollRatio = 0.0;
        public double ScrollRatio
        {
            get { return scrollRatio; }
            set
            {
                Set(ref scrollRatio, value);
            }
        }

        public ICommand NewCommand => new RelayCommand(() =>
        {
            Content = "";
            DocumentTitle = loader.GetString("UntitledTitle");
            IsModified = false;
        });


        public async void OpenDoc(StorageFile file = null)
        {
            if (file == null)
            {
                var picker = new FileOpenPicker();
                picker.ViewMode = PickerViewMode.List;
                picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                picker.FileTypeFilter.Add(".md");
                picker.FileTypeFilter.Add(".markdown");
                file = await picker.PickSingleFileAsync();
                if (file == null)
                    return;                
            }

            var buffer = await FileIO.ReadBufferAsync(file);
            var encoder = SimpleHelpers.FileEncoding.DetectFileEncoding(buffer.AsStream(), Encoding.UTF8);
            var reader = new StreamReader(buffer.AsStream(), encoder);

            Content = reader.ReadToEnd().Replace("\r\n","\n");
            DocumentFile = file;
            DocumentTitle = file.Name;
            IsModified = false;
        }

        public ICommand OpenCommand => new RelayCommand(() => 
        {
            OpenDoc();
        });


        public ICommand SaveCommand => new RelayCommand(async () =>
        {
            await Save();
            IsModified = false;
        });

        private async Task<bool> Save()
        {
            if (DocumentFile == null)
            {
                var picker = new FileSavePicker();
                picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                picker.FileTypeChoices.Add("Markdown document", new List<string>() { ".md", ".markdown" });
                picker.SuggestedFileName = DocumentTitle;
                StorageFile file = await picker.PickSaveFileAsync();
                if (file != null)
                {
                    SaveDoc2File(file);
                    DocumentFile = file;
                    DocumentTitle = file.Name;
                    IsModified = false;
                }
                else
                    return false;
            }
            else
            {
                SaveDoc2File(DocumentFile);
            }
            return true;
        }

        private async void SaveDoc2File(StorageFile file)
        {
            await FileIO.WriteTextAsync(file, Content);
        }
    }
}