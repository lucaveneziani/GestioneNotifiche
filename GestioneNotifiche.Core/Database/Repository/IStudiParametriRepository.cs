using GestioneNotifiche.Core.Database.Model;

namespace GestioneNotifiche.Core.Database.Repository
{
    public interface IStudiParametriRepository
    {
        public List<StudiParametri> GetStudiParametri(bool abilitato);
    }
}
