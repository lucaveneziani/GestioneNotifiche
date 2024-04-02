using GestioneNotifiche.Core.Database.Model;

namespace GestioneNotifiche.Core.Database.Repository
{
    public interface IBdmEsecuzioneReminderImpegniRepository
    {
        public void Add(BdmEsecuzioneReminderImpegni record);
        public void Remove(BdmEsecuzioneReminderImpegni record);
    }
}
