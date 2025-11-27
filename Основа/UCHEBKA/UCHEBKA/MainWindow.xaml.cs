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
    /// <summary>
    /// Главное окно приложения для просмотра мероприятий
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly EventRepository _eventRepo;
        private readonly SectionRepository _sectionRepo;
        private readonly UserRepository _userRepo;
        private List<Event> _allEvents;
        private DateTime? _selectedDate = null;
        private int? _selectedSectionId = null;
        private User _currentUser;

        /// <summary>
        /// Инициализирует новый экземпляр главного окна
        /// </summary>
        public MainWindow()
        {
            var db = new UchebnayaLeto2025Context();
            _eventRepo = new EventRepository(db);
            _sectionRepo = new SectionRepository(db);
            _userRepo = new UserRepository(db);

            InitializeComponent();
            LoadData();
        }

        /// <summary>
        /// Загружает данные мероприятий и секций
        /// </summary>
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

        /// <summary>
        /// Обработчик нажатия кнопки авторизации
        /// </summary>
        private void AuthButton_Click(object sender, RoutedEventArgs e)
        {
            TryAutoLogin();

            if (_currentUser == null)
            {
                ShowAuthWindow();
            }
        }

        /// <summary>
        /// Пытается выполнить автоматический вход пользователя
        /// </summary>
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

        /// <summary>
        /// Открывает окно в зависимости от роли пользователя
        /// </summary>
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

        /// <summary>
        /// Показывает окно авторизации
        /// </summary>
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

        /// <summary>
        /// Отображает список мероприятий в интерфейсе
        /// </summary>
        /// <param name="events">Список мероприятий для отображения</param>
        private void ShowEvents(List<Event> events)
        {
            EventsWrapPanel.Children.Clear();
            foreach (var ev in events)
            {
                EventsWrapPanel.Children.Add(new EventCard(ev));
            }
        }

        /// <summary>
        /// Применяет фильтры к списку мероприятий
        /// </summary>
        private void ApplyFilters()
        {
            List<Event> filteredEvents;

            if (_selectedDate.HasValue && _selectedSectionId.HasValue && _selectedSectionId > 0)
            {
                var eventsBySection = _eventRepo.GetEventsBySection(_selectedSectionId.Value);
                var eventsByDate = _eventRepo.GetEventsByDate(_selectedDate);
                filteredEvents = eventsBySection.Intersect(eventsByDate).ToList();
            }
            else if (_selectedDate.HasValue)
            {
                filteredEvents = _eventRepo.GetEventsByDate(_selectedDate);
            }
            else if (_selectedSectionId.HasValue && _selectedSectionId > 0)
            {
                filteredEvents = _eventRepo.GetEventsBySection(_selectedSectionId.Value);
            }
            else
            {
                filteredEvents = _allEvents;
            }

            ShowEvents(filteredEvents);
        }

        /// <summary>
        /// Обработчик нажатия кнопки выбора даты
        /// </summary>
        private void DatePickerButton_Click(object sender, RoutedEventArgs e)
        {
            MyCalendar.Visibility = MyCalendar.Visibility == Visibility.Visible
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        /// <summary>
        /// Обработчик изменения выбранной даты в календаре
        /// </summary>
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

        /// <summary>
        /// Обработчик изменения выбранной секции в комбобоксе
        /// </summary>
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MyComboBox.SelectedValue != null && int.TryParse(MyComboBox.SelectedValue.ToString(), out int sectionId))
            {
                _selectedSectionId = sectionId > 0 ? sectionId : null;
                ApplyFilters();
            }
        }

        /// <summary>
        /// Обработчик нажатия кнопки сброса фильтров
        /// </summary>
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