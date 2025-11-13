using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using UCHEBKA.Models;
using UCHEBKA.Repos;

namespace UCHEBKA.Views.Helpers
{
    public partial class EventEditWindow : Window
    {
        private readonly EventRepository _eventRepo;
        private Event _currentEvent;
        private bool _isNewEvent;
        private string _selectedImagePath;

        public EventEditWindow(Event eventToEdit)
        {
            InitializeComponent();

            var context = new UchebnayaLeto2025Context();
            _eventRepo = new EventRepository(context);

            if (eventToEdit == null)
            {
                _currentEvent = new Event();
                _isNewEvent = true;
                Title = "Создание нового мероприятия";
                DeleteButton.IsEnabled = false;
            }
            else
            {
                _currentEvent = eventToEdit;
                _isNewEvent = false;
                Title = "Редактирование мероприятия";
            }

            LoadEventData();
        }

        private void LoadEventData()
        {
            EventTitleTextBox.Text = _currentEvent.EventTitle;
            EventStartTimePicker.SelectedDate = _currentEvent.EventStartTime;
            EventDurationTextBox.Text = _currentEvent.EventDuration?.ToString();
            EventLogoUrlTextBox.Text = _currentEvent.EventLogoUrl;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(EventTitleTextBox.Text) ||
                string.IsNullOrWhiteSpace(EventDurationTextBox.Text) ||
                EventStartTimePicker.SelectedDate == null)
            {
                MessageBox.Show("Пожалуйста, заполните все обязательные поля", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                _currentEvent.EventTitle = EventTitleTextBox.Text;
                _currentEvent.EventStartTime = EventStartTimePicker.SelectedDate;

                if (int.TryParse(EventDurationTextBox.Text, out int duration))
                {
                    _currentEvent.EventDuration = duration;
                }
                else
                {
                    _currentEvent.EventDuration = null;
                }

                // Если выбрано новое изображение, сохраняем только имя файла
                if (!string.IsNullOrEmpty(_selectedImagePath))
                {
                    _currentEvent.EventLogoUrl = Path.GetFileName(_selectedImagePath);
                }
                else if (string.IsNullOrEmpty(_currentEvent.EventLogoUrl))
                {
                    _currentEvent.EventLogoUrl = null;
                }

                if (_isNewEvent)
                {
                    _currentEvent.EventId = _eventRepo.GetNextEventId();
                    _eventRepo.AddEvent(_currentEvent);
                }
                else
                {
                    _eventRepo.UpdateEvent(_currentEvent);
                }

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении мероприятия: {ex.Message}",
                              "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                $"Вы уверены, что хотите удалить мероприятие '{_currentEvent.EventTitle}'?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    bool success = _eventRepo.DeleteEvent(_currentEvent.EventId);
                    if (success)
                    {
                        MessageBox.Show("Мероприятие успешно удалено", "Успех",
                                      MessageBoxButton.OK, MessageBoxImage.Information);
                        DialogResult = true;
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("Не удалось найти мероприятие для удаления", "Ошибка",
                                      MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении мероприятия: {ex.Message}",
                                  "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void SelectImageButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png|All files (*.*)|*.*",
                Title = "Выберите логотип мероприятия"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                _selectedImagePath = openFileDialog.FileName;
                EventLogoUrlTextBox.Text = Path.GetFileName(_selectedImagePath);
            }
        }
    }
}