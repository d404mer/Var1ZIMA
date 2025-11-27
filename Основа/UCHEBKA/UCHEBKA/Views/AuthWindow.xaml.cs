using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using UCHEBKA.Repos;
using UCHEBKA.Models;
using System.Windows.Threading;

namespace UCHEBKA
{
    /// <summary>
    /// Окно авторизации пользователя с капчей и блокировкой
    /// </summary>
    public partial class AuthWindow : Window
    {
        private string currentCaptcha;
        private readonly Random random = new Random();
        private readonly UserRepository _usRepo;
        private User _currUser;


        private static DateTime? _blockTime = null;
        private static int _failedAttempts = 0;
        /// <summary>
        /// Инициализирует новый экземпляр окна авторизации
        /// </summary>
        public AuthWindow()
        {
            var db = new UchebnayaLeto2025Context();
            _usRepo = new UserRepository(db);

            InitializeComponent();
            CaptchaInput.Foreground = Brushes.Gray;
            GenerateNewCaptcha();
            this.Loaded += (s, e) => GenerateNewCaptcha();
        }

        /// <summary>
        /// Генерирует новую капчу
        /// </summary>
        private void GenerateNewCaptcha()
        {
            currentCaptcha = "";
            for (int i = 0; i < 4; i++)
            {
                currentCaptcha += random.Next(0, 10).ToString();
            }

            CaptchaCanvas.Children.Clear();

            AddNoiseToCanvas();

            DrawCaptchaText();

            AddInterferenceLines();
        }

        /// <summary>
        /// Добавляет шум на холст капчи
        /// </summary>
        private void AddNoiseToCanvas()
        {
            int dotCount = 100;
            double canvasWidth = CaptchaCanvas.ActualWidth;
            double canvasHeight = CaptchaCanvas.ActualHeight;

            for (int i = 0; i < dotCount; i++)
            {
                Ellipse dot = new Ellipse
                {
                    Width = random.Next(1, 3),
                    Height = random.Next(1, 3),
                    Fill = new SolidColorBrush(Color.FromRgb(
                        (byte)random.Next(150, 220),
                        (byte)random.Next(150, 220),
                        (byte)random.Next(150, 220)))
                };

                Canvas.SetLeft(dot, random.Next(0, (int)canvasWidth));
                Canvas.SetTop(dot, random.Next(0, (int)canvasHeight));
                CaptchaCanvas.Children.Add(dot);
            }
        }

        private void DrawCaptchaText()
        {
            double xPos = 10;
            double yPos = CaptchaCanvas.ActualHeight / 3;
            double fontSize = 24;

            for (int i = 0; i < currentCaptcha.Length; i++)
            {
                TextBlock charBlock = new TextBlock
                {
                    Text = currentCaptcha[i].ToString(),
                    FontSize = fontSize,
                    FontWeight = FontWeights.Bold,
                    Foreground = Brushes.DarkBlue,
                    RenderTransform = new TransformGroup
                    {
                        Children = new TransformCollection
                        {
                            new RotateTransform(random.Next(-10, 11)),
                            new TranslateTransform(random.Next(-2, 3), random.Next(-2, 3))
                        }
                    }
                };

                Canvas.SetLeft(charBlock, xPos);
                Canvas.SetTop(charBlock, yPos + random.Next(-5, 6));
                CaptchaCanvas.Children.Add(charBlock);

                xPos += fontSize - 5;
            }
        }

        private void AddInterferenceLines()
        {
            for (int i = 0; i < 2; i++)
            {
                Line line = new Line
                {
                    X1 = random.Next(0, (int)CaptchaCanvas.ActualWidth / 2),
                    Y1 = random.Next(0, (int)CaptchaCanvas.ActualHeight),
                    X2 = random.Next((int)CaptchaCanvas.ActualWidth / 2, (int)CaptchaCanvas.ActualWidth),
                    Y2 = random.Next(0, (int)CaptchaCanvas.ActualHeight),
                    Stroke = Brushes.Gray,
                    StrokeThickness = 1,
                    Opacity = 0.5
                };
                CaptchaCanvas.Children.Add(line);
            }
        }

