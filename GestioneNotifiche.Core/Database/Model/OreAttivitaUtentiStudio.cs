using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestioneNotifiche.Core.Database.Model
{
    public class OreAttivitaUtentiStudio
    {
        public string Username { get; set; } = "";
        public TimeOnly Orario_Inizio_Lavoro { get; set; }
        public TimeOnly Orario_Fine_Lavoro { get; set; }
        public int MinutiDaLavorare { get; set; }
        public TimeOnly Orario_Inizio { get; set; }
        public TimeOnly Orario_Fine { get; set; }
        public int MinutiLavorati { get; set; }
        public string Descrizione { get; set; } = "";
        public DateTime Date_Inizio { get; set; }
        public DateTime Date_Fine { get; set; }
    }
}
