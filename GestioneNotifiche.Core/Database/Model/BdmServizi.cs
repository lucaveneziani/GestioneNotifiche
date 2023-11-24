using System;
using System.Collections.Generic;

namespace GestioneNotifiche.Core.Database.Model;

public partial class BdmServizi
{
    public int IdServizio { get; set; }

    public string Descrizione { get; set; } = null!;
}
