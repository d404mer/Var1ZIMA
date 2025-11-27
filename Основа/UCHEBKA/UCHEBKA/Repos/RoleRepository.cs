using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCHEBKA.Models;

namespace UCHEBKA.Repos
{
    /// <summary>
    /// Репозиторий для работы с ролями пользователей
    /// </summary>
    public class RoleRepository
    {
        private readonly UchebnayaLeto2025Context _db;

        /// <summary>
        /// Инициализирует новый экземпляр репозитория ролей
        /// </summary>
        /// <param name="db">Контекст базы данных</param>
        public RoleRepository(UchebnayaLeto2025Context db)
        {
            _db = db;
        }

        /// <summary>
        /// Получает все роли из базы данных
        /// </summary>
        /// <returns>Список всех ролей</returns>
        public List<Role> GetAllRoles()
        {
            return _db.Roles.ToList();
        }
    }
}
