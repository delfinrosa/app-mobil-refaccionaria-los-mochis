using CapaEntidad;
using Newtonsoft.Json.Linq;
using Refaccionaria_los_Mochis.Generic;
using Refaccionaria_los_Mochis.Models;
using ZXing.Net.Maui;
using Refaccionaria_los_Mochis.Pages.Registrar.Fotos;
namespace Refaccionaria_los_Mochis.Pages.Registrar;

public partial class BuscarCodigoBarras : ContentPage
{
	public BuscarCodigoBarras()
	{
		InitializeComponent();
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
        barcodeReaderView.IsEnabled = true; // Asegura que la cámara esté activa al iniciar
    }

    private void barcodeReaderView_BarcodesDetected(object sender, ZXing.Net.Maui.BarcodeDetectionEventArgs e)
    {
        Device.BeginInvokeOnMainThread(async () =>
        {
            var barcode = e.Results.FirstOrDefault();
            if (barcode != null)
            {
                // Desactivar la cámara
                barcodeReaderView.IsEnabled = false;

                buscarProducto(barcode.Value, barcode.Format);
                await Task.Delay(1000);
            }
        });
    }

    private async void buscarProducto(string CodigoBarras, ZXing.Net.Maui.BarcodeFormat formato)
    {
        try
        {
            List<Producto> producto = new List<Producto>();
            
            if (formato != ZXing.Net.Maui.BarcodeFormat.Ean13)
            {
                producto = await listaProductos(CodigoBarras);

                if (producto.Count != 0)
                {
                    if (producto.Count ==1)
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
            else
            {

                producto = await listaProductos(CodigoBarras);

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
                    if (CodigoBarras.StartsWith("0"))
                    {
                        CodigoBarras = CodigoBarras.Substring(1);
                        producto = await listaProductos(CodigoBarras);
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

                            var confirmarPage = new ConfirmarCodigoBarrasPage("0" + CodigoBarras, CodigoBarras);
                            App.Current.MainPage = confirmarPage;

                        }
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
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudo obtener la lista de productos: {ex.Message}", "OK");
        }
    }

    public async Task<List<Producto>> listaProductos(string CodigoBarras)
    {
        List<Producto> producto = new List<Producto>();
        var url = $"http://" + IP.SERVIDOR + ":5210/Productos/ListaSeleccionarCodigoBarras?codigoBarras=" + CodigoBarras;
        var cliente = new HttpClient();
        var response = await cliente.GetAsync(url);

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadAsStringAsync();
            var jsonObject = JObject.Parse(result); 

            return jsonObject["Productos"].ToObject<List<Producto>>();
        }
        else
        {
            await response.Content.ReadAsStringAsync(); 
            return new List<Producto>();
        }
    }




    private void btnRegresar_Clicked(object sender, EventArgs e)
    {
        App.Current.MainPage = new PrincipalPage();

    }

    private void btnEscribir_Clicked(object sender, EventArgs e)
    {
        App.Current.MainPage = new EscribirCodigoBarras();

    }
}