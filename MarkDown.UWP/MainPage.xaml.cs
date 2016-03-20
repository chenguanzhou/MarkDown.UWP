using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.ViewManagement;
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
        //private bool isCtrlKeyPressed = false;

        public MainPage()
        {
            //App.Dispatcher = Dispatcher;
            this.InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, grid.ActualWidth > 720 ? "WideState" : "NarrowState", false);
        }

        internal class ComboBoxItem
        {
        }

        //private void Grid_KeyUp(object sender, KeyRoutedEventArgs e)
        //{
        //    if (e.Key == VirtualKey.Control) isCtrlKeyPressed = false;
        //}

        //private void Grid_KeyDown(object sender, KeyRoutedEventArgs e)
        //{
        //    if (e.Key == VirtualKey.Control) isCtrlKeyPressed = true;
        //    else if (isCtrlKeyPressed)
        //    {
        //        var vm = ViewModel.ViewModelLocator.Main;
        //        switch (e.Key)
        //        {
        //            case VirtualKey.N: vm.NewCommand.Execute(null); break;
        //            case VirtualKey.O: vm.OpenCommand.Execute(null); break;
        //            case VirtualKey.S: vm.SaveCommand.Execute(null); break;
        //        }
        //    }
        //}
    }
}
