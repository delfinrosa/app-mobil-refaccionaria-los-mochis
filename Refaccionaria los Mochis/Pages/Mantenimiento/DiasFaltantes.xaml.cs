using Microsoft.Maui.Controls;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Refaccionaria_los_Mochis.Generic;
using System.Drawing;
using CapaEntidad;
using Color = Microsoft.Maui.Graphics.Color;

namespace Refaccionaria_los_Mochis.Pages.Mantenimiento;

public partial class DiasFaltantes : ContentPage
{
    public DiasFaltantes()
    {
        InitializeComponent();
        InitDias();
    }

    private async void InitDias()
    {
        var url = $"http://{IP.SERVIDOR}:5210/Mantenimiento/DiasFaltanteMantenimiento";
        var info = await Http.GetDiasFaltantes(url);

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

    private View CreateCard(Tuple<string, string, int> data)
    {
        var frame = new Frame
        {
            BorderColor = Color.FromHex("#FCC208"),
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
                            Text = $"ID Equipo: {data.Item1}",
                            FontSize = 18,
                            FontAttributes = FontAttributes.Bold,
                            VerticalTextAlignment=TextAlignment.Center
                        },
                        new Label
                        {
                            Text = $"Fecha Último Mantenimiento: {data.Item2}",
                            FontSize = 16
                        },
                        CreateDaysRemainingCircleWithButton(data.Item3,data.Item1)
                    }
            }
        };

        return frame;
    }
    private View CreateDaysRemainingCircleWithButton(int daysRemaining,string IdEquipo)
    {
        var gridLayout = new Grid
        {
            ColumnDefinitions =
        {
            new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
            new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) }
        }
        };

        // Botón en la primera columna
        var button = new Button
        {
            Text = "Mas informacion",
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            BackgroundColor = Color.FromHex("#808080"),
            TextColor = Color.FromHex("#FFFFFF"),
            BorderColor = Color.FromHex("#FCC208"), // Convierte el color hex a Color
            BorderWidth = 2 // BorderWidth es un double, no necesita comillas
        };


        button.Clicked += async (sender, e) =>
        {

            var navigationParameter = new Dictionary<string, string>
            {
                    { "IdEquipo", IdEquipo }
                };

            var detallesPage = new DetalleEquipo(navigationParameter);

            App.Current.MainPage = detallesPage;

        };

        var circleLayout = new Frame
        {
            WidthRequest = 100,
            HeightRequest = 100,
            BackgroundColor = Colors.LightGray,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            CornerRadius = 50,
            Padding = 0,

            Content = new StackLayout
            {
                Children =
            {
                new Label
                {
                    Text = "Dias Faltantes: \n" + daysRemaining.ToString(),
                    FontSize = 18,
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Center
                }
            }
            }
        };

        // Añadir los elementos al grid
        gridLayout.Add(circleLayout, 0, 0); // Añadir círculo a la segunda columna
        gridLayout.Add(button, 1, 0); // Añadir botón a la primera columna

        return gridLayout;
    }


    private void btnRegresar_Clicked(object sender, EventArgs e)
    {
        App.Current.MainPage = new PrincipalPage();
    }
}

