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
using System.Windows.Navigation;
using System.Windows.Shapes;
using UCHEBKA.Models;
using UCHEBKA.Repos;

namespace UCHEBKA.Views
{
    /// <summary>
    /// Interaction logic for EventCard.xaml
    /// </summary>
    public partial class EventCard : UserControl
    {
        private readonly EventRepository _eventRepo;

        /// <summary>
        /// Инициализирует новый экземпляр карточки мероприятия
        /// </summary>
        /// <param name="ev">Мероприятие для отображения</param>
        public EventCard(Event ev)
        {
            InitializeComponent();
            _eventRepo = new EventRepository(new UchebnayaLeto2025Context());
            DataContext = ev;
            
            if (!string.IsNullOrEmpty(ev.EventLogoUrl))
            {
                var imagePath = _eventRepo.GetDisplayImagePath(ev.EventLogoUrl);
                EventImage.Source = new BitmapImage(new Uri(imagePath));
            }
        }
    }
}