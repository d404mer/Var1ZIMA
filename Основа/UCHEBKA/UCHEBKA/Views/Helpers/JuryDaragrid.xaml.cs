using System;
using Microsoft.EntityFrameworkCore;
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
    /// Логика взаимодействия для JuryDaragrid.xaml
    /// </summary>
    public partial class JuryDaragrid : Window
    {
        public JuryDaragrid()
        {
            InitializeComponent();
            LoadUsersWithRole4();
            
            // Добавляем обработчик двойного клика
            UsersWithRole4DataGrid.MouseDoubleClick += UsersWithRole4DataGrid_MouseDoubleClick;
        }

        private void LoadUsersWithRole4()
        {
            try
            {
                using (var context = new UchebnayaLeto2025Context())
                {
                    var usersWithRole1 = context.UserRoles
                        .Include(ur => ur.FkUser)
                        .Include(ur => ur.FkRole)
                        .Where(ur => ur.FkRoleId == 4)
                        .ToList();

                    UsersWithRole4DataGrid.ItemsSource = usersWithRole1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке пользователей: {ex.Message}",
                              "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadUsersWithRole4();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new UserEditWindow(null, 4); // 4 - жюри
            if (addWindow.ShowDialog() == true)
            {
                LoadUsersWithRole4(); // обновить таблицу
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void UsersWithRole4DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectedItem = UsersWithRole4DataGrid.SelectedItem as UserRole;
            if (selectedItem != null)
            {
                var editWindow = new UserEditWindow(selectedItem.FkUser, 4); // 4 - роль жюри
                if (editWindow.ShowDialog() == true)
                {
                    LoadUsersWithRole4(); // Перезагружаем данные после редактирования
                }
            }
        }
    }
}
