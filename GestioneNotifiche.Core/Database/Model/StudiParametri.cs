namespace GestioneNotifiche.Core.Database.Model
{
    public class StudiParametri
    {
        public int IdParametroServizio { get; set; }
        public string Descrizione { get; set; } = "";
        public int IdStudio { get; set; }
        public string Ragione_Sociale { get; set; } = "";
        public string Partita_Iva { get; set; } = "";
        public string Codice_Fiscale { get; set; } = "";
        public DateOnly DataExec { get; set; }
        public string Valore { get; set; } = "";
        public string TimeZone { get; set; } = "";
    }
}
