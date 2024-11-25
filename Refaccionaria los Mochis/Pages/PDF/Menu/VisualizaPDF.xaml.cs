using Microsoft.Maui.Controls;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Refaccionaria_los_Mochis.Generic;
using System.Net;

namespace Refaccionaria_los_Mochis.Pages.PDF.Menu;

public partial class VisualizaPDF : ContentPage
{
    public VisualizaPDF(Dictionary<string, string> navigationParameter)
    {
        InitializeComponent();
        if (navigationParameter.TryGetValue("idCompra", out string idCompra))
        {
            LoadPdfAsync(idCompra);
        }
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        string userId = await SecureStorage.GetAsync("UserId");

        if (!string.IsNullOrEmpty(userId))
        {
            int idUsuario = int.Parse(userId);
            if (idUsuario <= 0)
            {
                App.Current.MainPage = new login();
            }
        }
        else
        {
            App.Current.MainPage = new login();

        }
    }

    private async Task LoadPdfAsync(string idCompra)
    {
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
}
