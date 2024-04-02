using GestioneNotifiche.Core.Database.Model;

namespace GestioneNotifiche.Core.Database.Repository
{
    public  class BdmEsecuzioneReminderImpegniDettagliRepository : IBdmEsecuzioneReminderImpegniDettagliRepository
    {
        private readonly BdmonitorContext _dbContext;
        public BdmEsecuzioneReminderImpegniDettagliRepository(BdmonitorContext dbContext)
        {
            _dbContext = dbContext;
        }
        public void Add(BdmEsecuzioneReminderImpegniDettagli record)
        {
            _dbContext.BdmEsecuzioneReminderImpegniDettaglis.Add(record);
            _dbContext.SaveChanges();
        }
    }
}
