using GestioneNotifiche.Core.Database.Model;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace GestioneNotifiche.Core.Database.Repository
{
    public class BdmEsecuzioneServiziStudiRepository : IBdmEsecuzioneServiziStudiRepository
    {
        private readonly BdmonitorContext _dbContext;
        public BdmEsecuzioneServiziStudiRepository(BdmonitorContext dbContext)
        {
            _dbContext = dbContext;
        }
        public int InsertEsecuzioneServiziStudi(int numUtenti, int idStudio, string timeZone, int idServizio)
        {
            int idEsecuzione = 0;

            if (numUtenti > 0)
            {
                var esecServiziStudi = new BdmEsecuzioneServiziStudi()
                {
                    IdServizio = idServizio,
                    IdStudio = idStudio,
                    DataExec = DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(timeZone)))
                };
                _dbContext.BdmEsecuzioneServiziStudis.Add(esecServiziStudi);
                _dbContext.SaveChanges();
                idEsecuzione = esecServiziStudi.IdEsecuzione;
            }

            return idEsecuzione;
        }
        public string ClearDbTable(int giorniBackup)
        {
            var spOutput = GetSqlParameterOutput("@OutMessage", System.Data.SqlDbType.VarChar);
            spOutput.Size = int.MaxValue;
            var parGiorniBackup = GetSqlParameter("@giorniBackup", giorniBackup);

            var liParams = new object[]
            {
                parGiorniBackup,
                spOutput
            };

            _dbContext.Database.ExecuteSqlRaw("EXEC spDeleteEsecuzioneServizi @giorniBackup, @OutMessage OUT", liParams);
            return spOutput.Value.ToString();
        }
        protected SqlParameter GetSqlParameterOutput(string paramName, System.Data.SqlDbType type)
        {
            return new SqlParameter
            {
                ParameterName = paramName,
                SqlDbType = type,
                Direction = System.Data.ParameterDirection.Output
            };
        }
        protected SqlParameter GetSqlParameter(string paramName, object value)
        {
            return new SqlParameter(paramName, value ?? DBNull.Value);
        }
    }
}
