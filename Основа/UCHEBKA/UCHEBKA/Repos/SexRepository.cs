using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCHEBKA.Models;

namespace UCHEBKA.Repos
{
    public class SexRepository
    {
        private readonly UchebnayaLeto2025Context _db;

        public SexRepository(UchebnayaLeto2025Context db)
        {
            _db = db;
        }

        public List<Sex> GetAllSexes()
        {
            return _db.Sexes.ToList();
        }
    }
}
