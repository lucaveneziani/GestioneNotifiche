using GestioneNotifiche.Core.Database.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestioneNotifiche.Core.Database.Repository
{
    public interface IStudiParametriRepository
    {
        public List<StudiParametri> GetStudiParametri(bool abilitato);
    }
}
