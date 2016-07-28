using Microsoft.Azure.Mobile.Server;
using Microsoft.Azure.Mobile.Server.Config;
using Microsoft.Azure.NotificationHubs;
using System.Web.Http;

namespace Xamarin.Azure.Backend
{
    public class NotificationService
    {
        public NotificationHubClient Hub { get; private set; }

        public void Initialize(HttpConfiguration configuration)
        {
            MobileAppSettingsDictionary settings =
                configuration.GetMobileAppSettingsProvider().GetMobileAppSettings();

            string notificationHubName = settings.NotificationHubName;
            string notificationHubConnection = 
                settings.Connections[MobileAppSettingsKeys.NotificationHubConnectionString].ConnectionString;

            Hub = NotificationHubClient.CreateClientFromConnectionString(notificationHubConnection, notificationHubName);
        }
    }
}