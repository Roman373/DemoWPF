using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Логика взаимодействия для AddEditPage.xaml
    /// </summary>
    public partial class AddEditPage : Page
    {
        private Entities.Service _currentService = null;
        private byte[] _mainImageData;
        public AddEditPage()
        {
            InitializeComponent();
        }
        public AddEditPage(Entities.Service service)
        {
            InitializeComponent();
            _currentService = service;
            Title = "Редактирование услуги";
            TBoxTitle.Text = _currentService.Title;
            TBoxCost.Text = _currentService.Cost.ToString("N2");
            TBoxDurationInSecond.Text = (_currentService.DurationInSeconds / 60).ToString();
            TBoxDescription.Text = _currentService.Description;
            if (_currentService.Discount > 0)
                TBoxDiscount.Text = (_currentService.Discount.Value * 100).ToString();
            if (_currentService.MainImagePath != null)
                ImageService.Source = new ImageSourceConverter().ConvertFrom(_currentService.MainImagePath)as ImageSource;
        }
        public string CheckError()
        {
            var errorBuilder = new StringBuilder();
            if (string.IsNullOrWhiteSpace(TBoxTitle.Text))
                errorBuilder.AppendLine("Название услуги обязательно для заполнения;");
            var serviceFromDB = App.Context.Service.ToList()
                .FirstOrDefault(predicate => predicate.Title.ToLower() == TBoxTitle.Text.ToLower());
            if (serviceFromDB != null && serviceFromDB !=_currentService)
                errorBuilder.AppendLine("Такая услуга уже существует;");
            decimal cost = 0;
            if (decimal.TryParse(TBoxCost.Text, out cost) == false || cost <= 0)
                errorBuilder.AppendLine("Стоимость услуги должна быть положиетльным числом;");
            int duratuinInMinutes = 0;
            if(int.TryParse(TBoxDurationInSecond.Text,out duratuinInMinutes) == false 
                || duratuinInMinutes > 240 || duratuinInMinutes <= 0)
            {
                errorBuilder.AppendLine("Дительность оказания услуги должна быть положительным " +
                    "числом (не больше, чем 4 часа);");
            }
            if (!string.IsNullOrEmpty(TBoxDiscount.Text))
            {
                int discount = 0;
                if(int.TryParse(TBoxDiscount.Text,out discount) ==false
                    || discount < 0 || discount > 100)
                {
                    errorBuilder.AppendLine("Размер скидки - целое число в диапазоне от 0 до 100%;");
                }
            }
            if (errorBuilder.Length > 0)
                errorBuilder.Insert(0, "Устраните следующие ошибки:\n");
            return errorBuilder.ToString();

        }
        private void BtnSelectedImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Image |*.png;*.jpg;*.jpeg";
            if (ofd.ShowDialog() == true)
            {
                _mainImageData = File.ReadAllBytes(ofd.FileName);
                ImageService.Source = (ImageSource)new ImageSourceConverter().ConvertFrom(_mainImageData);
            }
                
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            var errorMessage = CheckError();
            if (errorMessage.Length > 0)
            {
                MessageBox.Show(errorMessage, "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                if(_currentService == null)
                {
                    var service = new Entities.Service
                    {
                        Title = TBoxTitle.Text,
                        Cost = decimal.Parse(TBoxCost.Text),
                        DurationInSeconds = int.Parse(TBoxDurationInSecond.Text),
                        Description = TBoxDescription.Text,
                        Discount = string.IsNullOrWhiteSpace(TBoxDiscount.Text) ? 0 : double.Parse(TBoxDiscount.Text) / 100,
                        MainImagePath = _mainImageData.ToString()
                    };
                    App.Context.Service.Add(service);
                    App.Context.SaveChanges();
                }
                else
                {
                    _currentService.Title = TBoxTitle.Text;
                    _currentService.Cost = decimal.Parse(TBoxCost.Text);
                    _currentService.DurationInSeconds = int.Parse(TBoxDurationInSecond.Text)*60;
                    _currentService.Description = TBoxDescription.Text;
                    _currentService.Discount =string.IsNullOrWhiteSpace(TBoxDiscount.Text) ? 0 : double.Parse(TBoxDiscount.Text) /100;
                    if (_mainImageData != null)
                        _currentService.MainImagePath = _mainImageData.ToString();
                    App.Context.SaveChanges();
                }
                
                NavigationService.GoBack();
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
        }
    }
}
