using GestioneNotifiche.Core.Database.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestioneNotifiche.Core.Database.Repository
{
    public class BdmAttivitaRepository : IBdmAttivitaRepository
    {
        private readonly BdmonitorContext _dbContext;
        public BdmAttivitaRepository(BdmonitorContext dbContext)
        {
            _dbContext = dbContext;
        }
        public List<OreAttivitaUtentiStudio> GetOreAttivitaUtentiStudio(int idStudio, DateOnly dataDa)
        {
            var res = _dbContext.OreAttivitaUtentiStudios.FromSqlRaw<OreAttivitaUtentiStudio>("spGetOreAttivitaUtentiStudio {0},{1}", idStudio, dataDa).ToList();
            return res;
        }
    }
}
