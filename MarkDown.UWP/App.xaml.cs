using Cimbalino.Toolkit.Controls;
using MarkDown.UWP.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
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

namespace MarkDown.UWP
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            Microsoft.ApplicationInsights.WindowsAppInitializer.InitializeAsync(
                Microsoft.ApplicationInsights.WindowsCollectors.Metadata |
                Microsoft.ApplicationInsights.WindowsCollectors.Session);
            this.InitializeComponent();
            this.Resuming += OnResuming;
            this.Suspending += OnSuspending;
            this.UnhandledException += OnUnhandledException;
        }

        public static Windows.UI.Core.CoreDispatcher Dispatcher;

        private async void OnUnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            await new MessageDialog("Application Unhandled Exception:\r\n" + e.Exception.Message, "Error :(")
                .ShowAsync();
        }

        /// <summary>
        /// Should be called from OnActivated and OnLaunched
        /// </summary>
        private void RegisterExceptionHandlingSynchronizationContext()
        {
            ExceptionHandlingSynchronizationContext
                .Register()
                .UnhandledException += SynchronizationContext_UnhandledException;
        }

        private async void SynchronizationContext_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            await new MessageDialog("Synchronization Context Unhandled Exception:\r\n" + e.Exception.Message, "Error :(")
                .ShowAsync();
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
            RegisterExceptionHandlingSynchronizationContext();

#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif

            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page

                //rootFrame = new Frame();
                rootFrame = new HamburgerFrame()
                {
                    Header = new HamburgerTitleBar(),
                    Pane = new HamburgerPaneControl() { DataContext = ViewModelLocator.Main},
                    OpenPaneLength = 200,
                    IsPaneOpen = false
                };

                Binding binding = new Binding() { Source = ViewModelLocator.Main, Path = new PropertyPath("Title") };
                BindingOperations.SetBinding(((HamburgerTitleBar)(((HamburgerFrame)rootFrame).Header)), HamburgerTitleBar.TitleProperty, binding);


                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                rootFrame.Navigate(typeof(MainPage), e.Arguments);
                await ViewModelLocator.Main.Restore();
            }
            // Ensure the current window is active
            Window.Current.Activate();
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity

            await ViewModelLocator.Main.BackUp();

            deferral.Complete();
        }

        private async void OnResuming(object sender, object e)
        {
            await ViewModelLocator.Main.Restore();
        }

        protected override async void OnFileActivated(FileActivatedEventArgs args)
        {
            base.OnFileActivated(args);
            RegisterExceptionHandlingSynchronizationContext();

            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page

                //rootFrame = new Frame();
                rootFrame = new HamburgerFrame()
                {
                    Header = new HamburgerTitleBar(),
                    Pane = new HamburgerPaneControl() { DataContext = ViewModelLocator.Main },
                    OpenPaneLength = 200,
                    IsPaneOpen = false
                };

                Binding binding = new Binding() { Source = ViewModelLocator.Main, Path = new PropertyPath("Title") };
                BindingOperations.SetBinding(((HamburgerTitleBar)(((HamburgerFrame)rootFrame).Header)), HamburgerTitleBar.TitleProperty, binding);


                rootFrame.NavigationFailed += OnNavigationFailed;

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;

            }
            if (rootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                rootFrame.Navigate(typeof(MainPage));
                await ViewModelLocator.Main.Restore();
            }

            Window.Current.Activate();
            await ViewModelLocator.Main.OpenDoc((StorageFile)args.Files[0]);
        }
    }
}
