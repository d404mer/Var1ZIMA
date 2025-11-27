using Microsoft.Win32;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using UCHEBKA.Models;
using UCHEBKA.Repos;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text.RegularExpressions;

namespace UCHEBKA.Views
{
    public partial class UserEditWindow : Window
    {
        private readonly UserRepository _userRepo;
        private readonly EventRepository _eventRepo;
        private readonly UchebnayaLeto2025Context _db;
        private User _user;
        private int _roleId;
        private string _photoFileName;
        private bool _isEditMode;
        private readonly SectionRepository _sectionRepo;
        private const string PhoneMaskPlaceholder = "+7(___)-___-__-__";

        public UserEditWindow(User user, int roleId)
        {
            InitializeComponent();
            _db = new UchebnayaLeto2025Context();
            _userRepo = new UserRepository(_db);
            _eventRepo = new EventRepository(_db);
            _sectionRepo = new SectionRepository(_db);
            _roleId = roleId;
            _isEditMode = user != null;
            _user = user ?? new User();
            LoadData();

            // Инициализируем маску телефона, если поле пустое
            if (string.IsNullOrEmpty(PhoneTextBox.Text))
            {
                PhoneTextBox.Text = PhoneMaskPlaceholder;
                PhoneTextBox.CaretIndex = 3; // Помещаем курсор после +7(
            }
        }

        private void LoadData()
        {
            // Пол
            SexComboBox.ItemsSource = _userRepo.GetAllSexes();
            SexComboBox.DisplayMemberPath = "SexName";
            SexComboBox.SelectedValuePath = "SexId";

            // Роль (загружаем только id и имя)
            var roles = _db.Roles.Select(r => new { r.RoleId, r.RoleName }).ToList();
            RoleComboBox.ItemsSource = roles;
            RoleComboBox.DisplayMemberPath = "RoleName";
            RoleComboBox.SelectedValuePath = "RoleId";
            RoleComboBox.SelectedValue = _roleId;
            RoleComboBox.SelectionChanged += RoleComboBox_SelectionChanged;

            // Мероприятия
            EventComboBox.ItemsSource = _eventRepo.GetAllEvents();
            EventComboBox.DisplayMemberPath = "EventTitle";
            EventComboBox.SelectedValuePath = "EventId";

            // Направления
            DirectionComboBox.ItemsSource = _sectionRepo.GetAllSections();
            DirectionComboBox.DisplayMemberPath = "SecName";
            DirectionComboBox.SelectedValuePath = "SecId";

            // Скрываем поля для участников
            SetFieldsVisibility((int)(_isEditMode ? _roleId : (RoleComboBox.SelectedValue ?? 1)));

            if (_isEditMode)
            {
                IdTextBox.Text = _user.UserId.ToString();
                FioTextBox.Text = $"{_user.UserSurname} {_user.UserName} {_user.UserLastname}";
                EmailTextBox.Text = _user.UserEmail;
                PhoneTextBox.Text = _user.UserPhone;
                _photoFileName = _user.UserPhoto;
                if (!string.IsNullOrEmpty(_photoFileName))
                {
                    var path = _userRepo.GetDisplayImagePath(_photoFileName);
                    PhotoImage.Source = new BitmapImage(new Uri(path));
                }
                // Пол
                var sexId = _userRepo.GetUserSexId(_user.UserId);
                if (sexId != null && sexId != 0)
                    SexComboBox.SelectedValue = sexId;
                else
                    SexComboBox.SelectedIndex = -1;
                // Роль
                RoleComboBox.SelectedValue = _roleId;
                // Пароль
                PasswordTextBox.Password = _user.UserPassword;
                RepeatPasswordTextBox.Password = _user.UserPassword;
                // Направление
                if (_user.UserSecs.Any())
                    DirectionComboBox.SelectedValue = _user.UserSecs.First().FkSecId;

                DeleteButton.Visibility = Visibility.Visible;

                // Если номер телефона загружен, но он пустой, применяем маску
                if (string.IsNullOrEmpty(_user.UserPhone))
                {
                    PhoneTextBox.Text = PhoneMaskPlaceholder;
                    PhoneTextBox.CaretIndex = 3;
                }
            }
            else
            {
                IdTextBox.Text = "(авто)";
                DeleteButton.Visibility = Visibility.Collapsed;
                PhoneTextBox.Text = PhoneMaskPlaceholder; // Применяем маску по умолчанию для нового пользователя
                PhoneTextBox.CaretIndex = 3;
            }
        }

