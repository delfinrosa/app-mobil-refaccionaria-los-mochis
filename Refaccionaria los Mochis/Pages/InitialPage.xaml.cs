using Microsoft.Maui.Controls;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Refaccionaria_los_Mochis.Generic;
using System.Net;

namespace Refaccionaria_los_Mochis.Pages
{
    public partial class InitialPage : ContentPage
    {
        public InitialPage()
        {
            InitializeComponent();
            //LoadPdfAsync();
            LoadReportes();
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
            //string nombre = await SecureStorage.GetAsync("UserNombre");
            //NombreCuentas.Text = "Cuenta: " + nombre;
        }

        //        private async Task LoadPdfAsync()
        //        {
        //            var idCompra = "B854E6F7-BC87-436C-9D45-AD1190A98F28";
        //            var pdfUrl = $"http://{IP.SERVIDOR}:8080/Compras/CompraPDF?idCompra={idCompra}";
        //            var localFileName = $"COMPRA_{idCompra}.pdf";

        //#if ANDROID
        //            var localFilePath = Path.Combine(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads).AbsolutePath, localFileName);

        //            // Descargar el PDF
        //            using (var httpClient = new HttpClient())
        //            {
        //                var pdfData = await httpClient.GetByteArrayAsync(pdfUrl);
        //                File.WriteAllBytes(localFilePath, pdfData);
        //            }

        //            // Configurar WebView para que pueda acceder a archivos
        //            Microsoft.Maui.Handlers.WebViewHandler.Mapper.AppendToMapping("pdfviewer", (handler, view) =>
        //            {
        //                handler.PlatformView.Settings.AllowFileAccess = true;
        //                handler.PlatformView.Settings.AllowFileAccessFromFileURLs = true;
        //                handler.PlatformView.Settings.AllowUniversalAccessFromFileURLs = true;
        //            });

        //            // Mostrar el PDF en el WebView
        //            var localUrl = $"file://{localFilePath}";
        //            pdfview.Source = $"file:///android_asset/pdfjs/web/viewer.html?file={WebUtility.UrlEncode(localUrl)}";
        //#endif
        //        }

        private async Task LoadReportes()
        {
            try
            {
                var url = $"http://{IP.SERVIDOR}:5210/Reportes/ObtenerStockReporte";
                var (totalProductos, cantidadBaja, cantidadMediana, cantidadBien) = await Http.GetStockReporteResumen(url);

                TotalProductosLabel.Text = $"Productos Totales: {totalProductos}";
                BajoStockLabel.Text = $"Bajo Stock: {cantidadBaja}";
                MedioStockLabel.Text = $"Medio Stock: {cantidadMediana}";
                AltoStockLabel.Text = $"Alto Stock: {cantidadBien}";

                url = $"http://{IP.SERVIDOR}:5210/Reportes/ObtenerReporteCompras";
                // Llamar al método para obtener el reporte
                var reporteCompras = await Http.GetReporteComprasAsync(url);

                if (reporteCompras != null && reporteCompras.Count > 0)
                {
                    // Asignar la lista al CollectionView
                    ComprasCollectionView.ItemsSource = reporteCompras;
                }
                else
                {
                    await DisplayAlert("Sin Datos", "No se encontraron datos de compras.", "OK");
                }
            }
            catch (Exception ex)
            {
                // Manejo de errores
                await DisplayAlert("Error", $"No se pudo cargar el reporte: {ex.Message}", "OK");
            }
        }
        int caso = 1; // Por defecto, Bajo Stock
        int pagina = 0; // Página inicial

        private async void BajoButton_Clicked(object sender, EventArgs e)
        {
            TIPOSTOCK.Text = "Bajo Stock";
            TIPOSTOCK.TextColor = Colors.Red;
            pagina = 0;
            caso = 1;
            ContainerElement.IsVisible = true;

            await CargarDatosAsync(); // Cargar datos dinámicamente
        }

        private async void MedioButton_Clicked(object sender, EventArgs e)
        {
            TIPOSTOCK.Text = "Medio Stock";
            TIPOSTOCK.TextColor = Color.FromArgb("#FCC208");
            pagina = 0;
            caso = 2;
            ContainerElement.IsVisible = true;

            await CargarDatosAsync(); // Cargar datos dinámicamente
        }

        private async void BienButton_Clicked(object sender, EventArgs e)
        {
            TIPOSTOCK.Text = "Bien Stock";
            TIPOSTOCK.TextColor = Color.FromArgb("#124734");
            pagina = 0;
            caso = 3;
            ContainerElement.IsVisible = true;
            await CargarDatosAsync(); // Cargar datos dinámicamente
        }

        private async void ANTERIORButton_Clicked(object sender, EventArgs e)
        {
            if (pagina > 0) // Evitar páginas negativas
            {
                pagina--;
                await CargarDatosAsync(); // Cargar datos dinámicamente
            }
        }

        private async void SIGUIENTEButton_Clicked(object sender, EventArgs e)
        {
            pagina++;
            await CargarDatosAsync(); // Cargar datos dinámicamente
        }
        private async Task CargarDatosAsync()
        {
            try
            {

                Color colorSeleccion;

                if (caso == 1) // Bajo Stock
                {
                    colorSeleccion = Colors.Red;
                }
                else if (caso == 2) // Medio Stock
                {
                    colorSeleccion = Color.FromArgb("#FCC208"); // Amarillo
                }
                else // Bien Stock
                {
                    colorSeleccion = Color.FromArgb("#124734"); // Verde
                }


                var url = $"http://{IP.SERVIDOR}:5210/Reportes/ObtenerStockReportePaginado?caso={caso}&page={pagina}";
                var (totalRegistros, datos) = await Http.GetStockReportePaginadoAsync(url);

                if (datos.Count > 0)
                {
                    ScrollContent.Children.Clear(); // Limpiar contenido anterior

                    foreach (var item in datos)
                    {
                        // Crear una etiqueta para cada registro y agregarlo al ScrollContent
                        var label = new Label
                        {
                            Text = $"NoParte: {item.Item1},       Marca: {item.Item5}",
                            FontSize = 22,
                            TextColor = Colors.White,
                            HorizontalOptions = LayoutOptions.Center
                        };

                        ScrollContent.Children.Add(label);
                        var label2 = new Label
                        {
                            Text = $"Cantidad: {item.Item2}",
                            FontSize = 24,
                            TextColor = colorSeleccion,
                            HorizontalOptions = LayoutOptions.Center
                        };

                        ScrollContent.Children.Add(label2);
                        var label3 = new Label
                        {
                            Text = $"Min: {item.Item3},       Max: {item.Item4}",
                            FontSize = 20,
                            TextColor = Colors.White,
                            HorizontalOptions = LayoutOptions.Center
                        };

                        ScrollContent.Children.Add(label3);
                        var label4 = new Label
                        {
                            Text = $"____________________________________________________",
                            FontSize = 16,
                            TextColor = Colors.Black,
                            HorizontalOptions = LayoutOptions.Center
                        };

                        ScrollContent.Children.Add(label4);
                    }
                }
                else
                {
                    // Mostrar mensaje si no hay datos
                    ScrollContent.Children.Clear();
                    ScrollContent.Children.Add(new Label
                    {
                        Text = "No se encontraron datos para esta página.",
                        FontSize = 18,
                        TextColor = Colors.Gray,
                        HorizontalOptions = LayoutOptions.Center
                    });
                }
            }
            catch (Exception ex)
            {
                // Manejo de errores
                await DisplayAlert("Error", $"No se pudieron cargar los datos: {ex.Message}", "OK");
            }
        }


    }
}

