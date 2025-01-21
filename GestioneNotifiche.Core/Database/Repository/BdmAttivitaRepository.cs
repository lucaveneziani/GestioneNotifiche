using GestioneNotifiche.Core.Database.Model;
using Microsoft.EntityFrameworkCore;

namespace GestioneNotifiche.Core.Database.Repository
{
    public class BdmAttivitaRepository : IBdmAttivitaRepository
    {
        private readonly BdmonitorContext _dbContext;
        public BdmAttivitaRepository(BdmonitorContext dbContext)
        {
            _dbContext = dbContext;
        }
        public List<OreAttivitaUtentiStudio> GetOreAttivitaUtentiStudio(int idStudio, DateOnly dataDa, DateOnly dataA)
        {

            var res = _dbContext.OreAttivitaUtentiStudios.FromSqlRaw<OreAttivitaUtentiStudio>("spGetOreAttivitaUtentiStudio {0},{1},{2}", idStudio, dataDa, dataA).ToList();
            return res;
        }
    }
}
