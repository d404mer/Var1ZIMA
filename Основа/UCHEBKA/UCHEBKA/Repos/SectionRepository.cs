using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCHEBKA.Models;

namespace UCHEBKA.Repos
{
    class SectionRepository
    {
        private readonly UchebnayaLeto2025Context _db;

        public SectionRepository(UchebnayaLeto2025Context db)
        {
            _db = db;
        }

        public List<Section> GetAllSections()
        {
            return _db.Sections.ToList();
        }
    }
}
