using CapaEntidad;
using Newtonsoft.Json.Linq;
using Refaccionaria_los_Mochis.Generic;
namespace Refaccionaria_los_Mochis.Pages.Registrar.Fotos;

public partial class EscribirCodigoBarras : ContentPage
{
	public EscribirCodigoBarras()
	{
		InitializeComponent();
	}
    private async void Buscar_Clicked(object sender, EventArgs e)
    {
        buscarProducto(entry.Text.ToUpper().Trim());
    }
    private async void buscarProducto(string CodigoBarras)
    {
        try
        {
            List<Producto> producto = new List<Producto>();
            var url = $"http://" + IP.SERVIDOR + ":5210/Productos/ListaSeleccionarCodigoBarras?codigoBarras=" + CodigoBarras;
            var cliente = new HttpClient();
            var response = await cliente.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                var jsonObject = JObject.Parse(result);

                producto= jsonObject["Productos"].ToObject<List<Producto>>();

                if (producto.Count != 0)
                {
                    if (producto.Count == 1)
                    {
                        var tomarFoto = new TomarFoto(producto[0].IdProducto);
                        App.Current.MainPage = tomarFoto;
                    }
                    else
                    {

                        var confirmarElemento = new ConfirmarElemento(producto);
                        App.Current.MainPage = confirmarElemento;
                    }
                }
                else
                {
                    await SecureStorage.SetAsync("Error", "No se a encontrado el codigo de barras vuelva a buscar");

                }
            }
            else
            {
                await response.Content.ReadAsStringAsync();
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