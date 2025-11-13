using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using UCHEBKA.Models;
using UCHEBKA.Repos;
using UCHEBKA.Views;
using Microsoft.EntityFrameworkCore;

namespace UCHEBKA
{
    /// <summary>
    /// Логика взаимодействия для AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {
        private readonly UserRepository _usRepo;
        private readonly EventRepository _eventRepo;
        private readonly UchebnayaLeto2025Context _db;
        private User _currentUser;
        private List<EventViewModel> _events;
        private List<ActivityViewModel> _activities;

        public AdminWindow()
        {
            InitializeComponent();

            _db = new UchebnayaLeto2025Context();
            _usRepo = new UserRepository(_db);
            _eventRepo = new EventRepository(_db);

            LoadUserData();

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
            }
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
                MessageBox.Show("Не удалось загрузить данные пользователя");
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
            }
        }

        
    }

    public class EventViewModel
    {
        public Event Event { get; set; }
        public bool IsRegistered { get; set; }
    }

    public class ActivityViewModel
    {
        public bool IsModerating { get; set; }
    }
}
