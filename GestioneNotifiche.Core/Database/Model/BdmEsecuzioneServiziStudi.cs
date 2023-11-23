using System;
using System.Collections.Generic;

namespace GestioneNotifiche.Core.Database.Model;

public partial class BdmEsecuzioneServiziStudi
{
    public int IdEsecuzione { get; set; }

    public int IdServizio { get; set; }

    public int IdStudio { get; set; }

    public DateOnly DataExec { get; set; }
}
