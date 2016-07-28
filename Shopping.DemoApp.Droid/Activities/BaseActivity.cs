using System;
using Android.OS;
using Android.Support.V7.App;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Com.Lilarcor.Cheeseknife;

namespace Shopping.DemoApp.Droid.Activities
{
    public abstract class BaseActivity : AppCompatActivity
    {
        [InjectView(Resource.Id.toolbar)]
        protected Toolbar toolbar;

        public bool IsActivityVisible;

        protected abstract int LayoutResource
        {
            get;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(LayoutResource);

            Cheeseknife.Inject(this);

            ConfigureToolbar();
        }

        protected override void OnResume()
        {
            base.OnResume();

            IsActivityVisible = true;
        }

        protected override void OnPause()
        {
            base.OnPause();

            IsActivityVisible = false;
        }

        private void ConfigureToolbar()
        {
            if (toolbar != null)
            {
                SetSupportActionBar(toolbar);
                SupportActionBar.SetDisplayShowTitleEnabled(false);
            }
        }
    }
}