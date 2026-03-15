using semenova_library;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Semenova_ind
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private readonly IMainApp _mainApp;
        private List<Partners> _partners;
        private List<SalesHistory> _currentOrders;
        private string _searchTerm;
        private string _currentSort = "По названию";

        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindow()
        {
            InitializeComponent();

            try
            {
                _mainApp = new MainApp();
                DataContext = this;

                _mainApp.PartnersChanged += OnPartnersChanged;
                _mainApp.DataLoaded += OnDataLoaded;
                _mainApp.ErrorOccurred += OnErrorOccurred;

                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при инициализации: {ex.Message}",
                    "Критическая ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public string SearchTerm
        {
            get => _searchTerm;
            set
            {
                _searchTerm = value;
                OnPropertyChanged();
                FilterPartners();
            }
        }

        public List<Partners> Partners
        {
            get => _partners;
            set
            {
                _partners = value;
                OnPropertyChanged();
            }
        }

        public string StatusMessage
        {
            get => _mainApp?.StatusMessage;
            set
            {
                if (_mainApp != null)
                {
                    _mainApp.StatusMessage = value;
                    OnPropertyChanged();
                }
            }
        }

        private void LoadData()
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;
                _mainApp.LoadData();

                UpdatePartnersList();

                StatusMessage = "Данные успешно загружены";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                StatusMessage = "Ошибка загрузки данных";
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        private void UpdatePartnersList()
        {
            try
            {
                _partners = _mainApp.GetFilteredPartners().ToList();
                PartnersListBox.ItemsSource = _partners;
                ApplySorting();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении списка партнеров: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplySorting()
        {
            if (_partners == null) return;

            switch (_currentSort)
            {
                case "По названию":
                    _partners = _partners.OrderBy(p => p.Name).ToList();
                    break;
                case "По рейтингу":
                    _partners = _partners.OrderByDescending(p => p.Rating).ToList();
                    break;
                case "По продажам":
                    var sorted = _mainApp.GetPartnersSortedBySales().ToList();
                    _partners = _partners.OrderBy(p => sorted.FindIndex(sp => sp.Id == p.Id)).ToList();
                    break;
            }

            PartnersListBox.ItemsSource = _partners;
        }

        private void FilterPartners()
        {
            _mainApp.SearchTerm = SearchTerm;
            UpdatePartnersList();
        }

        private void PartnersListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (PartnersListBox.SelectedItem is Partners selectedPartner)
                {
                    LoadPartnerOrders(selectedPartner.Id);
                    UpdatePartnerInfo(selectedPartner);
                    AddOrderButton.IsEnabled = true;
                    PartnerInfoPanel.Visibility = Visibility.Visible;

                    OrdersHeaderTextBlock.Text = $"Заказы партнера: {selectedPartner.Name}";
                }
                else
                {
                    OrdersListView.ItemsSource = null;
                    AddOrderButton.IsEnabled = false;
                    PartnerInfoPanel.Visibility = Visibility.Collapsed;
                    OrdersHeaderTextBlock.Text = "Список заказов выбранного партнера";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при выборе партнера: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadPartnerOrders(int partnerId)
        {
            try
            {
                _currentOrders = _mainApp.GetPartnerSalesHistory(partnerId).ToList();
                OrdersListView.ItemsSource = _currentOrders;

                decimal total = 0;
                foreach (var sale in _currentOrders)
                {
                    if (sale.Product != null)
                    {
                        total += sale.Quantity * sale.Product.Price;
                    }
                }
                TotalSalesTextBlock.Text = $"{total:N2} ₽";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки заказов: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdatePartnerInfo(Partners partner)
        {
            DirectorTextBlock.Text = partner.DirectorFullname ?? "-";
            PhoneTextBlock.Text = partner.Phone ?? "-";
            EmailTextBlock.Text = partner.Email ?? "-";
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SearchTerm = SearchTextBox.Text;
        }

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void AddMenuItem_Click(object sender, RoutedEventArgs e)
        {
            AddPartner();
        }

        private void EditMenuItem_Click(object sender, RoutedEventArgs e)
        {
            EditSelectedPartner();
        }

        private void DeleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            DeleteSelectedPartner();
        }

        private void SayMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SaySAlesHistory();
        }
        private void SaySAlesHistory()
        {
            var salhisWindow = new SalesHistoryWindow(_mainApp);
            salhisWindow.ShowDialog();
        }

        private void AddOrderButton_Click(object sender, RoutedEventArgs e)
        {
            if (PartnersListBox.SelectedItem is Partners selectedPartner)
            {
                var addSaleWindow = new AddSaleWindow(_mainApp, selectedPartner);
                addSaleWindow.Owner = this;

                if (addSaleWindow.ShowDialog() == true)
                {
                  
                    LoadPartnerOrders(selectedPartner.Id);

                    UpdatePartnerInfo(selectedPartner);

                    UpdatePartnersList();

                    StatusMessage = "Продажа успешно добавлена";
                }
            }
        }

        private void AddPartner()
        {
            var editWindow = new PartnerEditWindow(_mainApp);
            editWindow.Owner = this;

            if (editWindow.ShowDialog() == true)
            {
                UpdatePartnersList();
                StatusMessage = "Новый партнер успешно добавлен";
            }
        }
        
        private void EditSelectedPartner()
        {
            if (PartnersListBox.SelectedItem is Partners selectedPartner)
            {
                var partner = _mainApp.GetPartnerById(selectedPartner.Id);
                if (partner != null)
                {
                    var editWindow = new PartnerEditWindow(_mainApp, partner);
                    editWindow.Owner = this;

                    if (editWindow.ShowDialog() == true)
                    {
                        UpdatePartnersList();
                        if (PartnersListBox.SelectedItem is Partners updatedPartner)
                        {
                            LoadPartnerOrders(updatedPartner.Id);
                            UpdatePartnerInfo(updatedPartner);
                        }
                        StatusMessage = "Данные партнера успешно обновлены";
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите партнера для редактирования",
                    "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void DeleteSelectedPartner()
        {
            if (PartnersListBox.SelectedItem is Partners selectedPartner)
            {
                var result = MessageBox.Show(
                    $"Вы уверены, что хотите удалить партнера '{selectedPartner.Name}'?\n" +
                    "Это действие нельзя отменить.\n" +
                    "Вся история продаж этого партнера также будет удалена.",
                    "Подтверждение удаления",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        _mainApp.RemovePartner(selectedPartner.Id);
                        UpdatePartnersList();
                        OrdersListView.ItemsSource = null;
                        PartnerInfoPanel.Visibility = Visibility.Collapsed;
                        AddOrderButton.IsEnabled = false;
                        StatusMessage = "Партнер успешно удален";
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении: {ex.Message}",
                            "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите партнера для удаления",
                    "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void OnPartnersChanged()
        {
            Dispatcher.Invoke(() =>
            {
                UpdatePartnersList();
                OnPropertyChanged(nameof(StatusMessage));
            });
        }

        private void OnDataLoaded()
        {
            Dispatcher.Invoke(() =>
            {
                StatusMessage = "Данные успешно загружены";
            });
        }

        private void OnErrorOccurred(string errorMessage)
        {
            Dispatcher.Invoke(() =>
            {
                MessageBox.Show(errorMessage, "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                StatusMessage = "Произошла ошибка";
            });
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        
    }
}