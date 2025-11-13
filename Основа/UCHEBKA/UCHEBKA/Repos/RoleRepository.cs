using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCHEBKA.Models;

namespace UCHEBKA.Repos
{
    public class RoleRepository
    {
        private readonly UchebnayaLeto2025Context _db;

        public RoleRepository(UchebnayaLeto2025Context db)
        {
            _db = db;
        }

        public List<Role> GetAllRoles()
        {
            return _db.Roles.ToList();
        }
    }
}
