using Microsoft.EntityFrameworkCore;
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
using UCHEBKA.Views;

namespace UCHEBKA.Views.Helpers
{
    /// <summary>
    /// Логика взаимодействия для ParticipantsDatagrid.xaml
    /// </summary>
    public partial class ParticipantsDatagrid : Window
    {
        public ParticipantsDatagrid()
        {
            InitializeComponent();
            LoadUsersWithRole1();
            
            // Добавляем обработчик двойного клика
            UsersWithRole1DataGrid.MouseDoubleClick += UsersWithRole1DataGrid_MouseDoubleClick;
        }

        private void LoadUsersWithRole1()
        {
            try
            {
                using (var context = new UchebnayaLeto2025Context())
                {
                    var usersWithRole1 = context.UserRoles
                        .Include(ur => ur.FkUser)
                        .Include(ur => ur.FkRole)
                        .Where(ur => ur.FkRoleId == 1)
                        .ToList();

                    UsersWithRole1DataGrid.ItemsSource = usersWithRole1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке пользователей: {ex.Message}",
                              "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UsersWithRole1DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectedItem = UsersWithRole1DataGrid.SelectedItem as UserRole;
            if (selectedItem != null)
            {
                var editWindow = new UserEditWindow(selectedItem.FkUser, 1); // 1 - роль участника
                if (editWindow.ShowDialog() == true)
                {
                    LoadUsersWithRole1(); // Перезагружаем данные после редактирования
                }
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new UserEditWindow(null, 1); // 1 - участник
            if (addWindow.ShowDialog() == true)
            {
                LoadUsersWithRole1(); // обновить таблицу
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadUsersWithRole1();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
