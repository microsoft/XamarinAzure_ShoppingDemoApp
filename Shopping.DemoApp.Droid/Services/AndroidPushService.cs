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
using System.Threading.Tasks;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using WindowsAzure.Messaging;
using Shopping.DemoApp.Droid.Activities;
using Gcm;

namespace Shopping.DemoApp.Droid.Services
{
    public static class AndroidPushService
    {
        private const int VibrateTimeInMillis = 300;
        private const string PushTitle = "Shopping Demo App";

        private static TaskCompletionSource<string> registrationTaskCompletionSource;
        private static TaskCompletionSource<string> unRegistrationTaskCompletionSource;

        private static Context initContext;
        private static bool gmsAvailabilable;
        private static NotificationHub hub;

        public static void Initialize(Context context)
        {
            initContext = context;

            gmsAvailabilable = CheckAvailability();

            //hub = new NotificationHub(AppSettings.NotificationHubPath, AppSettings.NotificationHubConnectionString, context);
        }

        public static async Task Register(string[] tags)
        {
            if (gmsAvailabilable)
            {
                var registrationId = await RegisterInternal();

                try
                {
                    await Task.Run(() => hub.Register(registrationId, tags));
                    Console.WriteLine($"Registered to channel with registrationId {registrationId}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        public static async Task UnRegister(string[] tags)
        {
            if (gmsAvailabilable)
            {
                var registrationId = await UnRegisterInternal();

                try
                {
                    await Task.Run(() => hub.UnregisterAll(registrationId));
                    Console.WriteLine($"UnRegistered to channel with registrationId {registrationId}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        public static void SetRegisterRegistrationId(string rRegistrationId)
        {
            registrationTaskCompletionSource?.TrySetResult(rRegistrationId);
            Console.WriteLine($"Current rRegistrationId {rRegistrationId}");
        }

        public static void SetUnRegisterRegistrationId(string unrRegistrationId)
        {
            unRegistrationTaskCompletionSource?.TrySetResult(unrRegistrationId);
            Console.WriteLine($"Current unrRegistrationId {unrRegistrationId}");
        }

        public static void ShowInAppToast(Context context, string msg)
        {
            Console.WriteLine($"Toast received: {msg}");
            var toastMsg = msg;
            if (!string.IsNullOrEmpty(toastMsg))
            {
                Toast.MakeText(context, toastMsg, ToastLength.Long).Show();

                var vibrator = (Vibrator)context.GetSystemService(Context.VibratorService);
                vibrator.Vibrate(VibrateTimeInMillis);
            }
        }

        public static void ShowLocalPush(Context context, string msg)
        {
            Console.WriteLine($"Toast received: {msg}");
            var pushMsg = msg;
            if (!string.IsNullOrEmpty(pushMsg))
            {
                var pendingIntent = GeneratePendingIntent(context, msg);

                var currentMillis = Java.Lang.JavaSystem.CurrentTimeMillis();

                var builder = new NotificationCompat.Builder(context)
                .SetContentIntent(pendingIntent)
                .SetContentTitle(PushTitle)
                .SetContentText(pushMsg)
                .SetAutoCancel(true)
                .SetStyle(new NotificationCompat.BigTextStyle().BigText(pushMsg))
                .SetDefaults((int)(NotificationDefaults.Sound | NotificationDefaults.Vibrate))
                .SetWhen(currentMillis)
                .SetSmallIcon(GetNotificationIcon());

                builder = SetNotificationColor(context, builder);

                var notification = builder.Build();

                var notificationManager = (NotificationManager)context.GetSystemService(Context.NotificationService);
                notificationManager.Notify((int)currentMillis, notification);
            }
        }

        private static PendingIntent GeneratePendingIntent(Context context, string msg)
        {
            var notificationIntent = new Intent(context, typeof(MainActivity));
            notificationIntent.SetFlags(ActivityFlags.SingleTop);

            var pendingIntent = PendingIntent.GetActivity(context, 0, notificationIntent, PendingIntentFlags.UpdateCurrent);
            return pendingIntent;
        }

        private static Task<string> RegisterInternal()
        {
            registrationTaskCompletionSource = new TaskCompletionSource<string>();

            try
            {
                GcmClient.Register(initContext, GcmBroadcastReceiver.SENDER_IDS);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                registrationTaskCompletionSource.TrySetResult(string.Empty);
            }

            return registrationTaskCompletionSource.Task;
        }

        private static Task<string> UnRegisterInternal()
        {
            unRegistrationTaskCompletionSource = new TaskCompletionSource<string>();

            try
            {
                GcmClient.UnRegister(initContext);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                unRegistrationTaskCompletionSource.TrySetResult(string.Empty);
            }

            return unRegistrationTaskCompletionSource.Task;
        }

        private static bool CheckAvailability()
        {
            try
            {
                GcmClient.CheckDevice(initContext);
                GcmClient.CheckManifest(initContext);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        private static NotificationCompat.Builder SetNotificationColor(Context context, NotificationCompat.Builder builder)
        {
            if (global::Android.OS.Build.VERSION.SdkInt >= global::Android.OS.BuildVersionCodes.Lollipop)
            {
                var blueDocks = ContextCompat.GetColor(context, Resource.Color.blue);

                builder = builder.SetColor(blueDocks);
            }

            return builder;
        }

        private static int GetNotificationIcon()
        {
            var useWhiteIcon = global::Android.OS.Build.VERSION.SdkInt >= global::Android.OS.BuildVersionCodes.Lollipop;
            return useWhiteIcon ? Resource.Drawable.Icon : Resource.Drawable.Icon;
        }
    }
}
