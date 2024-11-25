using Refaccionaria_los_Mochis.Generic;
namespace Refaccionaria_los_Mochis.Pages.Mantenimiento;

public partial class LectorCodigoManteniemiento : ContentPage
{
	public LectorCodigoManteniemiento()
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


    private async void detectorImagen_BarcodesDetected(object sender, ZXing.Net.Maui.BarcodeDetectionEventArgs e)
    {
        detectorImagen.IsDetecting = false;
        if (e.Results.Any())
        {
            var result = e.Results.FirstOrDefault();
            try
            {
                // Construir la URL con el ID del equipo
                var url = $"http://{IP.SERVIDOR}:5210/Mantenimiento/VerificarEquipo?idEquipo=" + result.Value;

                // Obtener la respuesta utilizando el m�todo HTTP adecuado
                var (exito, mensaje, existe) = await Http.VerificarEquipoAsync(url);

                if (exito)
                {
                    if (existe)
                    {
                        var navigationParameter = new Dictionary<string, object>
                        {
                            { "idEquipo", result.Value }
                        };

                        // Aseg�rate de que la asignaci�n se haga en el hilo principal
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            var detallesPage = new MantenimientoPage(navigationParameter);
                            App.Current.MainPage = detallesPage;
                        });
                    }

                    else
                    {
                        // Si no existe, mostrar mensaje de error
                        await DisplayAlert("Error", $"No se encontraron detalles para el equipo con ID: {result.Value}.", "OK");
                    }
                }
                else
                {
                    // Si hubo un problema con la solicitud, mostrar el mensaje de error
                    await DisplayAlert("Error", $"Error al verificar el equipo: {mensaje}", "OK");
                }
            }
            catch (Exception ex)
            {
                // Manejar cualquier excepci�n y mostrar mensaje de error
                await DisplayAlert("Error", $"Ocurri� un error al verificar el equipo: {ex.Message}", "OK");
            }
            finally
            {
                // Reiniciar la detecci�n
                detectorImagen.IsDetecting = true;
            }
        }
    }






















}