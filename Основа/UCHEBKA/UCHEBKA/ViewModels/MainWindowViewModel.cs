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
    private readonly SectionEvent _eventService;

    [ObservableProperty]
    private ObservableCollection<Event> _events = new();

    public MainWindowViewModel(EventService eventService)
    {
        _eventService = eventService;
        LoadEvents();
    }

    private void LoadEvents()
    {
        var events = _eventService.GetEventsWithSections();
        Events = new ObservableCollection<Event>(events);
    }

    [RelayCommand]
    private void ApplyFilter()
    {
        // Фильтрация по дате/направлению
    }
}
