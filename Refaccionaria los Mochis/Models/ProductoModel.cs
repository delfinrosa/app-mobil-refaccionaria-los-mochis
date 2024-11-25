using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapaEntidad;
using Refaccionaria_los_Mochis.Generic;
namespace Refaccionaria_los_Mochis.Models
{
    public class ProductoModel : BaseBinding
    {
        private List<Producto> _lista;

        public List<Producto> lista
        {
            get
            {
                return _lista;
            }
            set
            {
                SetValue(ref _lista, value);
            }
        }

    }
}
