using CapaEntidad;
using Refaccionaria_los_Mochis.Generic;
using ZXing;
using CommunityToolkit.Maui.Core.Platform;

namespace Refaccionaria_los_Mochis.Pages.Reportes;

public partial class LectorCodigoRack : ContentPage
{
    public LectorCodigoRack()
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
        if (e.Results.Any())
        {
            var result = e.Results.FirstOrDefault();

            Dispatcher.Dispatch(async () =>
            {
                // Crear el diccionario de parámetros de navegación
                var navigationParameter = new Dictionary<string, object>
            {
                { "Rack", result.Value }
            };

                // Crear una instancia de ReporteProductosRack con el diccionario de parámetros
                var detallesPage = new ReporteProductosRack(navigationParameter);

                // Redirigir a la nueva página
                App.Current.MainPage = detallesPage;
            });
        }
    }

    private async void BtnRackBusqueda_Clicked(object sender, EventArgs e)
    {
        var ubicacion = BusquedaRackEntry.Text.Trim();


        if (!string.IsNullOrEmpty(ubicacion))
        {
            var url = $"http://{IP.SERVIDOR}:5210/AlmacenRack/ObtenerAlmacenRack?almacenId={1}&ubicacion={ubicacion}";

            var (racks, mensaje) = await Http.GetAlmacenRack(url);

            if (!string.IsNullOrEmpty(mensaje))
            {
                await DisplayAlert("Error", mensaje, "Continuar");
                BusquedaRackEntry.Text = "";
            }
            else
            {
                RacksListView.ItemsSource = racks;
                RacksListView.IsVisible = true;
                BusquedaRackEntry.IsVisible = true;
            }
        }
        else
        {
            await DisplayAlert("Error", "No hay texto en Rack.", "OK");
        }
        if (KeyboardExtensions.IsSoftKeyboardShowing(BusquedaRackEntry))
        {
            await KeyboardExtensions.HideKeyboardAsync(BusquedaRackEntry, default);
        }
        else
        {
            await KeyboardExtensions.ShowKeyboardAsync(BusquedaRackEntry, default);
        }
    }

    private async void RacksListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {

        if (e.SelectedItem is AlmacenRack selectedAlmacenes)
        {
            Dispatcher.Dispatch(async () =>
            {
                // Crear el diccionario de parámetros de navegación
                var navigationParameter = new Dictionary<string, object>
                {
                { "Rack", selectedAlmacenes.Ubicacion }
            };

                // Crear una instancia de ReporteProductosRack con el diccionario de parámetros
                var detallesPage = new ReporteProductosRack(navigationParameter);

                // Redirigir a la nueva página
                App.Current.MainPage = detallesPage;
            });
        }
    }


}