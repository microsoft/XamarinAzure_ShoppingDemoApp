namespace Shopping.DemoApp.UWP
{
    using System;
    using System.Diagnostics;
    using Windows.ApplicationModel;
    using Windows.ApplicationModel.Activation;
    using Windows.Foundation.Metadata;
    using Windows.UI;
    using Windows.UI.Core;
    using Windows.UI.ViewManagement;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Navigation;

    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App : Application
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
            this.Suspending += this.OnSuspending;
        }

        public Frame RootFrame { get; set; }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif
            this.RootFrame = Window.Current.Content as Frame;

            this.SetStatusBarColor();

            this.SubscribeButtonsEvents();

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (this.RootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                this.RootFrame = new Frame();

                this.RootFrame.NavigationFailed += this.OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {   
                }

                // Place the frame in the current Window
                Window.Current.Content = this.RootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (this.RootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    this.RootFrame.Navigate(typeof(MainPage), e.Arguments);
                }

                // Ensure the current window is active
                Window.Current.Activate();
            }
        }

        private void SubscribeButtonsEvents()
        {
            Windows.UI.Core.SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            Windows.UI.Core.SystemNavigationManager.GetForCurrentView().BackRequested += (s, a) =>
            {
                Debug.WriteLine("BackRequested");
                if (this.RootFrame.CanGoBack)
                {
                    this.RootFrame.GoBack();
                    a.Handled = true;
                }
            };

            /*Windows.Phone.UI.Input.HardwareButtons.BackPressed += (sender, e) =>
            {
                if (this.RootFrame.CanGoBack)
                {
                    e.Handled = true;
                    this.RootFrame.GoBack();
                }
            };*/
        }

        private void SetStatusBarColor()
        {
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                var statusBar = StatusBar.GetForCurrentView();
                if (statusBar != null)
                {
                    statusBar.BackgroundOpacity = 1;
                    statusBar.BackgroundColor = Color.FromArgb(255, 1, 31, 79);
                    statusBar.ForegroundColor = Colors.White;
                }
            }
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
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
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            
            deferral.Complete();
        }
    }
}
