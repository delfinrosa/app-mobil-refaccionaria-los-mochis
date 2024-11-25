namespace Refaccionaria_los_Mochis.Pages;

public partial class Code128 : ContentPage
{
    public Code128()
    {
        InitializeComponent();
        //detectorImagen.Options = new ZXing.Net.Maui.BarcodeReaderOptions
        //{
        //    Formats = ZXing.Net.Maui.BarcodeFormat.Code128
        //};
    }
    private void barcodeReaderView_BarcodesDetected(object sender, ZXing.Net.Maui.BarcodeDetectionEventArgs e)
    {
        Device.BeginInvokeOnMainThread(() =>
        {
            var barcode = e.Results.FirstOrDefault();
            if (barcode != null)
            {
                string codigoEscaneado = barcode.Value;

                // Verificar si el código escaneado comienza con "0" y si hay un "0" en la posición 7
                if (codigoEscaneado.Length == 13 && codigoEscaneado.StartsWith("0") && codigoEscaneado[6] == '0')
                {
                    // El "0" al inicio es parte del código
                    resultLabel.Text = $"Código EAN-13: {codigoEscaneado}";
                }
                else
                {
                    // El "0" al inicio es solo un relleno
                    // Eliminar el "0" adicional al inicio del código
                    if (codigoEscaneado.StartsWith("0"))
                    {
                        codigoEscaneado = codigoEscaneado.Substring(1);
                    }

                    resultLabel.Text = $"\n\nCódigo: {codigoEscaneado} \nFormato: {barcode.Format}";
                }
            }
        });
    }


    private void btnRegresar_Clicked(object sender, EventArgs e)
    {
        App.Current.MainPage = new PrincipalPage();

    }
}