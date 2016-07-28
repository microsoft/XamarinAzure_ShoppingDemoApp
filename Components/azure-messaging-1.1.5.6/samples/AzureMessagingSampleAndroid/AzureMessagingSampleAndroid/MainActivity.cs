using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace AzureMessagingSampleAndroid
{
	[Activity (Label = "AzureMessagingSampleAndroid", MainLauncher = true)]
	public class MainActivity : Activity
	{
		TextView textViewMsg;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			textViewMsg = FindViewById<TextView> (Resource.Id.textViewMsg);

			#region Check to ensure user has used their own GCM Service, Hub Name, and Hub Secret
			if (SampleGcmBroadcastReceiver.SENDER_IDS.Length >= 1
					&& SampleGcmBroadcastReceiver.SENDER_IDS [0] == "YOUR-SENDER-ID") {
				textViewMsg.Text = "You must change the SampleGcmBroadcastReceiver.SENDER_IDS to your own Sender ID for the sample to work!";
				return;
			}

			if (SampleGcmBroadcastReceiver.HUB_NAME == "YOUR-HUB-NAME") {
				textViewMsg.Text = "You must enter your own Hub Name and Hub Listen Secret for the sample to work!";
				return;
			}
			#endregion

			textViewMsg.Text = "Registering for Remote Notifications...";

			// Initialize our Gcm Service Hub
			SampleGcmService.Initialize (this);

			// Register for GCM
			SampleGcmService.Register (this);
		}
	}
}


