using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Engine.Models;
using Engine.ViewModels;

namespace WPFUI
{
    /// <summary>
    /// Interaction logic for TradeScreen.xaml
    /// </summary>
    public partial class TradeScreen : Window
    {

        public GameSession Session => DataContext as GameSession;
        //In the context of WPF, DataContext is a concept used for data binding.
        //It’s the default source of binding,
        //so if you have a control (like a TextBox or Label) bound to a property,
        //it will look for that property in the DataContext
        public TradeScreen()
        {
            InitializeComponent();
        }

        private void OnClick_Sell(object sender, RoutedEventArgs e)
        {
            GroupedInventoryItem groupInventoryItem = ((FrameworkElement)sender).DataContext as GroupedInventoryItem;

            if (groupInventoryItem != null)
            {
                Session.CurrentPlayer.Gold += groupInventoryItem.Item.Price;
                Session.CurrentTrader.AddItemToInventory(groupInventoryItem.Item);
                Session.CurrentPlayer.RemoveItemFromInventory(groupInventoryItem.Item);
            }
        }
        private void OnClick_Buy(object sender, RoutedEventArgs e)
        {
            GroupedInventoryItem groupInventoryItem = ((FrameworkElement)sender).DataContext as GroupedInventoryItem;

            if (groupInventoryItem != null)
            {
                if (Session.CurrentPlayer.Gold >= groupInventoryItem.Item.Price)
                {
                    Session.CurrentPlayer.Gold -= groupInventoryItem.Item.Price;
                    Session.CurrentTrader.RemoveItemFromInventory(groupInventoryItem.Item);
                    Session.CurrentPlayer.AddItemToInventory(groupInventoryItem.Item);
                }
                else
                {
                    MessageBox.Show("You do not have enough gold");
                }
            }
        }
        private void OnClick_Close(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
