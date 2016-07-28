
using Android.App;
using Android.Content;
using Gcm;

namespace Shopping.DemoApp.Droid.Services
{
    [BroadcastReceiver(Permission = Constants.PERMISSION_GCM_INTENTS)]
    [IntentFilter(new[] { Intent.ActionBootCompleted })]
    [IntentFilter(new[] { Constants.INTENT_FROM_GCM_MESSAGE }, Categories = new string[] { AppSettings.AndroidPackageId })]
    [IntentFilter(new[] { Constants.INTENT_FROM_GCM_REGISTRATION_CALLBACK }, Categories = new string[] { AppSettings.AndroidPackageId })]
    [IntentFilter(new[] { Constants.INTENT_FROM_GCM_LIBRARY_RETRY }, Categories = new string[] { AppSettings.AndroidPackageId })]
    public class GcmBroadcastReceiver : GcmBroadcastReceiverBase<AndroidPushNotificationsService>
    {
        //The SENDER_ID is your Google API Console App Project Number
        public static string[] SENDER_IDS = { AppSettings.AndroidProjectNumber };
    }
}