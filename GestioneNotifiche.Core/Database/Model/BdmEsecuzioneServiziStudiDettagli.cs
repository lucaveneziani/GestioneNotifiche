using System;
using System.Collections.Generic;

namespace GestioneNotifiche.Core.Database.Model;

public partial class BdmEsecuzioneServiziStudiDettagli
{
    public int IdEsecuzioneDettaglio { get; set; }

    public int IdEsecuzione { get; set; }

    public string Utente { get; set; } = null!;

    public bool MailSent { get; set; }
}
