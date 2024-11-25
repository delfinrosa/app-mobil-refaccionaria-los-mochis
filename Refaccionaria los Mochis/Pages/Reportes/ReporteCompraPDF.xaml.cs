using Microsoft.Maui.Controls;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Refaccionaria_los_Mochis.Generic;
using System.Net;

namespace Refaccionaria_los_Mochis.Pages.Reportes;

public partial class ReporteCompraPDF : ContentPage
{
	public ReporteCompraPDF()
	{

        InitializeComponent();
        LoadPdfAsync();
    }

    private async Task LoadPdfAsync()
    {
        var idCompra = "B854E6F7-BC87-436C-9D45-AD1190A98F28";
        var pdfUrl = $"http://{IP.SERVIDOR}:8080/Compras/CompraPDF?idCompra={idCompra}";
        var localFileName = $"COMPRA_{idCompra}.pdf";

#if ANDROID
            var localFilePath = Path.Combine(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads).AbsolutePath, localFileName);

            // Descargar el PDF
            using (var httpClient = new HttpClient())
            {
                var pdfData = await httpClient.GetByteArrayAsync(pdfUrl);
                File.WriteAllBytes(localFilePath, pdfData);
            }

            // Configurar WebView para que pueda acceder a archivos
            Microsoft.Maui.Handlers.WebViewHandler.Mapper.AppendToMapping("pdfviewer", (handler, view) =>
            {
                handler.PlatformView.Settings.AllowFileAccess = true;
                handler.PlatformView.Settings.AllowFileAccessFromFileURLs = true;
                handler.PlatformView.Settings.AllowUniversalAccessFromFileURLs = true;
            });

            // Mostrar el PDF en el WebView
            var localUrl = $"file://{localFilePath}";
            pdfview.Source = $"file:///android_asset/pdfjs/web/viewer.html?file={WebUtility.UrlEncode(localUrl)}";
#endif
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        App.Current.MainPage = new PrincipalPage();

    }
}
