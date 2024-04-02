using GestioneNotifiche.Core.Database.Model;
using Microsoft.EntityFrameworkCore;

namespace GestioneNotifiche.Core.Database.Repository
{
    public class ImpegniReminderRepository : IImpegniReminderRepository
    {
        private readonly BdmonitorContext _dbContext;
        public ImpegniReminderRepository(BdmonitorContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<ImpegniReminder> GetImpegniReminder(DateTime data)
        {
            var res = _dbContext.ImpegniReminders.FromSqlRaw<ImpegniReminder>("spGetImpegniReminder {0}", data).ToList();
            return res;
        }
    }
}
