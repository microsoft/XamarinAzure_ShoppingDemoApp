namespace Shopping.DemoApp
{
    public static class AppSettings
    {
        public const string ApiAddress = "ENTER_YOUR_MOBILE_APP_ADDRESS_HERE";
        public const string NotificationHubConnectionString = "ENTER_YOUR_NOTIFICATION_HUB_CONNECTION_STRING_HERE";
        public const string NotificationHubPath = "ENTER_YOUR_NOTIFICATION_HUB_PATH_HERE";
        public const string EmotionApiKey = "ENTER_YOUR_EMOTION_API_KEY_HERE";

#if __ANDROID__
        public const string AndroidProjectNumber = "ENTER_YOUR_ANDROID_PROJECT_NUMBER_HERE";
        public const string AndroidPackageId = "ENTER_YOUR_ANDROID_PACKAGE_ID_HERE";
#endif
    }
}
