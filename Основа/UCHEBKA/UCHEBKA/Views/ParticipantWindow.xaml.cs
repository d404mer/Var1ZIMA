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
using UCHEBKA.Models;
using UCHEBKA.Repos;
using UCHEBKA.Views;

namespace UCHEBKA
{
    /// <summary>
    /// Логика взаимодействия для ParticipantWindow.xaml
    /// </summary>
    public partial class ParticipantWindow : Window
    {
        private readonly UserRepository _usRepo;
        private readonly UchebnayaLeto2025Context _db;
        private User _currentUser;

        public ParticipantWindow()
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

                if (_currentUser != null)
                {
                    SetGreeting();
                    MrMrsText.Text = GetUserTitle(_currentUser.UserId) + " " + _currentUser.UserLastname;
                }


                string imageName = _currentUser.UserPhoto;
                var ProfileImagePath = _usRepo.GetDisplayImagePath(imageName);

                ProfileImage.Source = new BitmapImage(new Uri(ProfileImagePath));

                IdTextBlock.Text = $"ID: {_currentUser.UserId}";
                FullNameTextBlock.Text = $"ФИО: {_currentUser.UserSurname} {_currentUser.UserLastname} {_currentUser.UserName}";
                EmailTextBlock.Text = $"Почта: {_currentUser.UserEmail}";
                BirthDateTextBlock.Text = $"Дата рождения: {_currentUser.UserBirthDay:dd.MM.yyyy}";
                PhoneTextBlock.Text = $"Телефонный номер: {_currentUser.UserPhone}";
            }
        }

        private string GetUserTitle(long userId)
        {
            var userSex = _usRepo.GetUserSex(userId);

            return userSex switch
            {
                "мужской" => "Mr",
                "женский" => "Ms",
                _ => string.Empty    // если пол не указан или не распознан
            };
        }

        private void SetGreeting()
        {
            var currentHour = DateTime.Now.Hour;

            if (currentHour >= 9 && currentHour < 11)
            {
                GoodText.Text = "Доброе утро";
            }
            else if (currentHour >= 11 && currentHour < 18)
            {
                GoodText.Text = "Добрый день";
            }
            else
            {
                GoodText.Text = "Добрый вечер";
            }
        }
    }
}
