using semenova_library;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace Semenova_ind
{
    public partial class AddSaleWindow : Window
    {
        private readonly IMainApp _mainApp;
        private readonly Partners _partner;

        public AddSaleWindow(IMainApp mainApp, Partners partner)
        {
            InitializeComponent();

            _mainApp = mainApp;
            _partner = partner;

            PartnerNameTextBox.Text = partner.Name;
            SaleDatePicker.SelectedDate = DateTime.Today;

            LoadProducts();
        }

        private void LoadProducts()
        {
            try
            {
                var products = _mainApp.GetAllProducts().ToList();
                ProductComboBox.ItemsSource = products;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки продукции: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ValidateInputs()
        {
            if (ProductComboBox.SelectedItem == null && string.IsNullOrWhiteSpace(ProductComboBox.Text))
            {
                MessageBox.Show("Выберите или введите наименование продукции", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                ProductComboBox.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(QuantityTextBox.Text))
            {
                MessageBox.Show("Введите количество", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                QuantityTextBox.Focus();
                return false;
            }

            if (!int.TryParse(QuantityTextBox.Text, out int quantity) || quantity <= 0)
            {
                MessageBox.Show("Количество должно быть положительным числом", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                QuantityTextBox.Focus();
                return false;
            }

            if (SaleDatePicker.SelectedDate == null)
            {
                MessageBox.Show("Выберите дату продажи", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!ValidateInputs())
                    return;

                Product product = null;

                if (ProductComboBox.SelectedItem != null)
                {
                    product = (Product)ProductComboBox.SelectedItem;
                }
                else
                {
                    string productName = ProductComboBox.Text.Trim();

                    product = _mainApp.FindProductByName(productName);

                    if (product == null)
                    {
                        product = new Product
                        {
                            Name = productName,
                            Article = GenerateArticle(productName),
                            Price = 0,
                            Unit = "шт"
                        };

                        _mainApp.AddProduct(product);
                    }
                }

                var sale = new SalesHistory
                {
                    PartnerId = _partner.Id,
                    ProductId = product.Id,
                    Quantity = int.Parse(QuantityTextBox.Text),
                    SaleDate = SaleDatePicker.SelectedDate.Value
                };

                _mainApp.AddSalesRecord(sale);

                MessageBox.Show("Продажа успешно добавлена!", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении продажи: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string GenerateArticle(string productName)
        {

            string prefix = "";
            string[] words = productName.Split(' ');
            foreach (string word in words)
            {
                if (word.Length > 0)
                    prefix += word[0].ToString().ToUpper();
            }

            Random rand = new Random();
            return $"{prefix}{rand.Next(100, 999)}";
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void QuantityTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, @"^\d+$");
        }
    }
}