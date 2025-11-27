using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCHEBKA.Models;

namespace UCHEBKA.Repos
{
    /// <summary>
    /// Репозиторий для работы с секциями
    /// </summary>
    public class SectionRepository
    {
        private readonly UchebnayaLeto2025Context _db;

        /// <summary>
        /// Инициализирует новый экземпляр репозитория секций
        /// </summary>
        /// <param name="db">Контекст базы данных</param>
        public SectionRepository(UchebnayaLeto2025Context db)
        {
            _db = db;
        }

        /// <summary>
        /// Получает все секции из базы данных
        /// </summary>
        /// <returns>Список всех секций</returns>
        public List<Section> GetAllSections()
        {
            return _db.Sections.ToList();
        }
    }
}
