using CapaEntidad;
using Newtonsoft.Json.Linq;
using Refaccionaria_los_Mochis.Generic;
namespace Refaccionaria_los_Mochis.Pages.PDF.Menu;

public partial class PrincipalMenuPDF : ContentPage
{
	public PrincipalMenuPDF()
	{

            InitializeComponent();
            InitCompras();
        }

        private async void InitCompras()
        {
            var url = $"http://{IP.SERVIDOR}:5210/Reportes/ObtenerCompras";
            var info = await GetCompras(url);

            if (info != null)
            {
                CardsContainer.Children.Clear(); // Limpia las tarjetas existentes

                foreach (var item in info)
                {
                    var card = CreateCard(item);
                    CardsContainer.Children.Add(card);
                }
            }
        }

        private async Task<List<Tuple<string, string, string, string, string>>> GetCompras(string url)
        {
            using (var cliente = new HttpClient())
            {
                var response = await cliente.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var jsonArray = JArray.Parse(result);
                    var compras = new List<Tuple<string, string, string, string, string>>();

                    foreach (var item in jsonArray)
                    {
                        compras.Add(Tuple.Create(
                            item["Item1"].ToString(), // IdCompra
                            item["Item2"].ToString(), // FechaCompra
                            item["Item3"].ToString(), // Proveedor
                            item["Item4"].ToString(), // CantidadProductos
                            item["Item5"].ToString()  // Total
                        ));
                    }
                    return compras;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error: {response.StatusCode}, Details: {errorContent}");
                    return null;
                }
            }
        }

    private View CreateCard(Tuple<string, string, string, string, string> data)
    {
        var gridLayout = new Grid();

        var button = new Button
        {
            Text = "Mostrar ID Compra",
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
        };

        button.Clicked += async (sender, e) =>
        {
            var navigationParameter = new Dictionary<string, string>
            {
                    { "idCompra", data.Item1 }
                };

            var detallesPage = new VisualizaPDF(navigationParameter);

            App.Current.MainPage = detallesPage;
        };

        var circleLayout = new Frame
        {
            BorderColor = Colors.Gray,
            CornerRadius = 20,
            Padding = 10,
            Margin = new Thickness(0, 0, 0, 10),
            HasShadow = true,
            Content = new StackLayout
            {
                Spacing = 10,
                Children =
            {
                new Label
                {
                    Text = $"ID Compra: {data.Item1}",
                    FontSize = 18,
                    FontAttributes = FontAttributes.Bold,
                    VerticalTextAlignment = TextAlignment.Center
                },
                new Label
                {
                    Text = $"Fecha de Compra: {data.Item2}",
                    FontSize = 16
                },
                new Label
                {
                    Text = $"Proveedor: {data.Item3}",
                    FontSize = 16
                },
                new Label
                {
                    Text = $"Cantidad de Productos: {data.Item4}",
                    FontSize = 16
                },
                new Label
                {
                    Text = $"Total: ${data.Item5}",
                    FontSize = 16
                },
                button 
            }
            }
        };

        gridLayout.Add(circleLayout); 

        return gridLayout;
    }



    private void btnRegresar_Clicked(object sender, EventArgs e)
        {
            App.Current.MainPage = new PrincipalPage(); // Cambiar a la página principal
        }
}