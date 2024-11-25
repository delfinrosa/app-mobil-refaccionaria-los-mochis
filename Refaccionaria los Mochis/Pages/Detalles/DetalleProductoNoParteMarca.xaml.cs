using CapaEntidad;
using Refaccionaria_los_Mochis.Pages.Reportes;
using Refaccionaria_los_Mochis.Pages;

namespace Refaccionaria_los_Mochis.Pages.Detalles;

public partial class DetalleProductoNoParteMarca : ContentPage
{
	public DetalleProductoNoParteMarca(Dictionary<string, object> navigationParameter)
	{
		InitializeComponent();       
        if (navigationParameter != null && navigationParameter.ContainsKey("producto"))
        {
            var producto = navigationParameter["producto"] as Producto;
            if (producto != null)
            {
                BindingContext = producto;
            }
        }
	}

    private void btnReporte_Clicked(object sender, EventArgs e)
    {
        App.Current.MainPage = new ReporteNoParteBoton();

    }
    private void btnRegresar_Clicked(object sender, EventArgs e)
    {
        App.Current.MainPage = new PrincipalPage();

    }


}