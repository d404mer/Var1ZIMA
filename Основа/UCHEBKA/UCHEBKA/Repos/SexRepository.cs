using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCHEBKA.Models;

namespace UCHEBKA.Repos
{
    /// <summary>
    /// Репозиторий для работы с полами пользователей
    /// </summary>
    public class SexRepository
    {
        private readonly UchebnayaLeto2025Context _db;

        /// <summary>
        /// Инициализирует новый экземпляр репозитория полов
        /// </summary>
        /// <param name="db">Контекст базы данных</param>
        public SexRepository(UchebnayaLeto2025Context db)
        {
            _db = db;
        }

        /// <summary>
        /// Получает все варианты пола из базы данных
        /// </summary>
        /// <returns>Список всех вариантов пола</returns>
        public List<Sex> GetAllSexes()
        {
            return _db.Sexes.ToList();
        }
    }
}
