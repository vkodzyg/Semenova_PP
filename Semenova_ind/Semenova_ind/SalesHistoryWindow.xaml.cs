using semenova_library;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Semenova_ind
{
    public partial class SalesHistoryWindow : Window, INotifyPropertyChanged
    {
        private readonly IMainApp _mainApp;
        private readonly int _partnerId;
        private List<SalesHistory> _salesHistory;
        private string _partnerName;
        private string _statusMessage;
        private IMainApp mainApp;

        public event PropertyChangedEventHandler PropertyChanged;

        public SalesHistoryWindow(IMainApp mainApp, int partnerId, string partnerName)
        {
            InitializeComponent();

            _mainApp = mainApp;
            _partnerId = partnerId;
            _partnerName = partnerName;

            DataContext = this;
            Title = $"Семенова - История продаж: {partnerName}";

            LoadData();
        }

        public SalesHistoryWindow(IMainApp mainApp)
        {
            this.mainApp = mainApp;
        }

        public List<SalesHistory> SalesHistory
        {
            get => _salesHistory;
            set
            {
                _salesHistory = value;
                OnPropertyChanged(nameof(SalesHistory));
            }
        }

        public string PartnerName
        {
            get => _partnerName;
            set
            {
                _partnerName = value;
                OnPropertyChanged(nameof(PartnerName));
            }
        }

        public int TotalSalesCount => SalesHistory?.Count ?? 0;

        public decimal TotalSalesSum
        {
            get
            {
                if (SalesHistory == null) return 0;
                decimal total = 0;
                foreach (var sale in SalesHistory)
                {
                    if (sale.Product != null)
                    {
                        total += sale.Quantity * sale.Product.Price;
                    }
                }
                return total;
            }
        }

        public int CurrentDiscount => _mainApp?.GetPartnerCurrentDiscount(_partnerId) ?? 0;

        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                OnPropertyChanged(nameof(StatusMessage));
            }
        }

        private void LoadData()
        {
            try
            {
                Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;

                _salesHistory = _mainApp.GetPartnerSalesHistory(_partnerId).ToList();

                SalesListView.ItemsSource = _salesHistory;

                UpdateDisplayInfo();

                StatusMessage = $"Загружено записей: {_salesHistory.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки истории продаж: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                StatusMessage = "Ошибка загрузки данных";
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        private void UpdateDisplayInfo()
        {
            PartnerNameTextBlock.Text = $"Партнер: {_partnerName}";
            DiscountTextBlock.Text = $"Текущая скидка: {CurrentDiscount}%";
            TotalSalesTextBlock.Text = $"Всего продаж: {TotalSalesCount} на сумму {TotalSalesSum:N2} ₽";
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBox.Show(
                    "Функция экспорта будет реализована позже",
                    "Информация",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при экспорте: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}