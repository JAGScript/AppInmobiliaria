using System;
using System.Collections.Generic;
using System.Text;

namespace AppInmobiliaria.Models
{
    public class Barrio
    {
        public int ba_id { get; set; }

        public int ba_parroquia_id { get; set; }

        public string ba_nombre { get; set; }

        public int ba_estado { get; set; }
    }
}