        private void RoleComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RoleComboBox.SelectedValue != null)
            {
                SetFieldsVisibility((int)(long)RoleComboBox.SelectedValue);
            }
        }

        private void SetFieldsVisibility(int roleId)
        {
            // 1 - участник, скрываем направление и мероприятие
            var isParticipant = roleId == 1;

            // Устанавливаем текст в зависимости от режима (редактирование/добавление) и роли
            TextBlock.Text = _isEditMode
                ? (isParticipant ? "Редактирование участника" : "Редактирование модератора/жюри")
                : (isParticipant ? "Добавление нового участника" : "Добавление нового модератора/жюри");

            // Остальные элементы
            DirectionPanel.Visibility = isParticipant ? Visibility.Collapsed : Visibility.Visible;
            EventPanel.Visibility = isParticipant ? Visibility.Collapsed : Visibility.Visible;
        }

        private void UploadPhoto_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png",
                Title = "Выберите фото профиля"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                _photoFileName = System.IO.Path.GetFileName(openFileDialog.FileName);
                PhotoImage.Source = new BitmapImage(new Uri(openFileDialog.FileName));
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            // Валидация
            if (string.IsNullOrWhiteSpace(FioTextBox.Text) ||
                SexComboBox.SelectedValue == null ||
                RoleComboBox.SelectedValue == null ||
                string.IsNullOrWhiteSpace(EmailTextBox.Text) ||
                string.IsNullOrWhiteSpace(PhoneTextBox.Text.Replace("+", "").Replace("(", "").Replace(")", "").Replace("-", "").Replace("_", "")))
            {
                MessageBox.Show("Заполните все обязательные поля");
                return;
            }

            // Дополнительная валидация перед сохранением
            PhoneTextBox_LostFocus(PhoneTextBox, null);
            EmailTextBox_LostFocus(EmailTextBox, null);

            // Теперь проверяем, валидны ли поля, не возвращая фокус
            string cleanPhoneText = new string(PhoneTextBox.Text.Where(char.IsDigit).ToArray());
            if (cleanPhoneText.Length != 11 || !cleanPhoneText.StartsWith("7"))
            {
                MessageBox.Show("Некорректный формат телефона. Ожидается: +7(XXX)-XXX-XX-XX", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string email = EmailTextBox.Text;
            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(email, emailPattern))
            {
                MessageBox.Show("Некорректный формат Email. Пример: example@domain.com", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string password = GetCurrentPassword();
            string repeatPassword = GetCurrentRepeatPassword();

            // Валидация пароля
            if (!_isEditMode) // Валидация только при создании нового пользователя
            {
                if (!ValidatePassword(password))
                {
                    return;
                }
                if (password != repeatPassword)
                {
                    MessageBox.Show("Пароли не совпадают.", "Ошибка валидации пароля", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            else // В режиме редактирования, если пароль изменяется, тоже его валидируем
            {
                // Валидируем, только если хотя бы одно поле пароля не пустое (т.е. пользователь хочет его изменить)
                if (!string.IsNullOrEmpty(password) || !string.IsNullOrEmpty(repeatPassword))
                {
                    if (!ValidatePassword(password))
                    {
                        return;
                    }
                    if (password != repeatPassword)
                    {
                        MessageBox.Show("Пароли не совпадают.", "Ошибка валидации пароля", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
            }

            try
            {
                // ФИО разбор
                var fio = FioTextBox.Text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                _user.UserSurname = fio.Length > 0 ? fio[0] : "";
                _user.UserName = fio.Length > 1 ? fio[1] : "";
                _user.UserLastname = fio.Length > 2 ? fio[2] : "";
                _user.UserEmail = EmailTextBox.Text;
                _user.UserPhone = new string(PhoneTextBox.Text.Where(char.IsDigit).ToArray()); // Извлекаем только цифры для сохранения
                _user.UserPhoto = _photoFileName;

                // --- Отладочный вывод --- //
                Console.WriteLine($"DEBUG: Surname={_user.UserSurname}, Name={_user.UserName}, Lastname={_user.UserLastname}");
                Console.WriteLine($"DEBUG: Email={_user.UserEmail}");
                Console.WriteLine($"DEBUG: Phone={_user.UserPhone}");
                // --- Конец отладочного вывода --- //

                var roleId = Convert.ToInt64(RoleComboBox.SelectedValue);
                var sexId = Convert.ToInt64(SexComboBox.SelectedValue);

                if (!_isEditMode)
                {
                    _user.UserId = _userRepo.GetNextUserId();
                    _user.UserPassword = password; // Используем отвалидированный пароль
                    _userRepo.AddUser(_user); // Сохраняем пользователя, чтобы UserId был в базе

                    // Добавляем роль
                    _db.UserRoles.Add(new UserRole 
                    {
                        UserRoleId = _userRepo.GetNextUserRoleId(), // Присваиваем ID
                        FkUserId = _user.UserId,
                        FkRoleId = roleId
                    });
                    _db.SaveChanges(); // Сохраняем UserRole

                    // Добавляем пол
                    _userRepo.UpdateUserSex(_user.UserId, sexId);

                    // Добавляем направление (если не участник)
                    if (roleId != 1 && DirectionComboBox.SelectedValue != null)
                    {
                        long secId = Convert.ToInt64(DirectionComboBox.SelectedValue);
                        // Проверка на дубликат UserSec (хотя GetNextUserSecId уже должен гарантировать уникальность)
                        //if (_db.UserSecs.All(us => us.FkUserId != _user.UserId))
                        {
                            _db.UserSecs.Add(new UserSec 
                            {
                                UserSecId = _userRepo.GetNextUserSecId(), // Присваиваем ID
                                FkUserId = _user.UserId,
                                FkSecId = secId
                            });
                            _db.SaveChanges(); // Сохраняем UserSec
                        }
                    }
                }
                else
                {
                    _user.UserPassword = password; // Используем отвалидированный пароль (если изменен)
                    _userRepo.UpdateUser(_user);
                    _userRepo.UpdateUserSex(_user.UserId, sexId);

                    // Обновляем роль
                    var userRole = _db.UserRoles.FirstOrDefault(ur => ur.FkUserId == _user.UserId);
                    if (userRole != null)
                    {
                        userRole.FkRoleId = roleId;
                    }
                    else
                    {
                        _db.UserRoles.Add(new UserRole 
                        {
                            UserRoleId = _userRepo.GetNextUserRoleId(), // Присваиваем ID при добавлении
                            FkUserId = _user.UserId,
                            FkRoleId = roleId
                        });
                    }

                    // Обновляем направление (если не участник)
                    if (roleId != 1 && DirectionComboBox.SelectedValue != null)
                    {
                        long secId = Convert.ToInt64(DirectionComboBox.SelectedValue);
                        var userSec = _db.UserSecs.FirstOrDefault(us => us.FkUserId == _user.UserId);
                        if (userSec != null)
                        {
                            userSec.FkSecId = secId;
                        }
                        else
                        {
                            _db.UserSecs.Add(new UserSec 
                            {
                                UserSecId = _userRepo.GetNextUserSecId(), // Присваиваем ID при добавлении
                                FkUserId = _user.UserId,
                                FkSecId = secId
                            });
                        }
                    }
                    _db.SaveChanges();
                }

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}\n{ex.InnerException?.Message}");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void ShowPasswordCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            PasswordTextBox.Visibility = Visibility.Collapsed;
            PlainPasswordTextBox.Visibility = Visibility.Visible;
            PlainPasswordTextBox.Text = PasswordTextBox.Password;

            RepeatPasswordTextBox.Visibility = Visibility.Collapsed;
            PlainRepeatPasswordTextBox.Visibility = Visibility.Visible;
            PlainRepeatPasswordTextBox.Text = RepeatPasswordTextBox.Password;
        }

        private void ShowPasswordCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            PasswordTextBox.Visibility = Visibility.Visible;
            PlainPasswordTextBox.Visibility = Visibility.Collapsed;
            PasswordTextBox.Password = PlainPasswordTextBox.Text;

            RepeatPasswordTextBox.Visibility = Visibility.Visible;
            PlainRepeatPasswordTextBox.Visibility = Visibility.Collapsed;
            RepeatPasswordTextBox.Password = PlainRepeatPasswordTextBox.Text;
        }

        // Вспомогательные методы для получения текущего значения пароля
        private string GetCurrentPassword()
        {
            return ShowPasswordCheckBox.IsChecked == true ? PlainPasswordTextBox.Text : PasswordTextBox.Password;
        }

        private string GetCurrentRepeatPassword()
        {
            return ShowPasswordCheckBox.IsChecked == true ? PlainRepeatPasswordTextBox.Text : RepeatPasswordTextBox.Password;
        }

        // Метод для валидации пароля
        private bool ValidatePassword(string password)
        {
            if (string.IsNullOrEmpty(password) || password.Length < 6)
            {
                MessageBox.Show("Пароль должен содержать не менее 6 символов.", "Ошибка валидации пароля", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            bool hasUpper = false;
            bool hasLower = false;
            bool hasDigit = false;
            bool hasSpecial = false;

            foreach (char c in password)
            {
                if (char.IsUpper(c)) hasUpper = true;
                else if (char.IsLower(c)) hasLower = true;
                else if (char.IsDigit(c)) hasDigit = true;
                else if (!char.IsLetterOrDigit(c)) hasSpecial = true; // Проверяем на спецсимволы
            }

            if (!hasUpper)
            {
                MessageBox.Show("Пароль должен содержать хотя бы одну заглавную букву.", "Ошибка валидации пароля", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (!hasLower)
            {
                MessageBox.Show("Пароль должен содержать хотя бы одну строчную букву.", "Ошибка валидации пароля", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (!hasDigit)
            {
                MessageBox.Show("Пароль должен содержать хотя бы одну цифру.", "Ошибка валидации пароля", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (!hasSpecial)
            {
                MessageBox.Show("Пароль должен содержать хотя бы один спецсимвол (например, !, @, #, $).", "Ошибка валидации пароля", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы уверены, что хотите удалить этого пользователя?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                try
                {
                    _userRepo.DeleteUser(_user.UserId);
                    DialogResult = true;
                    Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении пользователя: {ex.Message}\n{ex.InnerException?.Message}");
                }
            }
        }

        private void PhoneTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null) return;

            string currentText = textBox.Text;
            int caretIndex = textBox.CaretIndex;
            string newText = "";
            int newCaretIndex = caretIndex;

            string cleanDigits = new string(currentText.Where(char.IsDigit).ToArray());

            // Если текст пуст или начинается с 8, заменяем на 7
            if (cleanDigits.StartsWith("8"))
            {
                cleanDigits = "7" + cleanDigits.Substring(1);
            }

            // Формируем отформатированную строку с маской
            newText += "+";
            if (cleanDigits.Length > 0) newText += cleanDigits[0]; else newText += "7"; // Первая цифра (всегда 7)
            newText += "(";
            for (int i = 1; i < 4; i++)
            {
                if (cleanDigits.Length > i) newText += cleanDigits[i]; else newText += "_";
            }
            newText += ")-";
            for (int i = 4; i < 7; i++)
            {
                if (cleanDigits.Length > i) newText += cleanDigits[i]; else newText += "_";
            }
            newText += "-";
            for (int i = 7; i < 9; i++)
            {
                if (cleanDigits.Length > i) newText += cleanDigits[i]; else newText += "_";
            }
            newText += "-";
            for (int i = 9; i < 11; i++)
            {
                if (cleanDigits.Length > i) newText += cleanDigits[i]; else newText += "_";
            }

            // Ограничиваем ввод 11 цифрами
            if (cleanDigits.Length > 11)
            {
                MessageBox.Show("Номер телефона не может содержать более 11 цифр.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Information);
                newText = PhoneMaskPlaceholder; // Сбрасываем маску или корректируем
                cleanDigits = cleanDigits.Substring(0, 11);
                // Переформируем newText на основе обрезанного cleanDigits
                newText = "+";
                if (cleanDigits.Length > 0) newText += cleanDigits[0]; else newText += "7"; // Первая цифра (всегда 7)
                newText += "(";
                for (int i = 1; i < 4; i++)
                {
                    if (cleanDigits.Length > i) newText += cleanDigits[i]; else newText += "_";
                }
                newText += ")-";
                for (int i = 4; i < 7; i++)
                {
                    if (cleanDigits.Length > i) newText += cleanDigits[i]; else newText += "_";
                }
                newText += "-";
                for (int i = 7; i < 9; i++)
                {
                    if (cleanDigits.Length > i) newText += cleanDigits[i]; else newText += "_";
                }
                newText += "-";
                for (int i = 9; i < 11; i++)
                {
                    if (cleanDigits.Length > i) newText += cleanDigits[i]; else newText += "_";
                }
            }

            // Пересчитываем позицию курсора
            int oldFormattedLength = new string(textBox.Text.Where(char.IsDigit).ToArray()).Length; // Количество цифр в старом форматированном тексте
            int newFormattedLength = cleanDigits.Length; // Количество цифр в новом форматированном тексте

            if (newFormattedLength > oldFormattedLength) // Если добавили символ
            {
                newCaretIndex = caretIndex + (newText.Length - currentText.Length);
            }
            else if (newFormattedLength < oldFormattedLength) // Если удалили символ
            {
                newCaretIndex = caretIndex - (currentText.Length - newText.Length);
            }
            else // Не изменили количество цифр, но могли изменить позицию
            {
                newCaretIndex = caretIndex;
            }

            // Запоминаем текущее выделение
            int selectionStart = textBox.SelectionStart;
            int selectionLength = textBox.SelectionLength;

            textBox.Text = newText;

            // Восстанавливаем выделение и позицию курсора
            if (textBox.Text.Length >= selectionStart + selectionLength)
            {
                textBox.Select(selectionStart, selectionLength);
            }
            else
            {
                textBox.CaretIndex = newCaretIndex > newText.Length ? newText.Length : newCaretIndex;
            }
        }

        private void PhoneTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null) return;

            string cleanText = new string(textBox.Text.Where(char.IsDigit).ToArray());

            // Проверяем на полный формат +7(XXX)-XXX-XX-XX (11 цифр)
            if (cleanText.Length != 11 || !cleanText.StartsWith("7"))
            {
                // MessageBox.Show("Некорректный формат телефона. Ожидается: +7(XXX)-XXX-XX-XX", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                // textBox.Focus(); // Возвращаем фокус для исправления
                // return; // Важно остановить дальнейшую обработку
            }
        }

        private void EmailTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null) return;

            string email = textBox.Text;
            // Простая регулярка для email
            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

            if (!Regex.IsMatch(email, emailPattern))
            {
                // MessageBox.Show("Некорректный формат Email. Пример: example@domain.com", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                // textBox.Focus(); // Возвращаем фокус для исправления
                // return; // Важно остановить дальнейшую обработку
            }
        }
    }
} 