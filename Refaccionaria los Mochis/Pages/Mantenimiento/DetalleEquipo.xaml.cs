using CapaEntidad;
using Refaccionaria_los_Mochis.Generic;

namespace Refaccionaria_los_Mochis.Pages.Mantenimiento
{
    public partial class DetalleEquipo : ContentPage
    {
        private string IdEquipo;

        public DetalleEquipo(Dictionary<string, string> navigationParameter)
        {
            InitializeComponent();
            if (navigationParameter != null && navigationParameter.ContainsKey("IdEquipo"))
            {
                IdEquipo = navigationParameter["IdEquipo"];
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (!string.IsNullOrEmpty(IdEquipo))
            {
                try
                {
                    var url = $"http://{IP.SERVIDOR}:5210/Mantenimiento/DetallesEquipo?equipo={IdEquipo}";
                    var info = await Http.GetDetalleEquipo(url);
                    if (info.Count!=0)
                    {
                        DisplayDetails(info);

                    }
                    else
                    {
                        Application.Current.MainPage.DisplayAlert("Error", "Este Equipo no tiene registros de fallas o mantenimiento", "OK");
                        App.Current.MainPage = new PrincipalPage();

                    }
                }
                catch (Exception ex)
                {
                    // Manejo de excepciones
                    Console.WriteLine($"Error al obtener los detalles del equipo: {ex.Message}");
                }
            }
        }


        private void DisplayDetails(List<Tuple<bool, bool, string, string>> detalles)
        {
            CardsContainer.Children.Clear(); // Limpia los elementos anteriores

            foreach (var detalle in detalles)
            {
                // Crear el Grid para contener los elementos
                var grid = new Grid
                {
                    Padding = new Thickness(10, 5), // Espaciado interno para evitar sobreposición
                    ColumnDefinitions =
            {
            new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
            new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) },
            new ColumnDefinition { Width = new GridLength(3, GridUnitType.Star) },
            },
                    RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto }
            }
                };

                // Agregar los círculos de estado
                var limpiezaCircle = CreateStatusCircle(detalle.Item1, "Limpieza");
                var falloCircle = CreateStatusCircle(detalle.Item2, "Fallo");
                grid.Add(limpiezaCircle, 0, 0); // Columna 0, Fila 0
                grid.Add(falloCircle, 1, 0); // Columna 1, Fila 0

                // Agregar la información observación y fecha
                var fechaLabel = new Label
                {
                    Text = $"Fecha: {detalle.Item4}",
                    VerticalOptions = LayoutOptions.Center,
                    TextColor = Colors.White,
                    FontSize = 18,
                    FontAttributes = FontAttributes.Bold
                };
                grid.Add(fechaLabel, 2, 0); // Columna 2, Fila 1

                // Mostrar el label "Limpieza" solo si detalle.Item2 es true
                if (detalle.Item2)
                {
                var observacionLabel = new Label
                {
                    Text = $"Observación: {detalle.Item3}",
                    VerticalOptions = LayoutOptions.Center,
                    TextColor = Colors.White, 
                    FontSize = 18,
                    FontAttributes = FontAttributes.Bold
                };

                grid.Add(observacionLabel, 2, 1); // Columna 2, Fila 0
                }

                // Añadir el Grid al contenedor
                CardsContainer.Children.Add(grid);
            }
        }


        private View CreateStatusCircle(bool status, string type)
        {
            // Crear el círculo de estado
            var circle = new BoxView
            {
                Color = status ? Colors.Green : Colors.Red,
                WidthRequest = 80, // Ajustar el tamaño para que el círculo se vea bien
                HeightRequest = 30,
                CornerRadius = 25, // Mitad del ancho/alto para hacer un círculo
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                
            };

            // Crear el texto
            var label = new Label
            {
                Text = type, // Usa el texto completo
                TextColor = Colors.White, // Color del texto
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                FontSize = 12,
                FontAttributes = FontAttributes.Bold,
                LineBreakMode = LineBreakMode.NoWrap, // Evita el ajuste de línea
                HorizontalTextAlignment = TextAlignment.Center // Centra el texto horizontalmente
            };

            // Crear un Grid para combinar el círculo y el texto
            var grid = new Grid
            {
                WidthRequest = 50,
                HeightRequest = 50,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center
                
                
            };

            grid.Children.Add(circle);
            grid.Children.Add(label);

            return grid;
        }



        private void btnRegresar_Clicked(object sender, EventArgs e)
{
    // Regresar a la página principal
    App.Current.MainPage = new PrincipalPage();
}
    }
}
