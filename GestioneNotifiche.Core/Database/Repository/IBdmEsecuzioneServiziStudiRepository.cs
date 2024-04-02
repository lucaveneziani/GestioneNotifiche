namespace GestioneNotifiche.Core.Database.Repository
{
    public interface IBdmEsecuzioneServiziStudiRepository
    {
        public string ClearDbTable(int giorniBackup);
        public int InsertEsecuzioneServiziStudi(int numUtenti, int idStudio, string timeZone, int idServizio);
    }
}
