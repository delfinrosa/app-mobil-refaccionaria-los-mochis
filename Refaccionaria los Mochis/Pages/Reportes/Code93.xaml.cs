namespace Refaccionaria_los_Mochis.Pages;

public partial class Code93 : ContentPage
{
    public Code93()
    {
        InitializeComponent();
        detectorImagen.Options = new ZXing.Net.Maui.BarcodeReaderOptions
        {
            Formats = ZXing.Net.Maui.BarcodeFormat.Code93
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
                DisplayAlert("Valor", result.Value, "Cancelar");
                App.Current.MainPage = new PrincipalPage();
            });
        }
    }
}