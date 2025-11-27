using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using UCHEBKA.Models;
using System.IO;

namespace UCHEBKA.Repos
{
    /// <summary>
    /// Репозиторий для работы с мероприятиями
    /// </summary>
    public class EventRepository
    {
        private readonly UchebnayaLeto2025Context _db;
        private const string BaseImagePath = "Images\\Events\\";

        /// <summary>
        /// Инициализирует новый экземпляр репозитория мероприятий
        /// </summary>
        /// <param name="db">Контекст базы данных</param>
        public EventRepository(UchebnayaLeto2025Context db)
        {
            _db = db;
        }

        /// <summary>
        /// Получает корневой путь проекта
        /// </summary>
        /// <returns>Корневой путь проекта</returns>
        private string GetProjectRootPath()
        {
            var currentDir = AppDomain.CurrentDomain.BaseDirectory;
            var projectRoot = Path.GetFullPath(Path.Combine(currentDir, "..\\..\\..\\"));
            return projectRoot;
        }

        /// <summary>
        /// Получает все мероприятия с информацией о секциях
        /// </summary>
        /// <returns>Список всех мероприятий</returns>
        public List<Event> GetAllEvents()
        {
            return _db.Events
                .Include(e => e.SectionEvents)
                .ThenInclude(se => se.FkSec)
                .ToList();
        }

        /// <summary>
        /// Получает мероприятия по идентификатору секции
        /// </summary>
        /// <param name="sectionId">Идентификатор секции</param>
        /// <returns>Список мероприятий указанной секции</returns>
        public List<Event> GetEventsBySection(int sectionId)
        {
            return _db.Events
                .Where(e => e.SectionEvents.Any(se => se.FkSec.SecId == sectionId))
                .ToList();
        }

        /// <summary>
        /// Получает мероприятия по дате
        /// </summary>
        /// <param name="date">Дата мероприятий</param>
        /// <returns>Список мероприятий на указанную дату</returns>
        public List<Event> GetEventsByDate(DateTime? date)
        {
            if (!date.HasValue)
                return new List<Event>();

            return _db.Events
                .Include(e => e.SectionEvents)
                    .ThenInclude(se => se.FkSec)
                .Where(e => e.EventStartTime.HasValue &&
                            e.EventStartTime.Value.Date == date.Value.Date)
                .ToList();
        }

        /// <summary>
        /// Получает полный путь к изображению мероприятия
        /// </summary>
        /// <param name="fileName">Имя файла изображения</param>
        /// <returns>Полный путь к изображению</returns>
        public string GetDisplayImagePath(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return Path.Combine(GetProjectRootPath(), BaseImagePath, "2.jpeg");

            var fullPath = Path.Combine(GetProjectRootPath(), BaseImagePath, fileName);

            return fullPath;
        }

        /// <summary>
        /// Добавляет новое мероприятие в базу данных
        /// </summary>
        /// <param name="newEvent">Новое мероприятие</param>
        public void AddEvent(Event newEvent)
        {
            _db.Events.Add(newEvent);
            _db.SaveChanges();
        }

        /// <summary>
        /// Обновляет существующее мероприятие
        /// </summary>
        /// <param name="eventToUpdate">Мероприятие для обновления</param>
        public void UpdateEvent(Event eventToUpdate)
        {
            _db.Events.Update(eventToUpdate);
            _db.SaveChanges();
        }
        /// <summary>
        /// Получает следующий доступный идентификатор для мероприятия
        /// </summary>
        /// <returns>Следующий идентификатор мероприятия</returns>
        public long GetNextEventId()
        {
            return _db.Events.Any() ? _db.Events.Max(e => e.EventId) + 1 : 1;
        }

        /// <summary>
        /// Удаляет мероприятие из базы данных
        /// </summary>
        /// <param name="eventId">Идентификатор мероприятия</param>
        /// <returns>True, если удаление прошло успешно, иначе False</returns>
        public bool DeleteEvent(long eventId)
        {
            try
            {
                var eventToDelete = _db.Events.Find(eventId);
                if (eventToDelete != null)
                {
                    _db.SectionEvents.RemoveRange(_db.SectionEvents.Where(se => se.FkEventId == eventId));

                    _db.Events.Remove(eventToDelete);
                    _db.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при удалении мероприятия: {ex.Message}");
                return false;
            }
        }
    }
}