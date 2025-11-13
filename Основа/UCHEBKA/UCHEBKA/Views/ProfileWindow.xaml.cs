using Microsoft.Win32;
using System;
using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using UCHEBKA.Models;
using UCHEBKA.Repos;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using UCHEBKA.Views.Helpers;

namespace UCHEBKA.Views
{
    /// <summary>
    /// Логика взаимодействия для ProfileWindow.xaml
    /// </summary>
    public partial class ProfileWindow : Window
    {
        private readonly UserRepository _userRepo;
        private readonly User _currentUser;
        private string _tempPhotoPath;

        public ProfileWindow(UserRepository userRepo)
        {
            InitializeComponent();

            _userRepo = userRepo;
            _currentUser = _userRepo.GetCurrentUserData();

            if (_currentUser == null)
            {
                MessageBox.Show("Не удалось загрузить данные пользователя", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
                return;
            }

            LoadUserData();
        }

        private void LoadUserData()
        {
            if (_currentUser == null) return;

            // Основная информация
            SurnameTextBox.Text = _currentUser.UserSurname;
            NameTextBox.Text = _currentUser.UserName;
            LastnameTextBox.Text = _currentUser.UserLastname;
            BirthDatePicker.SelectedDate = _currentUser.UserBirthDay;
            PhoneTextBox.Text = _currentUser.UserPhone;

            // Контактная информация
            EmailTextBox.Text = _currentUser.UserEmail;

            // Загрузка пола
            SexComboBox.ItemsSource = _userRepo.GetAllSexes();
            if (_currentUser.UserSexes.Count > 0)
            {
                SexComboBox.SelectedValue = _currentUser.UserSexes.First().FkSexId;
            }

            // Загрузка фото
            if (!string.IsNullOrEmpty(_currentUser.UserPhoto) && File.Exists(_currentUser.UserPhoto))
            {
                ProfileImage.Source = new BitmapImage(new Uri(_currentUser.UserPhoto));
            }
        }

        private void ChangePhoto_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png",
                Title = "Выберите фото профиля"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                // Сохраняем ТОЛЬКО имя файла (например, "foto28.jpg")
                _tempPhotoPath = System.IO.Path.GetFileName(openFileDialog.FileName);

                // Для отображения используем полный путь из диалога
                ProfileImage.Source = new BitmapImage(new Uri(openFileDialog.FileName));
            }
        }

        private void ChangePassword_Click(object sender, RoutedEventArgs e)
        {
            //var passwordWindow = new ChangePasswordWindow(_currentUser.UserId, _userRepo);
            //passwordWindow.Owner = this;
            //passwordWindow.ShowDialog();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Обновление основной информации
                _currentUser.UserSurname = SurnameTextBox.Text;
                _currentUser.UserName = NameTextBox.Text;
                _currentUser.UserLastname = LastnameTextBox.Text;
                _currentUser.UserBirthDay = BirthDatePicker.SelectedDate;
                _currentUser.UserPhone = PhoneTextBox.Text;

                // Обновление контактной информации
                _currentUser.UserEmail = EmailTextBox.Text;

                // Обновление фото
                if (!string.IsNullOrEmpty(_tempPhotoPath))
                {
                    _currentUser.UserPhoto = _tempPhotoPath;
                }

                // Обновление пола
                if (SexComboBox.SelectedItem is Sex selectedSex)
                {
                    _userRepo.UpdateUserSex(_currentUser.UserId, selectedSex.SexId);
                }

                // Сохранение изменений
                _userRepo.UpdateUser(_currentUser);

                MessageBox.Show("Данные успешно сохранены", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void LogoutBtn_Click(object sender, RoutedEventArgs e)
        {
            var parentWindow = this.Owner;
            MainWindow mainWindow = new MainWindow();
            _userRepo.Logout();
            this.Close();
            mainWindow.Show();
            parentWindow?.Close();
        }
    }
}
