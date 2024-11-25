using Refaccionaria_los_Mochis.Generic;
using Refaccionaria_los_Mochis.Models;
using Refaccionaria_los_Mochis.Pages;
using ZXing.Net.Maui;
using ZXing.Net.Maui.Controls;
using Microsoft.Maui.Controls;

namespace Refaccionaria_los_Mochis.Pages;

public partial class LeerProducto : ContentPage
{
    public ProductoModel oProductoModel { get; set; }
    private bool isBusy = false;
    private CameraLocation currentCameraLocation = CameraLocation.Rear;

    public LeerProducto()
    {
        InitializeComponent();
    }

    private void barcodeReaderView_BarcodesDetected(object sender, BarcodeDetectionEventArgs e)
    {
        Device.BeginInvokeOnMainThread(() =>
        {
            if (isBusy) return;

            var barcode = e.Results.FirstOrDefault();
            if (barcode != null)
            {
                buscarProducto(barcode.Value);
            }
        });
    }

    private async void buscarProducto(string CodigoBarras)
    {
        try
        {
            isBusy = true;

            if (CodigoBarras.Length > 1)
            {
                CodigoBarras = CodigoBarras.Substring(1);
            }

            var url = $"http://" + IP.SERVIDOR + ":5210/Productos/SeleccionarCodigoBarras2?codigoBarras=" + CodigoBarras;
            var producto = await Http.Get(url);
            if (producto != null)
            {
                var navigationParameter = new Dictionary<string, object>
                {
                    { "producto", producto }
                };

                // Crear una instancia de la página de detalles del producto
                var detallesPage = new ProductoDetallesPage(navigationParameter);

                // Asignar la página de detalles del producto como la página principal de la aplicación
                App.Current.MainPage = detallesPage;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudo obtener la lista de productos: {ex.Message}", "OK");
        }
        finally
        {
            isBusy = false;
        }
    }

    private void btnRegresar_Clicked(object sender, EventArgs e)
    {
        App.Current.MainPage = new PrincipalPage();
    }

    private void OnChangeOrientationClicked(object sender, EventArgs e)
    {
        if (currentCameraLocation == CameraLocation.Rear)
        {
            currentCameraLocation = CameraLocation.Front;
        }
        else
        {
            currentCameraLocation = CameraLocation.Rear;
        }

        barcodeReaderView.CameraLocation = currentCameraLocation;
    }
}
