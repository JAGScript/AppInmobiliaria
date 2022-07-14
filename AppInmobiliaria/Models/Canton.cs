using System;
using System.Collections.Generic;
using System.Text;

namespace AppInmobiliaria.Models
{
    public class Canton
    {
        public int ca_id { get; set; }

        public int ca_provincia_id { get; set; }

        public string ca_codigo { get; set; }

        public string ca_nombre { get; set; }

        public int ca_estado { get; set; }
    }
}
