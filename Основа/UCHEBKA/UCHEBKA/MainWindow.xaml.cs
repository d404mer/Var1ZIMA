using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using UCHEBKA.Models;
using UCHEBKA.Repos;
using UCHEBKA.Views;

namespace UCHEBKA
{
    public partial class MainWindow : Window
    {
        private readonly EventRepository _eventRepo;
        private readonly SectionRepository _sectionRepo;
        private readonly UserRepository _userRepo;
        private List<Event> _allEvents;
        private DateTime? _selectedDate = null;
        private int? _selectedSectionId = null;
        private User _currentUser;

        public MainWindow()
        {
            var db = new UchebnayaLeto2025Context();
            _eventRepo = new EventRepository(db);
            _sectionRepo = new SectionRepository(db);
            _userRepo = new UserRepository(db);

            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            _allEvents = _eventRepo.GetAllEvents();
            var sections = _sectionRepo.GetAllSections();

            var allSections = new List<Section> { new Section { SecId = 0, SecName = "Все секции" } };
            allSections.AddRange(sections);

            MyComboBox.ItemsSource = allSections;
            MyComboBox.SelectedIndex = 0;

            ShowEvents(_allEvents);
        }

        private void AuthButton_Click(object sender, RoutedEventArgs e)
        {
            // Пытаемся выполнить автоматический вход
            TryAutoLogin();

            // Если автоматический вход не удался, показываем окно авторизации
            if (_currentUser == null)
            {
                ShowAuthWindow();
            }
        }

        private void TryAutoLogin()
        {
            var cred = _userRepo.GetCurrentUser();
            if (cred != null)
            {
                var (userId, password) = cred.Value;
                _currentUser = _userRepo.Auth(userId, password);

                if (_currentUser != null)
                {
                    OpenRoleBasedWindow();
                }
            }
        }

        private void OpenRoleBasedWindow()
        {
            var role = _userRepo.GetUserRole(_currentUser.UserId);

            Window window = role switch
            {
                "модератор" => new AdminWindow(),
                "жюри" => new JuryWindow(),
                "организатор" => new OrgWindow(),
                _ => new ParticipantWindow()
            };

            window.Show();
            this.Close();
        }

        private void ShowAuthWindow()
        {
            AuthWindow authWindow = new AuthWindow();
            authWindow.Closed += (s, args) =>
            {
                if (authWindow.DialogResult == true)
                {
                    this.Close();
                }
            };
            authWindow.ShowDialog();
        }

        private void ShowEvents(List<Event> events)
        {
            EventsWrapPanel.Children.Clear();
            foreach (var ev in events)
            {
                EventsWrapPanel.Children.Add(new EventCard(ev));
            }
        }

        private void ApplyFilters()
        {
            List<Event> filteredEvents;

            // Если выбраны и дата и секция
            if (_selectedDate.HasValue && _selectedSectionId.HasValue && _selectedSectionId > 0)
            {
                // Получаем события по секции
                var eventsBySection = _eventRepo.GetEventsBySection(_selectedSectionId.Value);
                // Получаем события по дате
                var eventsByDate = _eventRepo.GetEventsByDate(_selectedDate);
                // Находим пересечение
                filteredEvents = eventsBySection.Intersect(eventsByDate).ToList();
            }
            // Если выбрана только дата
            else if (_selectedDate.HasValue)
            {
                filteredEvents = _eventRepo.GetEventsByDate(_selectedDate);
            }
            // Если выбрана только секция
            else if (_selectedSectionId.HasValue && _selectedSectionId > 0)
            {
                filteredEvents = _eventRepo.GetEventsBySection(_selectedSectionId.Value);
            }
            // Если фильтры не выбраны
            else
            {
                filteredEvents = _allEvents;
            }

            ShowEvents(filteredEvents);
        }

        private void DatePickerButton_Click(object sender, RoutedEventArgs e)
        {
            MyCalendar.Visibility = MyCalendar.Visibility == Visibility.Visible
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        private void MyCalendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedDate = MyCalendar.SelectedDate;
            MyCalendar.Visibility = Visibility.Collapsed;

            if (_selectedDate.HasValue)
            {
                SelectedDateText.Text = $"Выбранная дата: {_selectedDate.Value.ToShortDateString()}";
            }
            else
            {
                SelectedDateText.Text = string.Empty;
            }

            ApplyFilters();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MyComboBox.SelectedValue != null && int.TryParse(MyComboBox.SelectedValue.ToString(), out int sectionId))
            {
                _selectedSectionId = sectionId > 0 ? sectionId : null;
                ApplyFilters();
            }
        }

        private void ResetFilters_Click(object sender, RoutedEventArgs e)
        {
            _selectedDate = null;
            _selectedSectionId = null;
            MyComboBox.SelectedIndex = 0;
            SelectedDateText.Text = string.Empty;
            MyCalendar.SelectedDate = null;
            ShowEvents(_allEvents);
        }

       
    }
}