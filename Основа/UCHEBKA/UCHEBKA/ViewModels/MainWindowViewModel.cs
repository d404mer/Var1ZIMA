using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCHEBKA.Models;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel; 
using CommunityToolkit.Mvvm.Input;          
using UchebnayaLeto2025.WPF.Models;       
using UchebnayaLeto2025.WPF.Services;      // Сервис для работы с БД
using UCHEBKA.Services;

namespace UCHEBKA.ViewModels
{
    /// <summary>
    /// Модель представления для главного окна
    /// </summary>
    public class MainWindowViewModel
    {
        private readonly SectionEvent _eventService;

    [ObservableProperty]
    private ObservableCollection<Event> _events = new();

        /// <summary>
        /// Инициализирует новый экземпляр модели представления главного окна
        /// </summary>
        /// <param name="eventService">Сервис для работы с мероприятиями</param>
        public MainWindowViewModel(EventService eventService)
        {
            _eventService = eventService;
            LoadEvents();
        }

        /// <summary>
        /// Загружает мероприятия с секциями
        /// </summary>
        private void LoadEvents()
        {
            var events = _eventService.GetEventsWithSections();
            Events = new ObservableCollection<Event>(events);
        }

        /// <summary>
        /// Применяет фильтр к мероприятиям
        /// </summary>
        [RelayCommand]
        private void ApplyFilter()
        {
            
        }
}
