using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using UCHEBKA.Models;

namespace UCHEBKA.Repos
{
    public class ActivityRepository
    {
        private readonly UchebnayaLeto2025Context _db;

        public ActivityRepository(UchebnayaLeto2025Context db)
        {
            _db = db;
        }

        public List<ActivityEvent> GetActivitiesForJury(long juryId)
        {
            return _db.EventJuries
                .Where(ej => ej.FkJuryId == juryId)
                .Include(ej => ej.FkActivity)
                    .ThenInclude(a => a.ActivityEvents)
                .Include(ej => ej.FkActivity)
                    .ThenInclude(a => a.ActivityEvents)
                        .ThenInclude(ae => ae.FkEvent)
                .SelectMany(ej => ej.FkActivity.ActivityEvents)
                .Include(ae => ae.FkActivity)
                .Include(ae => ae.FkEvent)
                .ToList();
        }
    }
}