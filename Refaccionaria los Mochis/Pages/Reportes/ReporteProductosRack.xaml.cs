using CapaEntidad;
using Refaccionaria_los_Mochis.Generic;
using System.Collections.ObjectModel;
using Refaccionaria_los_Mochis.Pages.Detalles;

namespace Refaccionaria_los_Mochis.Pages.Reportes;


public partial class ReporteProductosRack : ContentPage
{
    public ObservableCollection<DescripcionProducto> Descripciones { get; set; } = new ObservableCollection<DescripcionProducto>();
    public ObservableCollection<Opcion> Opciones { get; set; } = new ObservableCollection<Opcion>();

    private string _rack;

    public ReporteProductosRack(Dictionary<string, object> navigationParameter)
    {
        InitializeComponent();

        if (navigationParameter != null && navigationParameter.ContainsKey("Rack"))
        {
            var rack = navigationParameter["Rack"] as string;

            if (!string.IsNullOrEmpty(rack))
            {
                lblRack.Text = "Productos del el Rack: "+ rack;
                _rack = rack;
                this.BindingContext = this;
                InicializarDescripcionesRackAsync();
            }
        }
    }



    private async Task InicializarDescripcionesRackAsync()
    {
        // Construir la URL para la solicitud HTTP
        var url = $"http://{IP.SERVIDOR}:5210/Reportes/ObtenerDescripcionesRack?rack={_rack}";

        // Obtener los datos del servidor
        var descripcionesProductos = await Http.GetReporteRack1Async(url);

        MainStackLayout.Children.Clear();

        // Verificar si se obtuvieron datos
        if (descripcionesProductos != null && descripcionesProductos.Count > 0)
        {
            // Agregar las descripciones a la colección
            foreach (var descripcion in descripcionesProductos)
            {
                // Crear un nuevo objeto DescripcionProducto y agregarlo a la colección
                var descripcionProducto = new Button
                {
                    Text = $"{descripcion.Item1} [+] ({descripcion.Item2})",
                    Style = (Style)Resources["SubButtonStyle"]
                };
                descripcionProducto.Clicked += DescripcionOption_Clicked;
                descripcionProducto.BindingContext = descripcion;

                MainStackLayout.Children.Add(descripcionProducto);

                var marcasStackLayout = new StackLayout
                {
                    IsVisible = false
                };

                //marcasStackLayout.BindingContext = descripcion;

                descripcionProducto.ClassId = "itemStackLayout";

                MainStackLayout.Children.Add(marcasStackLayout);
            }

        }
        else
        {
            // Mostrar un mensaje si no se encontraron datos
            await DisplayAlert("Información", "No se encontraron descripciones para este rack.", "OK");
        }
    }






    private async void DescripcionOption_Clicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var descripcion = button.BindingContext as Tuple<string, int>;
        var subStackLayout = button.Parent as StackLayout;
        var marcasStackLayout = GetMarcasStackLayout(button);

        if (marcasStackLayout.Children.Count == 0)
        {
            var url = $"http://{IP.SERVIDOR}:5210/Reportes/ObtenerMarcasRack?descripcionProducto={descripcion.Item1}&ubicacionRack={_rack}";
            var marcas = await Http.GetMarcasAsyncReporteRack(url);
            foreach (var marca in marcas)
            {
                var marcaButton = new Button
                {
                    Text = $"{marca.Item1} [+] ({marca.Item2})",
                    Style = (Style)Resources["ItemButtonStyle"]
                };
                SetDescripcionProducto(marcaButton, descripcion.Item1);

                marcaButton.Clicked += MarcaOption_Clicked;
                marcaButton.BindingContext = new Tuple<string, int>(marca.Item1, marca.Item2);
                marcasStackLayout.Children.Add(marcaButton);

                var noParteStackLayout = new StackLayout { IsVisible = false };
                noParteStackLayout.ClassId = "noParteStackLayout";
                marcasStackLayout.Children.Add(noParteStackLayout);
            }
        }

