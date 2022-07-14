using System;
using System.Collections.Generic;
using System.Text;

namespace AppInmobiliaria.Models
{
    public class Caracteristica
    {
        public int ca_id { get; set; }

        public decimal ca_metros { get; set; }

        public int ca_plantas { get; set; }

        public int ca_banios { get; set; }

        public int ca_habitaciones { get; set; }

        public int ca_parqueaderos { get; set; }

        public string ca_servicios { get; set; }

        public string ca_otros { get; set; }
    }
}
