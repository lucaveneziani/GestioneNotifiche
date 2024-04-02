namespace GestioneNotifiche.Core.Database.Model
{
    public class ImpegniReminder
    {
        public int IdStudio { get; set; }
        public int IdUtenti { get; set; }
        public string Email_Registrazione { get; set; } = "";
        public int IdImpegno { get; set; }
        public string Titolo { get; set; } = "";
        public string Descrizione { get; set; } = "";
        public DateTime Data_Ora_Inizio { get; set; }
        public DateTime Data_Ora_Fine { get; set; }
        public DateTime Data_Ricordamelo { get; set; }
        public int ContReminder { get; set; }
        public string TimeZone { get; set; } = "";
    }
}
