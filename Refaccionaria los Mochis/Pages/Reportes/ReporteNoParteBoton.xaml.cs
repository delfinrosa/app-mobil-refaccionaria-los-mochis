using System.Collections.ObjectModel;

using Refaccionaria_los_Mochis.Generic;
using Refaccionaria_los_Mochis.Pages.Detalles;
using System.Threading.Tasks;

namespace Refaccionaria_los_Mochis.Pages.Reportes;

public partial class ReporteNoParteBoton : ContentPage
{
    public ObservableCollection<Opcion> Opciones { get; set; } = new ObservableCollection<Opcion>();

    public ReporteNoParteBoton()
    {
        InitializeComponent();
        this.BindingContext = this;
        InicializarOpcionesAsync();
    }

    private async Task InicializarOpcionesAsync()
    {
        var url = $"http://" + IP.SERVIDOR + ":5210/Reportes/ObtenerLineas";
        var lineasProductos = await Http.GetLineasAsync(url);
        foreach (var linea in lineasProductos)
        {
            var opcion = new Opcion { Titulo = $"{linea.Item1} [+] ({linea.Item2})" };
            Opciones.Add(opcion);
        }
        ConstruirInterfaz();
    }

    private void ConstruirInterfaz()
    {
        MainStackLayout.Children.Clear();
        foreach (var opcion in Opciones)
        {
            var mainButton = new Button { Text = opcion.Titulo, Style = (Style)Resources["MainButtonStyle"] };
            mainButton.Clicked += MainOption_Clicked;
            mainButton.BindingContext = opcion;
            MainStackLayout.Children.Add(mainButton);

            var subStackLayout = new StackLayout { IsVisible = false };
            subStackLayout.BindingContext = opcion;
            subStackLayout.ClassId = "subStackLayout";
            MainStackLayout.Children.Add(subStackLayout);
        }
    }

    private async void MainOption_Clicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var opcion = button.BindingContext as Opcion;
        var subStackLayout = GetSubStackLayout(button);

        if (subStackLayout.Children.Count == 0)
        {
            var descripcionLinea = System.Text.RegularExpressions.Regex.Match(opcion.Titulo, @"^(.+?)\s\[\+\]").Groups[1].Value;
            var url = $"http://" + IP.SERVIDOR + ":5210/Reportes/ObtenerDescripciones?descripcionLinea=" + descripcionLinea;
            var subOpciones = await Http.GetDescripcionesAsync(url);

            foreach (var sub in subOpciones)
            {
                var subButton = new Button { Text = $"{sub.Item1} [+] ({sub.Item2})", Style = (Style)Resources["SubButtonStyle"] };
                subButton.Clicked += SubOption_Clicked;
                subButton.BindingContext = sub;
                subStackLayout.Children.Add(subButton);

                var itemStackLayout = new StackLayout { IsVisible = false };
                itemStackLayout.ClassId = "itemStackLayout";
                subStackLayout.Children.Add(itemStackLayout);
            }
        }

        subStackLayout.IsVisible = !subStackLayout.IsVisible;
    }

    private async void SubOption_Clicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var subOpcion = button.BindingContext as Tuple<string, int>;
        var subStackLayout = button.Parent as StackLayout;
        var itemStackLayout = GetItemStackLayout(button);

        if (itemStackLayout.Children.Count == 0)
        {
            var url = $"http://" + IP.SERVIDOR + ":5210/Reportes/ObtenerMarcas?descripcionProducto=" + subOpcion.Item1;
            var marcas = await Http.GetMarcasAsyncReporte(url);
            foreach (var marca in marcas)
            {
                var marcaButton = new Button
                {
                    Text = $"{marca.Item1} [+] ({marca.Item2})",
                    Style = (Style)Resources["ItemButtonStyle"]
                };
                SetDescripcionProducto(marcaButton, subOpcion.Item1);

                marcaButton.Clicked += MarcaOption_Clicked;
                marcaButton.BindingContext = new Tuple<string, int>(marca.Item1, marca.Item2);
                itemStackLayout.Children.Add(marcaButton);

                var noParteStackLayout = new StackLayout { IsVisible = false };
                noParteStackLayout.ClassId = "noParteStackLayout";
                itemStackLayout.Children.Add(noParteStackLayout);
            }
        }

        itemStackLayout.IsVisible = !itemStackLayout.IsVisible;
    }

    private async void MarcaOption_Clicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var subStackLayout = button.Parent as StackLayout;
        var noParteStackLayout = GetNoParteStackLayout(button);

        if (noParteStackLayout.Children.Count == 0)
        {
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
                    var url = $"http://" + IP.SERVIDOR + $":5210/Reportes/ObtenerNumerosParte?descripcionProducto={descripcionProducto}&descripcionMarca={descripcionMarca}";

                    try
                    {
                        var noPartes = await Http.GetNumerosParteAsync(url);
                        noParteStackLayout.Children.Remove(loadingLabel);

        noParteStackLayout.IsVisible = !noParteStackLayout.IsVisible;
                        if (noPartes != null)
                        {
                            foreach (var noParte in noPartes)
                            {
                                
                                var noParteButton = new Button
                                {
                                    Text = noParte,
                                    Style = (Style)Resources["NoParteButtonStyle"]
                                };
                                SetMarca(noParteButton, descripcionMarca);
                                noParteButton.Clicked += NoParte_Clicked;
                                noParteStackLayout.Children.Add(noParteButton);
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

    private StackLayout GetSubStackLayout(Button button)
    {
        var parentStack = button.Parent as StackLayout;
        return parentStack.Children[parentStack.Children.IndexOf(button) + 1] as StackLayout;
    }

    private StackLayout GetItemStackLayout(Button button)
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
    BindableProperty.CreateAttached("DescripcionProducto", typeof(string), typeof(Code39), null);

    public static string GetDescripcionProducto(BindableObject view)
    {
        return (string)view.GetValue(DescripcionProductoProperty);
    }

    public static void SetDescripcionProducto(BindableObject view, string value)
    {
        view.SetValue(DescripcionProductoProperty, value);
    }

    public static readonly BindableProperty MarcaProperty =
    BindableProperty.CreateAttached("Marca", typeof(string), typeof(Code39), null);

    public static string GetMarca(BindableObject view)
    {
        return (string)view.GetValue(MarcaProperty);
    }

    public static void SetMarca(BindableObject view, string value)
    {
        view.SetValue(MarcaProperty, value);
    }

 

    private async void Button_Clicked(object sender, EventArgs e)
    {
        App.Current.MainPage = new PrincipalPage();
    }
}

public class Opcion
{
    public string Titulo { get; set; }
    public ObservableCollection<SubOpcion> SubOpciones { get; set; } = new ObservableCollection<SubOpcion>();
}

public class SubOpcion
{
    public string Titulo { get; set; }
    public ObservableCollection<string> Items { get; set; } = new ObservableCollection<string>();
}
