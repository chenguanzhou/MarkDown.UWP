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
            useLightTheme = ApplicationData.Current.LocalSettings.Values.Keys.Contains("UseLightTheme") && ((bool)ApplicationData.Current.LocalSettings.Values["UseLightTheme"]);
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
            var dlg = new MessageDialog("restart by hand");
            if (AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Desktop")
            {
                dlg.Content = MainViewModel.ResourceLoader.GetString("EffectAfterRestart");
                dlg.Commands.Add(new UICommand("restart", async cmd =>
                {
                    await ((App)App.Current).Restart();
                }));
            }
            else
            {
                dlg.Commands.Add(new UICommand("exit app", async cmd =>
                {
                    await ViewModelLocator.Main.BackUp();
                    ((App)App.Current).Exit();
                }));
            }
            dlg.Commands.Add(new UICommand("cancel"));
            await dlg.ShowAsync();
        }

        public Dictionary<string,string> AllThemes = new Dictionary<string, string>() { { MainViewModel.ResourceLoader.GetString("Light"), "Light" }, { MainViewModel.ResourceLoader.GetString("Dark"), "Dark" } };
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

    }

    public class SettingsPreviewViewModel : ViewModelBase
    {

    }

    public class SettingsAboutViewModel : ViewModelBase
    {

    }
}