using GestioneNotifiche.Core.Database.Model;

namespace GestioneNotifiche.Core.Database.Repository
{
    public interface IImpegniReminderRepository
    {
        public List<ImpegniReminder> GetImpegniReminder(DateTime data);
    }
}