        marcasStackLayout.IsVisible = !marcasStackLayout.IsVisible;
    }


    private async void MarcaOption_Clicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var subStackLayout = button.Parent as StackLayout;
        var noParteStackLayout = GetNoParteStackLayout(button);
        var loadingLabel = new Label { Text = "Cargando...", Style = (Style)Resources["LoadingLabelStyle"] };
        noParteStackLayout.Children.Add(loadingLabel);


        var textoCompleto = button.Text;
        var indiceInicio = textoCompleto.IndexOf('[');

        if (indiceInicio != -1)
        {
            var descripcionProducto = GetDescripcionProducto(button);
            var marca = button.BindingContext as Tuple<string, int>;

            if (marca != null && !string.IsNullOrWhiteSpace(descripcionProducto))
            {
                var descripcionMarca = marca.Item1;

                var url = $"http://{IP.SERVIDOR}:5210/Reportes/ObtenerNumerosParteRack?descripcionProducto={descripcionProducto}&descripcionMarca={descripcionMarca}&ubicacionRack={_rack}";

                try
                {
                    noParteStackLayout.Children.Remove(loadingLabel);
                    noParteStackLayout.IsVisible = !noParteStackLayout.IsVisible;
                    var noPartes = await Http.GetNumerosParteRack(url);

                    if (noPartes != null)
                    {
                        foreach (var noParte in noPartes)
                        {
                            // Crear el layout horizontal para el botón y el círculo del stock
                            var stackLayout = new StackLayout
                            {
                                Orientation = StackOrientation.Horizontal,
                                Spacing = 10 // Espacio entre el texto y el círculo
                            };

                            // Crear el botón con el número de parte
                            var noParteButton = new Button
                            {
                                Text = noParte.Item1, // noParte.Item1 es el número de parte
                                Style = (Style)Resources["NoParteButtonStyle"]
                            };
                            noParteButton.Clicked += NoParte_Clicked;

                            Color color = Color.FromRgb(250, 250, 250);
                            if (noParte.Item3 == 1)
                            {
                                color = Color.FromRgb(250, 0, 0);
                            }
                            else if (noParte.Item3 == 2)
                            {
                                color = Color.FromRgb(250, 190, 0);
                            }
                            else
                            {
                                color = Color.FromRgb(30, 130, 70);
                            }

                            var stockFrame = new Frame
                            {
                                BackgroundColor = color,
                                CornerRadius = 50, // Esto hace que sea un círculo
                                Padding = new Thickness(6),
                                HeightRequest = 36,
                                WidthRequest = 36,
                                VerticalOptions = LayoutOptions.Center,
                                HorizontalOptions = LayoutOptions.Center
                            };

                            // Crear el Label para mostrar el stock dentro del círculo
                            var stockLabel = new Label
                            {
                                Text = noParte.Item2.ToString(), // noParte.Item2 es el stock
                                TextColor = Color.FromRgb(250, 250, 250),
                                HorizontalOptions = LayoutOptions.Center,
                                VerticalOptions = LayoutOptions.Center,
                                FontAttributes = FontAttributes.Bold,
                                FontSize = 18
                            };

                            // Agregar el Label al Frame (círculo)
                            stockFrame.Content = stockLabel;

                            // Agregar el botón y el círculo al StackLayout
                            stackLayout.Children.Add(noParteButton);
                            stackLayout.Children.Add(stockFrame);

                            // Agregar el StackLayout al contenedor
                            noParteStackLayout.Children.Add(stackLayout);
                            SetMarca(noParteButton, descripcionMarca);

                            await Task.Delay(200);
                        }
                    }


                    else
                    {
                        await DisplayAlert("Error", "No se pudieron cargar los números de parte.", "OK");
                    }
                }
                catch (Exception ex)
                {
                    noParteStackLayout.Children.Remove(loadingLabel);
                    await DisplayAlert("Error", $"Error al obtener los números de parte: {ex.Message}", "OK");
                }
            }
            else
            {
                noParteStackLayout.Children.Remove(loadingLabel);
                await DisplayAlert("Error", "Descripción del producto o marca no válida.", "OK");
            }
        }
        else
        {
            noParteStackLayout.Children.Remove(loadingLabel);
            await DisplayAlert("Error", "Formato del texto del botón no válido.", "OK");
        }

    }



    private async void NoParte_Clicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var numeroParte = button.Text;
        var marca = GetMarca(button);

        var url = $"http://{IP.SERVIDOR}:5210/Productos/SeleccionarNoparteMarca?noparte={numeroParte}&marca={marca}";
        var producto = await Http.Get(url);

        if (producto != null && producto.Descripcion != null)
        {
            var navigationParameter = new Dictionary<string, object>
                {
                    { "producto", producto }
                };

            var detallesPage = new DetalleProductoNoParteMarca(navigationParameter);
            App.Current.MainPage = detallesPage;
        }
        else
        {
            await DisplayAlert("Error", "No se pudo obtener el producto.", "OK");
        }
    }

    private StackLayout GetMarcasStackLayout(Button button)
    {
        var parentStack = button.Parent as StackLayout;
        return parentStack.Children[parentStack.Children.IndexOf(button) + 1] as StackLayout;
    }

    private StackLayout GetNoParteStackLayout(Button button)
    {
        var parentStack = button.Parent as StackLayout;
        return parentStack.Children[parentStack.Children.IndexOf(button) + 1] as StackLayout;
    }

    public static readonly BindableProperty DescripcionProductoProperty =
        BindableProperty.CreateAttached("DescripcionProducto", typeof(string), typeof(ReporteProductosRack), null);

    public static string GetDescripcionProducto(BindableObject view)
    {
        return (string)view.GetValue(DescripcionProductoProperty);
    }

    public static void SetDescripcionProducto(BindableObject view, string value)
    {
        view.SetValue(DescripcionProductoProperty, value);
    }

    public static readonly BindableProperty MarcaProperty =
        BindableProperty.CreateAttached("Marca", typeof(string), typeof(ReporteProductosRack), null);

    public static string GetMarca(BindableObject view)
    {
        return (string)view.GetValue(MarcaProperty);
    }

    public static void SetMarca(BindableObject view, string value)
    {
        view.SetValue(MarcaProperty, value);
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        App.Current.MainPage = new PrincipalPage();

    }
}

public class DescripcionProducto
{
    public string Titulo { get; set; }
}
