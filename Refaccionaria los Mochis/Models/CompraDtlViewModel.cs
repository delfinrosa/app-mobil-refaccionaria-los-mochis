using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Refaccionaria_los_Mochis.Models
{
    public class CompraDtlViewModel
    {
        public string IdProducto { get; set; }
        public bool IsActChecked { get; set; }
        public bool IsCantidadChecked { get; set; }
    }
}
