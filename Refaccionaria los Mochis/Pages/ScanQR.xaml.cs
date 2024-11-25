using CapaEntidad;
using Refaccionaria_los_Mochis.Generic;
using Refaccionaria_los_Mochis.Pages.Compra;

namespace Refaccionaria_los_Mochis.Pages;

public partial class ScanQR : ContentPage
{
    public ScanQR()
    {

        InitializeComponent();
        detectorImagen.Options = new ZXing.Net.Maui.BarcodeReaderOptions
        {
            Formats = ZXing.Net.Maui.BarcodeFormat.QrCode
        };

    }

    private void btnRegresar_Clicked(object sender, EventArgs e)
    {
        App.Current.MainPage = new PrincipalPage();
    }

    private void detectorImagen_BarcodesDetected(object sender, ZXing.Net.Maui.BarcodeDetectionEventArgs e)
    {
        detectorImagen.IsDetecting = false;
        if (e.Results.Any())
        {
            var result = e.Results.FirstOrDefault();
            Dispatcher.Dispatch(async () =>
            {


                // Guardar el valor del código de barras en almacenamiento seguro
                await SecureStorage.SetAsync("Compra", result.Value);

                try
                {
                    // Llamar al método GetAllCompras desde la clase Http para obtener los detalles de la compra
                    var url = $"http://{IP.SERVIDOR}:5210/Compra/ObtenerDetallesCompra?idcompra=" + result.Value;
                    var detallesCompra = await Http.GetAllCompras(url);

                    if (detallesCompra.Any())
                    {
                        var navigationParameter = new Dictionary<string, object>
                    {
                        { "detallesCompra", detallesCompra }
                    };

                        var detallesPage = new CompraDetallesPage(navigationParameter);
                        App.Current.MainPage = detallesPage;
                    }
                    else
                    {
                        await DisplayAlert("Error", "No se encontraron detalles de la compra.", "OK");
                    }
                }
                catch (Exception ex)
                {
                    // Manejar posibles errores
                    await DisplayAlert("Error", $"No se pudo obtener los detalles de la compra: {ex.Message}", "OK");
                }
            });
        }
    }



}