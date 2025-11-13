using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using UCHEBKA.Models;
using System.IO;

namespace UCHEBKA.Repos
{
    public class EventRepository
    {
        private readonly UchebnayaLeto2025Context _db;
        private const string BaseImagePath = "Images\\Events\\";

        public EventRepository(UchebnayaLeto2025Context db)
        {
            _db = db;
        }

        private string GetProjectRootPath()
        {
            var currentDir = AppDomain.CurrentDomain.BaseDirectory;
            var projectRoot = Path.GetFullPath(Path.Combine(currentDir, "..\\..\\..\\"));
            return projectRoot;
        }

        public List<Event> GetAllEvents()
        {
            return _db.Events
                .Include(e => e.SectionEvents)
                .ThenInclude(se => se.FkSec)
                .ToList();
        }

        public List<Event> GetEventsBySection(int sectionId)
        {
            return _db.Events
                .Where(e => e.SectionEvents.Any(se => se.FkSec.SecId == sectionId))
                .ToList();
        }

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

        // чёт не так с этой функцией короче, где-то ещё раз вызываю похожую, но эта перекрывает и чёть юблять я короче хуй знает как оно работает. надо рефакторить
        public string GetDisplayImagePath(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return Path.Combine(GetProjectRootPath(), BaseImagePath, "2.jpeg");

            var fullPath = Path.Combine(GetProjectRootPath(), BaseImagePath, fileName);

            return fullPath;
        }

        public void AddEvent(Event newEvent)
        {
            _db.Events.Add(newEvent);
            _db.SaveChanges();
        }

        public void UpdateEvent(Event eventToUpdate)
        {
            _db.Events.Update(eventToUpdate);
            _db.SaveChanges();
        }
        public long GetNextEventId()
        {
            return _db.Events.Any() ? _db.Events.Max(e => e.EventId) + 1 : 1;
        }

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
                // Логирование ошибки
                Console.WriteLine($"Ошибка при удалении мероприятия: {ex.Message}");
                return false;
            }
        }
    }
}