using System;
using System.Collections.Generic;
using System.Text;

namespace AppInmobiliaria.Models
{
    public class Propiedad
    {
        public int pr_id { get; set; }

        public int pr_caracteristica_id { get; set; }

        public int pr_barrio_id { get; set; }

        public int pr_tipo_id { get; set; }

        public int pr_propietario_id { get; set; }

        public int pr_usuario_id { get; set; }

        public decimal pr_precio { get; set; }

        public DateTime pr_fecha_registro { get; set; }

        public string pr_foto_principal { get; set; }

        public int pr_estado { get; set; }

        public string tp_nombre { get; set; }
    }
}
