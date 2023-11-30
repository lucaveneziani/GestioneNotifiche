using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestioneNotifiche.Core.Database.Repository
{
    public class BdmEsecuzioneServiziStudiRepository : IBdmEsecuzioneServiziStudiRepository
    {
        private readonly BdmonitorContext _dbContext;
        public BdmEsecuzioneServiziStudiRepository(BdmonitorContext dbContext)
        {
            _dbContext = dbContext;
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
