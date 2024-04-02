namespace GestioneNotifiche.Core.Database.Model;

public partial class BdmEsecuzioneReminderImpegniDettagli
{
    public int IdEsecuzioneDettaglio { get; set; }

    public int IdEsecuzione { get; set; }

    public int IdImpegno { get; set; }

    public int IdUtente { get; set; }

    public int ContReminder { get; set; }
}
