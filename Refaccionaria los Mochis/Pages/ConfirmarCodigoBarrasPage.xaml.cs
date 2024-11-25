using CapaEntidad;

namespace Refaccionaria_los_Mochis.Pages;

public partial class ConfirmarCodigoBarrasPage : ContentPage
{
    public ConfirmarCodigoBarrasPage(string codigoConCero, string codigoSinCero)
    {
        InitializeComponent();

        // Asignar los códigos a los botones
        btnCodigoConCero.Text = codigoConCero;
        btnCodigoSinCero.Text = codigoSinCero;

        btnCodigoConCero.Clicked += async (sender, e) => await ConfirmarCodigo(codigoConCero);
        btnCodigoSinCero.Clicked += async (sender, e) => await ConfirmarCodigo(codigoSinCero);
    }

    private async Task ConfirmarCodigo(string codigoBarras)
    {
        Producto CrearProducto = new Producto { CodigoBarras = codigoBarras };
        var navigationParameter = new Dictionary<string, object>
        {
            { "producto", CrearProducto }
        };

        var detallesPage = new CrearProducto(navigationParameter);
        App.Current.MainPage = detallesPage;
    }
}