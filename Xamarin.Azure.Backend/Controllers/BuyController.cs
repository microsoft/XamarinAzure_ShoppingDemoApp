using Microsoft.Azure.Mobile.Server.Config;
using System;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Http.Controllers;
using Xamarin.Azure.Backend.DataObjects;

namespace Xamarin.Azure.Backend.Controllers
{
    [MobileAppController]
    public class BuyController : ApiController
    {
        private const int NotificationDelaySeconds = 15;

        private NotificationService notificationService;

        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);

            notificationService = new NotificationService();
            notificationService.Initialize(this.Configuration);
        }

        [Route("api/buy")]
        public bool Post(SaleItem item)
        {
            // We are running notification code in a QueueBackgroundWorkItem 
            // for demo purpouses only (simulate delayed notification for phone devices). 
            // In a normal environment, code should run inside method's body

            HostingEnvironment.QueueBackgroundWorkItem(async ct =>
            {
                await Task.Delay(TimeSpan.FromSeconds(NotificationDelaySeconds));

                string message = $"Someone wants to buy one of your items! {item.Name}";

                var appleNotificationPayload = "{\"aps\":{\"alert\":\"" + message + "\"}}";
                var gcmNotificationPayload = "{\"data\":{\"msg\":\"" + message + "\"}}";
                var uwpNotificationPayload = @"<toast><visual><binding template=""ToastText01""><text id=""1"">"
                        + message + @"</text></binding></visual></toast>";

                try
                {
                    var resultApple = await notificationService.Hub.SendAppleNativeNotificationAsync(appleNotificationPayload);
                    Configuration.Services.GetTraceWriter().Info(resultApple.State.ToString());

                    var resultGcm = await notificationService.Hub.SendGcmNativeNotificationAsync(gcmNotificationPayload);
                    Configuration.Services.GetTraceWriter().Info(resultGcm.State.ToString());

                    var resultUWP = await notificationService.Hub.SendWindowsNativeNotificationAsync(uwpNotificationPayload);
                    Configuration.Services.GetTraceWriter().Info(resultUWP.State.ToString());
                }
                catch (Exception ex)
                {
                    Configuration.Services.GetTraceWriter().Error(ex.Message, null, "Push.SendAsync Error");
                }
            });

            return true;
        }
    }
}