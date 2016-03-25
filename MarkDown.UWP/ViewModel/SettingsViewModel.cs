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
using Windows.System;
using Windows.System.Profile;
using Windows.UI.Popups;
using Windows.UI.Xaml;

namespace MarkDown.UWP.ViewModel
{
    public class SettingsViewModel : ViewModelBase
    {
        public SettingsEnvironmentViewModel SettingsEnvironmentViewModel { get; } = new SettingsEnvironmentViewModel();
        public SettingsEditorViewModel SettingsEditorViewModel { get; } = new SettingsEditorViewModel();
        public SettingsPreviewViewModel SettingsPreviewViewModel { get; } = new SettingsPreviewViewModel();
        public SettingsAboutViewModel SettingsAboutViewModel { get; } = new SettingsAboutViewModel();
    }


    public class SettingsEnvironmentViewModel : ViewModelBase
    {
        public SettingsEnvironmentViewModel()
        {
            useLightTheme = !(ApplicationData.Current.LocalSettings.Values.Keys.Contains("UseLightTheme") && !((bool)ApplicationData.Current.LocalSettings.Values["UseLightTheme"]));
        }

        private bool useLightTheme = false;
        public bool UseLightTheme
        {
            get { return useLightTheme; }
            set
            {
                SetUseLightTheme(value);
            }
        }

        private async void SetUseLightTheme(bool value)
        {
            if (useLightTheme == value)
                return;
            useLightTheme = value;
            RaisePropertyChanged("UseLightTheme");
            ApplicationData.Current.LocalSettings.Values["UseLightTheme"] = useLightTheme;

            MessageDialog dlg;
            if (MainViewModel.IsDesktopPlatform)
            {
                dlg = new MessageDialog(MainViewModel.ResourceLoader.GetString("EffectAfterRestart"), MainViewModel.ResourceLoader.GetString("NeedToRestart"));
                dlg.Commands.Add(new UICommand(MainViewModel.ResourceLoader.GetString("Restart"), async cmd =>
                {
                    await ((App)App.Current).Restart();
                }));
            }
            else
            {
                dlg = new MessageDialog(MainViewModel.ResourceLoader.GetString("EffectAfterRestart4Mobile"),MainViewModel.ResourceLoader.GetString("NeedToRestart") );
                dlg.Commands.Add(new UICommand(MainViewModel.ResourceLoader.GetString("Exit"), async cmd =>
                {
                    await ViewModelLocator.Main.BackUp();
                    ((App)App.Current).Exit();
                }));
            }
            dlg.Commands.Add(new UICommand(MainViewModel.ResourceLoader.GetString("Cancel")));
            await dlg.ShowAsync();
        }
    }

    public class SettingsEditorViewModel : ViewModelBase
    {
        public SettingsEditorViewModel()
        {
        }

        private string fontFamily = ApplicationData.Current.LocalSettings.Values.Keys.Contains("FontFamily")? 
            ((string)ApplicationData.Current.LocalSettings.Values["FontFamily"]) : "sans-serif";
        public string FontFamily
        {
            get { return fontFamily; }
            set
            {
                Set(ref fontFamily,value);
                ApplicationData.Current.LocalSettings.Values["FontFamily"] = value;
            }
        }

        public string[] AllSupportedFonts { get; } = new string[] { "Arial", "Calibri", "Comic Sans", "Monospace", "sans-serif" , "Times New Roman", "Verdana" };


        private bool isLineWrapping = ApplicationData.Current.LocalSettings.Values.Keys.Contains("IsLineWrapping") ?
            ((bool)ApplicationData.Current.LocalSettings.Values["IsLineWrapping"]) : true;
        public bool IsLineWrapping
        {
            get { return isLineWrapping; }
            set
            {
                Set(ref isLineWrapping, value);
                ApplicationData.Current.LocalSettings.Values["IsLineWrapping"] = value;
            }
        }

        private bool isShowLineNumber = ApplicationData.Current.LocalSettings.Values.Keys.Contains("IsShowLineNumber") ?
            ((bool)ApplicationData.Current.LocalSettings.Values["IsShowLineNumber"]) : true;
        public bool IsShowLineNumber
        {
            get { return isShowLineNumber; }
            set
            {
                Set(ref isShowLineNumber, value);
                ApplicationData.Current.LocalSettings.Values["IsShowLineNumber"] = value;
            }
        }

        private bool styleActiveLine = ApplicationData.Current.LocalSettings.Values.Keys.Contains("StyleActiveLine") ?
            ((bool)ApplicationData.Current.LocalSettings.Values["StyleActiveLine"]) : true;
        public bool StyleActiveLine
        {
            get { return styleActiveLine; }
            set
            {
                Set(ref styleActiveLine, value);
                ApplicationData.Current.LocalSettings.Values["StyleActiveLine"] = value;
            }
        }
    }

    public class SettingsPreviewViewModel : ViewModelBase
    {

    }

    public class SettingsAboutViewModel : ViewModelBase
    {

    }
}