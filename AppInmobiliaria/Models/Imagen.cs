using System;
using System.Collections.Generic;
using System.Text;

namespace AppInmobiliaria.Models
{
    public class Imagen
    {
        public int im_id { get; set; }

        public int im_propiedad_id { get; set; }

        public string im_nombre { get; set; }

        public int im_estado { get; set; }
    }
}
