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
            LoadEvents();

            // Добавляем обработчик выбора мероприятия
            EventsListView.SelectionChanged += EventsListView_SelectionChanged;
        }

        private void EventsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedEvent = EventsListView.SelectedItem as EventViewModel;
            if (selectedEvent != null)
            {
                LoadActivities(selectedEvent.Event.EventId);
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
            }
        }

        private void LoadEvents()
        {
            if (_currentUser == null) return;

            var events = _eventRepo.GetAllEvents();
            Console.WriteLine($"Loaded {events.Count} events");
            foreach (var ev in events)
            {
                Console.WriteLine($"Event: {ev.EventTitle}, LogoUrl: {ev.EventLogoUrl}");
            }

            var registeredEvents = _db.UserEvents
                .Where(ue => ue.FkUserId == _currentUser.UserId)
                .Select(ue => ue.FkEventId)
                .ToList();

            _events = events.Select(e => new EventViewModel
            {
                Event = e,
                IsRegistered = registeredEvents.Contains(e.EventId)
            }).ToList();

            EventsListView.ItemsSource = _events;
        }

        private void LoadActivities(long eventId)
        {
            if (_currentUser == null) return;

            var activities = _db.ActivityEvents
                .Where(ae => ae.FkEventId == eventId)
                .Include(ae => ae.FkActivity)
                .ToList();

            _activities = activities.Select(ae => new ActivityViewModel
            {
                ActivityEvent = ae,
                IsModerating = ae.FkModId == _currentUser.UserId
            }).ToList();

            ActivitiesListView.ItemsSource = _activities;
        }

        private void RegisterForEvent_Click(object sender, RoutedEventArgs e)
        {
            if (_currentUser == null) return;

            var button = sender as Button;
            var eventViewModel = button?.DataContext as EventViewModel;
            if (eventViewModel == null) return;

            try
            {
                if (eventViewModel.IsRegistered)
                {
                    MessageBox.Show("Вы уже зарегистрированы на это мероприятие");
                    return;
                }

                _db.UserEvents.Add(new UserEvent
                {
                    FkUserId = _currentUser.UserId,
                    FkEventId = eventViewModel.Event.EventId
                });

                _db.SaveChanges();
                eventViewModel.IsRegistered = true;
                EventsListView.Items.Refresh();
                MessageBox.Show("Вы успешно зарегистрированы на мероприятие");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при регистрации: {ex.Message}");
            }
        }

        private void ModerateActivity_Click(object sender, RoutedEventArgs e)
        {
            if (_currentUser == null) return;

            var button = sender as Button;
            var activityViewModel = button?.DataContext as ActivityViewModel;
            if (activityViewModel == null) return;

            try
            {
                if (activityViewModel.IsModerating)
                {
                    MessageBox.Show("Вы уже являетесь модератором этой активности");
                    return;
                }

                activityViewModel.ActivityEvent.FkModId = _currentUser.UserId;
                _db.SaveChanges();
                activityViewModel.IsModerating = true;
                ActivitiesListView.Items.Refresh();
                MessageBox.Show("Вы успешно назначены модератором активности");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при назначении модератором: {ex.Message}");
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
        public ActivityEvent ActivityEvent { get; set; }
        public bool IsModerating { get; set; }
    }
}
