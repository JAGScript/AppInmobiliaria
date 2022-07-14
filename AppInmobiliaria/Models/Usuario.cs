using System;
using System.Collections.Generic;
using System.Text;

namespace AppInmobiliaria.Models
{
    public class Usuario
    {
        public int us_id { get; set; }

        public int us_rol_id { get; set; }

        public string us_nombre { get; set; }

        public string us_identificacion { get; set; }

        public string us_correo { get; set; }

        public string us_login { get; set; }

        public string us_contrasena { get; set; }

        public int us_estado { get; set; }
    }
}
