using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace UCHEBKA.Views.Helpers
{
    /// <summary>
    /// Конвертер для преобразования булевого значения в текст регистрации
    /// </summary>
    public class BoolToRegisteredConverter : IValueConverter
    {
        /// <summary>
        /// Преобразует булевое значение в текст статуса регистрации
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? "Зарегистрирован" : "Не зарегистрирован";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Конвертер для преобразования булевого значения в текст модерации
    /// </summary>
    public class BoolToModeratingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? "Модератор" : "Не модератор";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Конвертер для преобразования булевого значения в видимость элемента
    /// </summary>
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
} 