using GestioneNotifiche.Core.Database.Model;

namespace GestioneNotifiche.Core.Database.Repository
{
    public  class BdmEsecuzioneServiziStudiDettagliRepository : IBdmEsecuzioneServiziStudiDettagliRepository
    {
        private readonly BdmonitorContext _dbContext;
        public BdmEsecuzioneServiziStudiDettagliRepository(BdmonitorContext dbContext)
        {
            _dbContext = dbContext;
        }
        public void InsertEsecuzioneServiziStudiDettagli(BdmEsecuzioneServiziStudiDettagli dett)
        {
            _dbContext.BdmEsecuzioneServiziStudiDettaglis.Add(dett);
            _dbContext.SaveChanges();
        }
    }
}
