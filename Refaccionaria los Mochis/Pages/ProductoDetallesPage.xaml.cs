using CapaEntidad;

namespace Refaccionaria_los_Mochis.Pages;

public partial class ProductoDetallesPage : ContentPage
{
    public ProductoDetallesPage(Dictionary<string, object> navigationParameter)
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

    private void btnRegresar_Clicked(object sender, EventArgs e)
    {
        App.Current.MainPage = new PrincipalPage();

    }

    private void btnLectorCodigo_Clicked(object sender, EventArgs e)
    {
        App.Current.MainPage = new LeerProductoInsert();

    }
}