        private void RefreshCaptchaButton_Click(object sender, RoutedEventArgs e)
        {
            GenerateNewCaptcha();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем блокировку
            if (_blockTime.HasValue)
            {
                var remainingTime = (DateTime.Now - _blockTime.Value).TotalSeconds;
                if (remainingTime < 10)
                {
                    MessageBox.Show($"Система заблокирована. Попробуйте через {10 - (int)remainingTime} секунд",
                                  "Блокировка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                else
                {
                    // Разблокируем после 10 секунд
                    _blockTime = null;
                    _failedAttempts = 0;
                }
            }

            // CAPTCHA
            if (CaptchaInput.Text != currentCaptcha)
            {
                HandleFailedAttempt("Неверная CAPTCHA! Введите показанные цифры.");
                GenerateNewCaptcha();
                CaptchaInput.Clear();
                return;
            }

            // ID
            if (!int.TryParse(UserIDTextBox.Text, out int userId))
            {
                HandleFailedAttempt("Некорректный ID пользователя. Введите число.");
                return;
            }

            // пароль
            if (string.IsNullOrWhiteSpace(UserPasswordBox.Password))
            {
                HandleFailedAttempt("Введите пароль.");
                return;
            }

            try
            {
                _currUser = _usRepo.Auth(userId, UserPasswordBox.Password);
                if (_currUser == null)
                {
                    HandleFailedAttempt("Неверные учетные данные");
                    return;
                }

                // Успешная авторизация - сбрасываем счетчик
                _failedAttempts = 0;
                _usRepo.SaveCurrentUser(userId, UserPasswordBox.Password);
                OpenRoleBasedWindow();
                DialogResult = true;
                this.Close();
            }
            catch
            {
                HandleFailedAttempt("Ошибка при авторизации");
                return;
            }
        }

        private void HandleFailedAttempt(string errorMessage)
        {
            _failedAttempts++;

            if (_failedAttempts >= 3)
            {
                _blockTime = DateTime.Now;
                MessageBox.Show($"{errorMessage}\n\nСлишком много попыток. Система заблокирована на 10 секунд.",
                              "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                BlockUI();
                StartUnlockTimer();
            }
            else
            {
                MessageBox.Show($"{errorMessage}\n\nОсталось попыток: {3 - _failedAttempts}",
                              "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BlockUI()
        {
            LogINButton.IsEnabled = false;
            UserIDTextBox.IsEnabled = false;
            UserPasswordBox.IsEnabled = false;
            CaptchaInput.IsEnabled = false;
            RefreshCaptchaButton.IsEnabled = false;
            BlockTimerText.Visibility = Visibility.Visible;
            BlockTimerText.Text = $"Система заблокирована на 10 секунд";
        }

        private void UnblockUI()
        {
            LogINButton.IsEnabled = true;
            UserIDTextBox.IsEnabled = true;
            UserPasswordBox.IsEnabled = true;
            CaptchaInput.IsEnabled = true;
            RefreshCaptchaButton.IsEnabled = true;
            GenerateNewCaptcha();
        }

        private void StartUnlockTimer()
        {
            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            int secondsLeft = 10;

            timer.Tick += (s, args) =>
            {
                secondsLeft--;
                BlockTimerText.Text = $"Система заблокирована. До разблокировки: {secondsLeft} сек.";

                if (secondsLeft <= 0)
                {
                    timer.Stop();
                    UnblockUI();
                    BlockTimerText.Visibility = Visibility.Collapsed;
                }
            };

            timer.Start();
        }

        private void OpenRoleBasedWindow()
        {
            var role = _usRepo.GetUserRole(_currUser.UserId);

            Window window = role switch
            {
                "модератор" => new AdminWindow(),
                "жюри" => new JuryWindow(),
                "организатор" => new OrgWindow(),
                _ => new ParticipantWindow()
            };

            window.Show();
        }

        private void CaptchaInput_GotFocus(object sender, RoutedEventArgs e)
        {
            CaptchaInput.Foreground = Brushes.Black;
            if (CaptchaInput.Text == "Введите каптчу")
            {
                CaptchaInput.Text = "";
            }
        }

        private void CaptchaInput_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CaptchaInput.Text))
            {
                CaptchaInput.Text = "Введите каптчу";
                CaptchaInput.Foreground = Brushes.Gray;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}