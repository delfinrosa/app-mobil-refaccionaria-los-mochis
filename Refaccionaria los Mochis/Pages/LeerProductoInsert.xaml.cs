using CapaEntidad;
using Refaccionaria_los_Mochis.Generic;
using Refaccionaria_los_Mochis.Models;
using ZXing.Net.Maui;

namespace Refaccionaria_los_Mochis.Pages;

public partial class LeerProductoInsert : ContentPage
{

    public LeerProductoInsert()
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

            if (formato != ZXing.Net.Maui.BarcodeFormat.Ean13)
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
            else
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
                    if (CodigoBarras.StartsWith("0"))
                    {
                        CodigoBarras = CodigoBarras.Substring(1);
                        url = $"http://" + IP.SERVIDOR + ":5210/Productos/SeleccionarCodigoBarras2?codigoBarras=" + CodigoBarras;
                        producto = await Http.Get(url);
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




    private void btnRegresar_Clicked(object sender, EventArgs e)
    {
        App.Current.MainPage = new PrincipalPage();

    }

    private void btnEscribir_Clicked(object sender, EventArgs e)
    {
        App.Current.MainPage = new EntryChange();

    }
}