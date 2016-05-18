using CommonMark;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
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
using Windows.Storage.Streams;
using Windows.System.Profile;
using Windows.UI.Popups;

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
            Content = "";
        }

        public override void Cleanup()
        {
            base.Cleanup();
        }

        public static ResourceLoader ResourceLoader = new ResourceLoader();

        static public string AppName => ResourceLoader.GetString("AppName");

        public SettingsViewModel SettingsViewModel { get; } = new SettingsViewModel();

        static public bool IsDesktopPlatform { get; } = AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Desktop";

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

            ApplicationData.Current.LocalSettings.Values["IsShowPreview"] = IsShowPreview;
            ApplicationData.Current.LocalSettings.Values["FileEncoding"] = FileEncoding.CodePage;
        }

        public async Task Restore()
        {
            try
            {
                if (ApplicationData.Current.LocalSettings.Values.Keys.Contains("IsShowPreview"))
                    IsShowPreview = (bool)ApplicationData.Current.LocalSettings.Values["IsShowPreview"];
                if (ApplicationData.Current.LocalSettings.Values.Keys.Contains("FileEncoding"))
                    FileEncoding = Encoding.GetEncoding((int)ApplicationData.Current.LocalSettings.Values["FileEncoding"]);

                //First Time Open
                if (!ApplicationData.Current.LocalSettings.Values.Keys.Contains("HasEditorOpened"))
                {
                    Content = ResourceLoader.GetString("FirstDocumentContent");
                    ApplicationData.Current.LocalSettings.Values["HasEditorOpened"] = true;
                    return;
                }

                StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                StorageFile file;
                try
                {
                    file = await localFolder.GetFileAsync("BackUp.txt");
                }
                catch (Exception)
                {
                    return;
                }
                    
                var backup = await FileIO.ReadTextAsync(file);
                JObject obj = JObject.Parse(backup);
                Content = obj["Content"].ToString();

                var token = obj?["Token"]?.ToString();
                if (token != null)
                {
                    DocumentFile = await Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.GetFileAsync(token);
                    DocumentTitle = DocumentFile.Name;
                    IsModified = (bool)obj["IsModified"];
                }
            }
            catch (Exception)
            {
                IsModified = true;
            }
        }

        public string Title => DocumentTitle + (IsModified ? "(*)" : "");

        public string documentTitle = ResourceLoader.GetString("UntitledTitle");
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

        public Encoding FileEncoding { get; set; } = Encoding.UTF8;

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

        private string content;
        public string Content
        {
            get { return content; }
            set
            {
                if (content == value)
                    return;
                Set(ref content, value);
                IsModified = true;

                UpdatePreviewHTML();
            }
        }

        MarkDownProcessor processor = new MarkDownProcessor();
        private async void UpdatePreviewHTML()
        {
            PreviewHTML = await processor.MD2HTML(Content, !IsDesktopPlatform);
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

        #region ShowPreview
        private string isNarrowState = "NarrowState";
        public string NarrowState
        {
            get { return isNarrowState; }
            set
            {
                if (isNarrowState == value)
                    return;
                isNarrowState = value;
                RaisePropertyChanged("IsNarrowState");

                //if (IsShowPreview == null)
                //    IsShowPreview = isNarrowState != "NarrowState";

                if (isNarrowState == "NarrowState")
                {                    
                    PreviewWidth = IsShowPreview == true ? "*" : "0";
                    SourceCodeWidth = IsShowPreview == true ? "0" : "*";
                }
                else
                {
                    PreviewWidth = IsShowPreview == true ? "*" : "0";
                    SourceCodeWidth = "*";
                }
            }
        }

        public bool showPreview = IsDesktopPlatform;
        public bool IsShowPreview
        {
            get { return showPreview; }
            set
            {
                if (showPreview == value)
                    return;
                showPreview = value;                
                RaisePropertyChanged("IsShowPreview");
                if (isNarrowState == "NarrowState")
                {
                    PreviewWidth = IsShowPreview == true ? "*" : "0";
                    SourceCodeWidth = IsShowPreview == true ? "0" : "*";
                }
                else
                {
                    PreviewWidth = IsShowPreview == true ? "*" : "0";
                    SourceCodeWidth = "*";
                }
            }
        }

        public string sourceCodeWidth = "*";
        public string SourceCodeWidth
        {
            get { return sourceCodeWidth; }
            set { Set(ref sourceCodeWidth, value);}
        }

        public string previewWidth = "0";
        public string PreviewWidth
        {
            get { return previewWidth; }
            set
            {
                Set(ref previewWidth, value);
            }
        }

        #endregion

        #region Search

        public string searchText = "";
        public string SearchText
        {
            get { return searchText; }
            set
            {
                Set(ref searchText, value);
            }
        }

        public string replaceText = "";
        public string ReplaceText
        {
            get { return replaceText; }
            set
            {
                Set(ref replaceText, value);
            }
        }

        public bool isSearchMactchCase = false;
        public bool IsSearchMactchCase
        {
            get { return isSearchMactchCase; }
            set
            {
                Set(ref isSearchMactchCase, value);
            }
        }

        public bool isMatchWholeWord = false;
        public bool IsMatchWholeWord
        {
            get { return isMatchWholeWord; }
            set
            {
                Set(ref isMatchWholeWord, value);
            }
        }

        public bool useWildcard = false;
        public bool UseWildcard
        {
            get { return useWildcard; }
            set
            {
                Set(ref useWildcard, value);
            }
        }

        public bool useRegularExpression = false;
        public bool UseRegularExpression
        {
            get { return useRegularExpression; }
            set
            {
                Set(ref useRegularExpression, value);
            }
        }
        #endregion

        #region Documents Commands

        public void NewDoc()
        {
            Content = "";
            DocumentTitle = ResourceLoader.GetString("UntitledTitle");
            DocumentFile = null;
            FileEncoding = Encoding.UTF8;
            IsModified = false;
        }

        public ICommand NewCommand => new RelayCommand(async () =>
        {
            if (IsModified)
            {
                var dlg = new MessageDialog(ResourceLoader.GetString("WhetherSave"), DocumentTitle);
                dlg.Commands.Add(new UICommand(ResourceLoader.GetString("Save"), async cmd => { await Save(); NewDoc(); }));
                dlg.Commands.Add(new UICommand(ResourceLoader.GetString("NoSave"), cmd => { NewDoc(); }));
                if (IsDesktopPlatform)
                    dlg.Commands.Add(new UICommand(ResourceLoader.GetString("Cancel")));
                await dlg.ShowAsync();
            }
            else
                NewDoc();            
        });


        public async Task OpenDoc(StorageFile file = null)
        {
            if (IsModified)
            {
                var dlg = new MessageDialog(ResourceLoader.GetString("WhetherSave"), DocumentTitle);
                dlg.Commands.Add(new UICommand(ResourceLoader.GetString("Save"), async cmd => { await Save(); await Open(file); }));
                dlg.Commands.Add(new UICommand(ResourceLoader.GetString("NoSave"), async cmd => { await Open(file); }));
                if (IsDesktopPlatform)
                    dlg.Commands.Add(new UICommand(ResourceLoader.GetString("Cancel")));
                await dlg.ShowAsync();
            }
            else
                await Open(file);
        }

        public async Task Open(StorageFile file = null)
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
            FileEncoding = SimpleHelpers.FileEncoding.DetectFileEncoding(buffer.AsStream(), Encoding.UTF8);
            var reader = new StreamReader(buffer.AsStream(), FileEncoding);

            Content = reader.ReadToEnd().Replace("\r\n","\n");
            DocumentFile = file;
            DocumentTitle = file.Name;
            IsModified = false;
        }

        public ICommand OpenCommand => new RelayCommand(async () => 
        {
            await OpenDoc();
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
                picker.FileTypeChoices.Add(ResourceLoader.GetString("MarkdownDocument"), new List<string>() { ".md", ".markdown" });
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
            //await FileIO.WriteTextAsync(file, Content);

            var bytes = FileEncoding.GetBytes(Content);
            await FileIO.WriteBytesAsync(file, bytes);
        }


		public ICommand ExportCommand => new RelayCommand(async () =>
		{
			await Export();
		});

		private async Task<bool> Export()
		{
			var picker = new FileSavePicker();
			picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
			picker.FileTypeChoices.Add(ResourceLoader.GetString("HTMLDocument"), new List<string>() { ".htm", ".html" });
			if (DocumentTitle.Contains("."))
				picker.SuggestedFileName = DocumentTitle.Substring(0, DocumentTitle.IndexOf('.')) + ".htm";
			else
				picker.SuggestedFileName = DocumentTitle + ".htm";
			StorageFile file = await picker.PickSaveFileAsync();

			if (file == null)
				return false;

			var bytes = FileEncoding.GetBytes(PreviewHTML);
			await FileIO.WriteBytesAsync(file, bytes);

			return true;
		}

		#endregion
	}
}