using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestioneNotifiche.Core.Database.Model
{
    public class OreAttivitaUtentiStudio
    {
        public DateOnly DataAttivita { get; set; }
        public string GiornoAttivita { get; set; } = "";
        public string Utente { get; set; } = "";
        public int MinutiDaLavorare { get; set; }
        public int MinutiLavorati { get; set; }
    }
}
