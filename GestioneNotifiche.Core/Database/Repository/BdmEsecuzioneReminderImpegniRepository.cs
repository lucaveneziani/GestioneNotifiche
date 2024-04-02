using GestioneNotifiche.Core.Database.Model;

namespace GestioneNotifiche.Core.Database.Repository
{
    public class BdmEsecuzioneReminderImpegniRepository : IBdmEsecuzioneReminderImpegniRepository
    {
        private readonly BdmonitorContext _dbContext;
        public BdmEsecuzioneReminderImpegniRepository(BdmonitorContext dbContext)
        {
            _dbContext = dbContext;
        }
        public void Add(BdmEsecuzioneReminderImpegni record)
        {
            _dbContext.BdmEsecuzioneReminderImpegnis.Add(record);
            _dbContext.SaveChanges();
        }
        public void Remove(BdmEsecuzioneReminderImpegni record)
        {
            _dbContext.BdmEsecuzioneReminderImpegnis.Remove(record);
            _dbContext.SaveChanges();
        }
    }
}
