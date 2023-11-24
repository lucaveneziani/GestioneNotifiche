using GestioneNotifiche.Core.Database.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestioneNotifiche.Core.Database
{
    public class BdmonitorContextRepository : BdmonitorContext
    {
        public BdmonitorContextRepository()
        {
        }

        public BdmonitorContextRepository(string connectionString)
            : base(connectionString)
        {
        }
        public BdmonitorContextRepository(DbContextOptions<BdmonitorContext> options)
            : base(options)
        {
        }

        public List<StudiParametri> GetStudiParametri(bool abilitato)
        {
            var res = StudiParametris.FromSqlRaw<StudiParametri>("spGetStudiParametri {0}", Convert.ToInt32(abilitato).ToString()).ToList();
            return res;
        }
        public List<OreAttivitaUtentiStudio> GetOreAttivitaUtentiStudio(int idStudio, DateOnly dataDa)
        {
            var res = OreAttivitaUtentiStudios.FromSqlRaw<OreAttivitaUtentiStudio>("spGetOreAttivitaUtentiStudio {0},{1}", idStudio, dataDa).ToList();
            return res;
        }
    }
}
