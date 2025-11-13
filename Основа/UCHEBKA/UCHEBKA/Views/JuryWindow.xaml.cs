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
    public partial class JuryWindow : Window
    {
        private readonly UserRepository _usRepo;
        private readonly ActivityRepository _activityRepo;
        private readonly UchebnayaLeto2025Context _db;
        private User _currentUser;
        private ObservableCollection<ActivityRatingViewModel> _activitiesToRate;
        private bool _isInitialLoad = true;

        public JuryWindow()
        {
            InitializeComponent();
            _db = new UchebnayaLeto2025Context();
            _usRepo = new UserRepository(_db);
            _activityRepo = new ActivityRepository(_db);

            LoadUserData();
            LoadActivities();
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

        private void LoadActivities()
        {
            if (_currentUser == null) return;

            _isInitialLoad = true; // Устанавливаем флаг начальной загрузки

            // Отключаем уведомления об изменениях во время загрузки
            ActivitiesListView.ItemsSource = null;

            var activities = _activityRepo.GetActivitiesForJury(_currentUser.UserId)
                .Select(ae => new ActivityRatingViewModel
                {
                    ActivityEventId = ae.ActivityEventId,
                    Event = ae.FkEvent,
                    Activity = ae.FkActivity,
                    Day = ae.Day,
                    StartTime = ae.StartTime,
                    Rating = ae.FkActivity.ActivityScore ?? 0 // Устанавливаем начальное значение
                }).ToList();

            _activitiesToRate = new ObservableCollection<ActivityRatingViewModel>(activities);
            ActivitiesListView.ItemsSource = _activitiesToRate;

            _isInitialLoad = false; // Сбрасываем флаг после загрузки
        }

        private void RatingComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox?.SelectedItem == null || _isInitialLoad) return;

            // Проверяем, было ли это изменение пользователем (а не программное)
            if (e.AddedItems.Count == 0 || e.RemovedItems.Count == 0) return;

            var selectedActivity = comboBox.DataContext as ActivityRatingViewModel;
            if (selectedActivity == null || selectedActivity.Activity == null) return;

            // Получаем новое значение
            var newValue = (comboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            if (!int.TryParse(newValue, out int newRating)) return;

            try
            {
                // Обновляем оценку в модели представления
                selectedActivity.Rating = newRating;

                // Помечаем объект как измененный
                _db.Entry(selectedActivity.Activity).State = EntityState.Modified;

                // Сохраняем изменения
                int changes = _db.SaveChanges();

                if (changes > 0)
                {
                    MessageBox.Show("Оценка сохранена успешно!", "Успех",
                                  MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении оценки: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);

                // Отменяем изменения и перезагружаем данные
                _db.Entry(selectedActivity.Activity).Reload();
                LoadActivities(); // Перезагружаем все данные
            }
        }
    }

    public class ActivityRatingViewModel
    {
        public long ActivityEventId { get; set; }
        public Event Event { get; set; }
        public Activity Activity { get; set; }
        public int? Day { get; set; }
        public DateTime? StartTime { get; set; }

        private int? _rating;
        public int? Rating
        {
            get => _rating;
            set
            {
                if (_rating != value)
                {
                    _rating = value;
                    if (Activity != null)
                    {
                        Activity.ActivityScore = value;
                    }
                }
            }
        }
    }
}
