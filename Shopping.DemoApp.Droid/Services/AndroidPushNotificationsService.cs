using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Gcm;
using Shopping.DemoApp.Droid.Activities;
using Plugin.CurrentActivity;

namespace Shopping.DemoApp.Droid.Services
{
    [Service]
    public class AndroidPushNotificationsService : GcmServiceBase
    {
        private Handler mainThreadHandler;

        public AndroidPushNotificationsService()
            : base(GcmBroadcastReceiver.SENDER_IDS)
        {
            mainThreadHandler = new Handler();
        }

        protected override void OnRegistered(Context context, string registrationId)
        {
            AndroidPushService.SetRegisterRegistrationId(registrationId);
        }

        protected override void OnUnRegistered(Context context, string registrationId)
        {
            AndroidPushService.SetUnRegisterRegistrationId(registrationId);
        }

        protected override void OnError(Context context, string errorId)
        {
            AndroidPushService.SetRegisterRegistrationId(string.Empty);
            AndroidPushService.SetUnRegisterRegistrationId(string.Empty);
        }

        protected override bool OnRecoverableError(Context context, string errorId)
        {
            AndroidPushService.SetRegisterRegistrationId(string.Empty);
            AndroidPushService.SetUnRegisterRegistrationId(string.Empty);

            return false;
        }

        protected override void OnMessage(Context context, Intent intent)
        {
            if (intent != null || intent.Extras != null)
            {
                var message = intent.Extras.GetString("msg");
                if (message != null)
                {
                    mainThreadHandler.Post(() =>
                    {
                        var baseActivity = CrossCurrentActivity.Current.Activity as BaseActivity;
                        if (baseActivity != null && baseActivity.IsActivityVisible)
                        {
                            AndroidPushService.ShowInAppToast(context, message);
                        }
                        else
                        {
                            AndroidPushService.ShowLocalPush(context, message);
                        }
                    });
                }
            }
        }
    }
}