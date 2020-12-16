using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace waterskibaan
{
    class boekingen
    {
        public int id { get; set; }
        public int klant_Id { get; set; }
        public string datum { get; set; }
        public bool online_boeking { get; set; }
        public List<boekingsRegels> boekingsregels { get; set; }
    }

    class contacten
    {
        public int id { get; set; }
        public string type { get; set; }
        public string naam { get; set; }
    }

    class klanten
    {
        public int id { get; set; }
        public string type { get; set; }
        public string displaynaam { get; set; }
        public string naam { get; set; }
        public string adres { get; set; }
        public string plaats { get; set; }
    }

    class startmomenten
    {
        public int id { get; set; }
        public int startmomentgroep_id { get; set; }
        public string datetime { get; set; }
    }
    
    class boekingsRegels
    {
        public int id { get; set; }
        public int locatie__id { get; set; }
        public int aantal { get; set; }
        public DateTime begin { get; set; }
        public DateTime eind { get; set; }
        public int minimum_aantal { get; set; }
        public string beschrijving { get; set; }
        public int product_id { get; set; }
    }
}
