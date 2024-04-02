namespace GestioneNotifiche.Core.Database.Model;

public partial class BdmEsecuzioneReminderImpegni
{
    public int IdEsecuzione { get; set; }

    public int IdServizio { get; set; }

    public DateTime DataExec { get; set; }
}
