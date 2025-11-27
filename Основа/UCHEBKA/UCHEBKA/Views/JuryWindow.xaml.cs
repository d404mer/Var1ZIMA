using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using UCHEBKA.Models;
using UCHEBKA.Repos;
using UCHEBKA.Views;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using Microsoft.EntityFrameworkCore;

namespace UCHEBKA
{
    /// <summary>
    /// Окно для работы жюри
    /// </summary>
    public partial class JuryWindow : Window
    {
        private readonly UserRepository _usRepo;
        private readonly UchebnayaLeto2025Context _db;
        private User _currentUser;
        private bool _isInitialLoad = true;

        /// <summary>
        /// Инициализирует новый экземпляр окна жюри
        /// </summary>
        public JuryWindow()
        {
            InitializeComponent();
            _db = new UchebnayaLeto2025Context();
            _usRepo = new UserRepository(_db);

            LoadUserData();
        }

        private void ProfileBtn_Click(object sender, RoutedEventArgs e)
        {
            var currentUser = _usRepo.GetCurrentUserData();
            if (currentUser != null)
            {
                var profileWindow = new ProfileWindow(_usRepo);
                profileWindow.Owner = this;
                profileWindow.ShowDialog();
                LoadUserData();
            }
            else
            {
                MessageBox.Show("Не удалось загрузить данные пользователя", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
            }
        }

        private void LoadUserData()
        {
            var currentUser = _usRepo.GetCurrentUser();
            if (currentUser != null)
            {
                _currentUser = _usRepo.Auth(currentUser.Value.userId, currentUser.Value.password);
                string imageName = _currentUser.UserPhoto;
                var ProfileImagePath = _usRepo.GetDisplayImagePath(imageName);
                ProfileImage.Source = new BitmapImage(new Uri(ProfileImagePath));
                DataContext = this;
            }
        }


    }
}
