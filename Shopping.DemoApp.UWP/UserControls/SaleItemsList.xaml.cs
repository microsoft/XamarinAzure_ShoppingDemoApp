namespace Shopping.DemoApp.UWP.UserControls
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices.WindowsRuntime;
    using Shopping.DemoApp.Models;
    using Windows.Foundation;
    using Windows.Foundation.Collections;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Controls.Primitives;
    using Windows.UI.Xaml.Data;
    using Windows.UI.Xaml.Input;
    using Windows.UI.Xaml.Media;
    using Windows.UI.Xaml.Navigation;
    using System.Threading.Tasks;
    using PCLStorage;
    using Services;
    using Events;
    using System.Collections.ObjectModel;
    public sealed partial class SaleItemsList : UserControl
    {
        // Declare the delegate (if using non-generic pattern).
        public delegate void NavigateToSellArticleEventHandler(object sender, EventArgs e);

        // Declare the event.
        public event NavigateToSellArticleEventHandler NavigateToSellArticle;

        public delegate void ItemSelectedEventHandler(object sender, ItemSelectedEventArgs e);

        public event ItemSelectedEventHandler ItemSelected;

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable<SaleItem>), typeof(SaleItemsList), new PropertyMetadata(null, ItemsSourcePropertyChanged));

        public IEnumerable<SaleItem> ItemsSource
        {
            get { return (IEnumerable<SaleItem>)this.GetValue(ItemsSourceProperty); }
            set { this.SetValue(ItemsSourceProperty, value); }
        }

        private ObservableCollection<SaleItem> leftItems;
        private ObservableCollection<SaleItem> rightItems;

        public SaleItemsList()
        {
            this.InitializeComponent();
        }

        public void UpdateItem(string fileName)
        {
            if (!this.UpdateItemIn(this.leftItems, fileName))
            {
                this.UpdateItemIn(this.rightItems, fileName);
            }
        }

        private bool UpdateItemIn(ObservableCollection<SaleItem> items, string fileName)
        {
            bool updated = false;

            if (items.Any(i => i.Id == fileName))
            {
                var item = items.FirstOrDefault(i => i.Id == fileName);
                var index = items.IndexOf(item);

                items[index] = items[index];
                updated = true;
            }

            return updated;
        }

        public async void UpdateItems(IEnumerable<SaleItem> items)
        {
            this.leftItems = new ObservableCollection<SaleItem>();
            this.rightItems = new ObservableCollection<SaleItem>();

            bool goLeft = true;
            foreach (var item in items)
            {
                item.Name = item.Name.ToUpper();

                if (string.IsNullOrEmpty(item.ImageUrl))
                {
                    var fullPath = await FileHelper.GetLocalFilePathAsync(item.Id);

                    item.ImageUrl = fullPath;
                }
                if (goLeft)
                {
                    leftItems.Add(item);
                }
                else
                {
                    rightItems.Add(item);
                }

                goLeft = !goLeft;
            }

            this.leftList.ItemsSource = this.leftItems;
            this.rightList.ItemsSource = this.rightItems;

            this.saleButton.Visibility = Visibility.Visible;
        }

        private static void ItemsSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SaleItemsList control = d as SaleItemsList;

            control.UpdateItems(e.NewValue as IEnumerable<SaleItem>);
        }

        private void saleButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigateToSellArticle?.Invoke(this, new EventArgs());
        }

        private void list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var list = (ListView)sender;
            var saleItem = list.SelectedItem as SaleItem;

            this.ItemSelected?.Invoke(this, new ItemSelectedEventArgs() { Item = saleItem });

            list.SelectionChanged -= this.list_SelectionChanged;
            list.SelectedItem = null;
            list.SelectionChanged += this.list_SelectionChanged;
        }
    }
}
