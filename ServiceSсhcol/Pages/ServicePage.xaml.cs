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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ServiceSсhcol.Pages
{
    /// <summary>
    /// Логика взаимодействия для ServicePage.xaml
    /// </summary>
    public partial class ServicePage : Page
    {
        public ServicePage()
        {
            InitializeComponent();
            if (App.CurrentUser.RoleId == 1)
            {
                BtnAddService.Visibility = Visibility.Visible;
            }
            else
            {
                BtnAddService.Visibility = Visibility.Collapsed;
            }
            CBoxDiscount.SelectedIndex = 0;
            CBoxSort.SelectedItem = 0;
            UpdateService();
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
        }
        private void UpdateService()
        {
            var service = App.Context.Service.ToList();
            if (CBoxSort.SelectedIndex == 0)
                service = service.OrderBy(p => p.CostWithDiscount).ToList();
            else
                service = service.OrderByDescending(p => p.CostWithDiscount).ToList();

            if (CBoxDiscount.SelectedIndex == 1)
                service = service.Where(p => p.Discount >= 0 && p.Discount < 0.05).ToList();
            if (CBoxDiscount.SelectedIndex == 2)
                service = service.Where(p => p.Discount >= 0.05 && p.Discount < 0.15).ToList();
            if (CBoxDiscount.SelectedIndex == 3)
                service = service.Where(p => p.Discount >= 0.15 && p.Discount < 0.30).ToList();
            if (CBoxDiscount.SelectedIndex == 4)
                service = service.Where(p => p.Discount >= 0.30 && p.Discount < 0.70).ToList();
            if (CBoxDiscount.SelectedIndex == 5)
                service = service.Where(p => p.Discount >= 0.70 && p.Discount <= 1).ToList();
            service = service.Where(p => p.Title.ToLower().Contains(TBoxSearch.Text.ToLower())).ToList();
            LViewServices.ItemsSource = service;
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var currentService = (sender as Button).DataContext as Entities.Service;
            NavigationService.Navigate(new AddEditPage(currentService));
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var currentservice = (sender as Button).DataContext as Entities.Service;
            if (MessageBox.Show($"вы уверены, что хотите удалить услугу: " +
                $"{currentservice.Title}?","Внимание",
                MessageBoxButton.YesNo,MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                App.Context.Service.Remove(currentservice);
                App.Context.SaveChanges();
                UpdateService();
            }
        }

        private void BtnAddService_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddEditPage());
        }

        private void CBoxDiscount_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateService();
        }

        private void CBoxSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateService();
        }

        private void TBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateService();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateService();
        }
    }
}
