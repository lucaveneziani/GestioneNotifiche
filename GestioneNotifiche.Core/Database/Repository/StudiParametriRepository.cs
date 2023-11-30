using GestioneNotifiche.Core.Database.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestioneNotifiche.Core.Database.Repository
{
    public class StudiParametriRepository : IStudiParametriRepository
    {
        private readonly BdmonitorContext _dbContext;
        public StudiParametriRepository(BdmonitorContext dbContext)
        {
            _dbContext = dbContext;
        }
        public List<StudiParametri> GetStudiParametri(bool abilitato)
        {
            var res = _dbContext.StudiParametris.FromSqlRaw<StudiParametri>("spGetStudiParametri {0}", Convert.ToInt32(abilitato).ToString()).ToList();
            return res;
        }
    }
}
