using semenova_library;
using Microsoft.Win32;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace Semenova_ind
{
    public partial class PartnerEditWindow : Window
    {
        private readonly IMainApp _mainApp;
        private readonly Partners _currentPartner;
        private readonly bool _isEditMode;

        public PartnerEditWindow(IMainApp mainApp, Partners partner = null)
        {
            InitializeComponent();

            _mainApp = mainApp;
            _currentPartner = partner ?? new Partners();
            _isEditMode = partner != null;

            LoadData();

            if (_isEditMode)
            {
                TitleTextBlock.Text = "Редактирование партнера";
                LoadPartnerData();
            }
        }

        private void LoadData()
        {
            try
            {
                var types = _mainApp.GetAllPartnerTypes().ToList();
                TypeComboBox.ItemsSource = types;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadPartnerData()
        {
            NameTextBox.Text = _currentPartner.Name;
            TypeComboBox.SelectedValue = _currentPartner.TypeId;
            AddressTextBox.Text = _currentPartner.LegalAdress;
            InnTextBox.Text = _currentPartner.Inn;
            DirectorTextBox.Text = _currentPartner.DirectorFullname;
            PhoneTextBox.Text = _currentPartner.Phone;
            EmailTextBox.Text = _currentPartner.Email;
            RatingTextBox.Text = _currentPartner.Rating.ToString();
           
        }

        private bool ValidateInputs()
        {
            if (TypeComboBox.SelectedItem == null)
            {
                ShowError("Выберите тип партнера");
                return false;
            }

            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                ShowError("Введите наименование компании");
                NameTextBox.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(DirectorTextBox.Text))
            {
                ShowError("Введите ФИО директора");
                DirectorTextBox.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(PhoneTextBox.Text))
            {
                ShowError("Введите телефон");
                PhoneTextBox.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(EmailTextBox.Text))
            {
                ShowError("Введите email");
                EmailTextBox.Focus();
                return false;
            }

            try
            {
                var addr = new System.Net.Mail.MailAddress(EmailTextBox.Text);
            }
            catch
            {
                ShowError("Введите корректный email адрес");
                EmailTextBox.Focus();
                return false;
            }

            if (!int.TryParse(RatingTextBox.Text, out int rating) || rating < 0)
            {
                ShowError("Рейтинг должен быть целым неотрицательным числом");
                RatingTextBox.Focus();
                return false;
            }


            if (!string.IsNullOrWhiteSpace(InnTextBox.Text))
            {
                string inn = InnTextBox.Text.Trim();
                if (inn.Length != 10 && inn.Length != 12)
                {
                    ShowError("ИНН должен содержать 10 или 12 цифр");
                    InnTextBox.Focus();
                    return false;
                }

                if (!Regex.IsMatch(inn, @"^\d+$"))
                {
                    ShowError("ИНН должен содержать только цифры");
                    InnTextBox.Focus();
                    return false;
                }

             
                if (!_mainApp.IsInnUnique(inn, _isEditMode ? _currentPartner.Id : (int?)null))
                {
                    ShowError("Партнер с таким ИНН уже существует");
                    InnTextBox.Focus();
                    return false;
                }
            }

            return true;
        }

        private void ShowError(string message)
        {
            MessageBox.Show(message, "Ошибка ввода",
                MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!ValidateInputs())
                    return;

                _currentPartner.TypeId = (int)TypeComboBox.SelectedValue;
                _currentPartner.Name = NameTextBox.Text.Trim();
                _currentPartner.LegalAdress = AddressTextBox.Text?.Trim();
                _currentPartner.Inn = InnTextBox.Text?.Trim();
                _currentPartner.DirectorFullname = DirectorTextBox.Text.Trim();
                _currentPartner.Phone = PhoneTextBox.Text.Trim();
                _currentPartner.Email = EmailTextBox.Text.Trim();
                _currentPartner.Rating = int.Parse(RatingTextBox.Text);
             

                if (_isEditMode)
                {
                    _mainApp.UpdatePartner(_currentPartner);
                }
                else
                {
                    _mainApp.AddPartner(_currentPartner);
                }

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        

        private void RatingTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, @"^\d+$");
        }
    }
}