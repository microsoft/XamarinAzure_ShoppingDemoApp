using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Shopping.DemoApp.Droid.Views
{
    public class StarView : LinearLayout
    {
        private StarViewClip starsClipView;

        private float percentage;
        public float Percentage
        {
            get
            {
                return percentage;
            }

            set
            {
                percentage = value;
                if (this.starsClipView != null)
                {
                    this.starsClipView.PercentageToClip = percentage;
                }
            }
        }

        public StarView(Activity context) : base(context)
        {
            var layoutInflater = LayoutInflater.From(this.Context);
            var view = layoutInflater.Inflate(Resource.Layout.starview, this, false);

            var starsClip = view.FindViewById<RelativeLayout>(Resource.Id.starsclip);
            this.starsClipView = new StarViewClip(context);
            starsClip.AddView(this.starsClipView);

            AddView(view);
        }
    }
}