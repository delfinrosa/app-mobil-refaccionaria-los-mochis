using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class Compra
    {
        public int IdCompra { get; set; }
        public string IdFactura { get; set; }
        public string Estatus { get; set; }
        public DateTime Fecha { get; set; }
        public string Nota { get; set; }
        public Proveedor Proveedor { get; set; }
        public Usuario Usuario { get; set; }
    }

}
