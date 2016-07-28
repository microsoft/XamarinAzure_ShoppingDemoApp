namespace Shopping.DemoApp
{
    using Acr.UserDialogs;
    using Microsoft.WindowsAzure.MobileServices;
    using Services;
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;

    public class AuthenticationService
    {
        private static AuthenticationService instance = new AuthenticationService();

        public bool UserIsAuthenticated { get; private set; }

        public static AuthenticationService Instance
        {
            get
            {
                return instance;
            }
        }

        private AuthenticationService()
        {
        }

        public async Task<bool> RequestLoginIfNecessary()
        {
            if (!UserIsAuthenticated)
            {
                string result = await UserDialogs.Instance.ActionSheetAsync("Want to sell something? Please, sign-in first", "Cancel", null, buttons: new[] { "Facebook", "Twitter" });

                try
                {
                    MobileServiceUser mobileUser = null;
                    switch (result)
                    {
                        case "Facebook":
                            mobileUser = await LoginWithProviderAsync(MobileServiceAuthenticationProvider.Facebook);
                            break;
                        case "Twitter":
                            mobileUser = await LoginWithProviderAsync(MobileServiceAuthenticationProvider.Twitter);
                            break;
                    }

                    UserIsAuthenticated = mobileUser != null;
                }
                catch (Exception ex)
                {                    
                    Debug.WriteLine(ex);
                }
            }

            return UserIsAuthenticated;
        }

#if __IOS__

        private Task<MobileServiceUser> LoginWithProviderAsync(MobileServiceAuthenticationProvider provider)
        {
            var window = UIKit.UIApplication.SharedApplication.KeyWindow;
            var vc = window.RootViewController;

            // take top presented view controller
            while (vc.PresentedViewController != null)
            {
                vc = vc.PresentedViewController;
            }

            return SaleItemDataService.Instance.MobileService.LoginAsync(vc, provider);
        }

#elif __ANDROID__

        private Task<MobileServiceUser> LoginWithProviderAsync(MobileServiceAuthenticationProvider provider)
        {
            return SaleItemDataService.Instance.MobileService.LoginAsync(Plugin.CurrentActivity.CrossCurrentActivity.Current.Activity, provider);
        }

#elif WINDOWS_UWP

        //// In UWP we need to be logged in to subscribe to push notifications
        private async Task<MobileServiceUser> LoginWithProviderAsync(MobileServiceAuthenticationProvider provider)
        {
            var serviceUser = await SaleItemDataService.Instance.MobileService.LoginAsync(provider);

            return serviceUser;
        }
#endif

    }
}