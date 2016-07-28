namespace Shopping.DemoApp.UWP.UserControls
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices.WindowsRuntime;
    using System.Threading.Tasks;
    using Windows.Foundation;
    using Windows.Foundation.Collections;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Controls.Primitives;
    using Windows.UI.Xaml.Data;
    using Windows.UI.Xaml.Input;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Media.Animation;
    using Windows.UI.Xaml.Navigation;

    public sealed partial class HappinessUserControl : UserControl
    {
        public float Happiness
        {
            get { return (float)this.GetValue(HappinessProperty); }
            set
            {
                this.SetValue(HappinessProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for Happiness.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HappinessProperty =
            DependencyProperty.Register("Happiness", typeof(float), typeof(HappinessUserControl), new PropertyMetadata(null, HappinesChanged));

        private static void HappinesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (HappinessUserControl)d;
            control.UpdateClipping((float)e.NewValue);
        }

        public string Text
        {
            get
            {
                return (string)GetValue(TextProperty);
            }
            set
            {
                SetValue(TextProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
DependencyProperty.Register("Text", typeof(string), typeof(HappinessUserControl), new PropertyMetadata(0));


        public HappinessUserControl()
        {
            this.InitializeComponent();
            this.Loaded += HappinessUserControl_Loaded;
        }

        public void Blink()
        {
            VisualStateManager.GoToState(this, NormalState.Name, true);
        }

        private void HappinessUserControl_Loaded(object sender, RoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, NormalState.Name, true);
            this.clipStackPanel.Width = 5;
        }

        private void UpdateClipping(float happinesValue)
        {            
            var relativeValue = (double) this.calificationGrid.ActualWidth * happinesValue;
            
            Debug.WriteLine($"Happiness: {happinesValue} Total Width: { relativeValue}");

            this.clipAnimation.To = relativeValue;

            VisualStateManager.GoToState(this, RatedState.Name, true);

            this.animationStoryBoard.Begin();
        }
    }
}
