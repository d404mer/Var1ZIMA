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
using UCHEBKA.Views.Helpers;

namespace UCHEBKA.Views
{
    /// <summary>
    /// Логика взаимодействия для EventsDatagrid.xaml
    /// </summary>
    public partial class EventsDatagrid : Window
    {
        private readonly EventRepository _eventRepo;

        public EventsDatagrid()
        {
            InitializeComponent();
            _eventRepo = new EventRepository(new UchebnayaLeto2025Context());
            LoadEvents();
        }

        private void LoadEvents()
        {
            try
            {
                var events = _eventRepo.GetAllEvents();
                EventsDataGrid.ItemsSource = events;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке мероприятий: {ex.Message}",
                              "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadEvents();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            // Логика добавления нового мероприятия
            var editWindow = new EventEditWindow(null); // null означает создание нового
            editWindow.ShowDialog();

            if (editWindow.DialogResult == true)
            {
                LoadEvents(); // Обновляем список после добавления
            }
        }

        private void EventsDataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (EventsDataGrid.SelectedItem is Event selectedEvent)
            {
                var editWindow = new EventEditWindow(selectedEvent);
                editWindow.ShowDialog();

                if (editWindow.DialogResult == true)
                {
                    LoadEvents();
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
