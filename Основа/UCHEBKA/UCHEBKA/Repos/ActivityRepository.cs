using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using UCHEBKA.Models;

namespace UCHEBKA.Repos
{
    /// <summary>
    /// Репозиторий для работы с активностями
    /// </summary>
    public class ActivityRepository
    {
        private readonly UchebnayaLeto2025Context _db;

        /// <summary>
        /// Инициализирует новый экземпляр репозитория активностей
        /// </summary>
        /// <param name="db">Контекст базы данных</param>
        public ActivityRepository(UchebnayaLeto2025Context db)
        {
            _db = db;
        }
    }
}