using Refaccionaria_los_Mochis.Generic;
using Refaccionaria_los_Mochis.Models;
using CapaEntidad;

namespace Refaccionaria_los_Mochis.Pages;



public partial class EntryChange : ContentPage
{
    public EntryChange()
    {
        InitializeComponent();
    }

    private async void entry_TextChanged_1(object sender, TextChangedEventArgs e)
    {
        await RealizarBusqueda();
    }

    private async void Buscar_Clicked(object sender, EventArgs e)
    {
        buscarProducto(entry.Text.ToUpper().Trim());
    }

    private async Task RealizarBusqueda()
    {
        // Check if the CheckBox is checked to determine the delay time
        //int delayTime = delayCheckBox.IsChecked ? 30000 : 1000;
        //await Task.Delay(delayTime);

        //buscarProducto(entry.Text.ToUpper().Trim());
    }

    private async void buscarProducto(string CodigoBarras)
    {
        try
        {
            var url = $"http://" + IP.SERVIDOR + ":5210/Productos/SeleccionarCodigoBarras2?codigoBarras=" + CodigoBarras;
            var producto = await Http.Get(url);
            if (producto.Descripcion != null)
            {
                var navigationParameter = new Dictionary<string, object>
                {
                    { "producto", producto }
                };

                var detallesPage = new ProductoDetallesPage(navigationParameter);

                App.Current.MainPage = detallesPage;
            }
            else
            {
                Producto CrearProducto = new Producto();
                CrearProducto.CodigoBarras = CodigoBarras;
                var navigationParameter = new Dictionary<string, object>
                {
                    { "producto", CrearProducto }
                };

                var detallesPage = new CrearProducto(navigationParameter);

                App.Current.MainPage = detallesPage;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudo obtener la lista de productos: {ex.Message}", "OK");
        }
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        App.Current.MainPage = new PrincipalPage();
    }

    private void Button_Clicked_1(object sender, EventArgs e)
    {
        buscarProducto(entrynumeros.Text.ToUpper().Trim());

    }
}