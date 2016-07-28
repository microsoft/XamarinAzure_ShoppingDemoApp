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
    public class StarViewClip : LinearLayout
    {
        private float percentageToClip;
        public float PercentageToClip
        {
            get
            {
                return this.percentageToClip;
            }
            set
            {
                this.percentageToClip = value;
                this.Invalidate();
            }
        }

        public StarViewClip(Activity context) : base(context)
        {
            SetWillNotDraw(false);

            var layoutInflater = LayoutInflater.From(this.Context);
            var view = layoutInflater.Inflate(Resource.Layout.starclip, this, false);

            AddView(view);
        }

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);

            var width = canvas.Width;
            var height = canvas.Height;

            var offsetX = percentageToClip * width;
            canvas.ClipRect(0, 0, offsetX, height);
        }
    }
}