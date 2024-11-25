using System.Collections.ObjectModel;
using Refaccionaria_los_Mochis.Generic;
namespace Refaccionaria_los_Mochis.Pages;

public partial class Code39 : ContentPage
{
    public ObservableCollection<Opcion> Opciones { get; set; } = new ObservableCollection<Opcion>();

    public Code39()
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
            // Usa una expresión regular para extraer la descripción completa antes del primer " [+]"
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
            //var marcas = await Http.GetMarcasAsync(subOpcion.Item1);

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
                // Crear un nuevo Tuple<string, int> con los valores del BindingContext
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
            // Obtener el texto completo del botón de subopción
            var textoCompleto = button.Text;

            // Encontrar el índice del primer corchete "["
            var indiceInicio = textoCompleto.IndexOf('[');

            if (indiceInicio != -1)
            {
                // Extraer la descripción del producto desde el inicio hasta el corchete "["
                var descripcionProducto = GetDescripcionProducto(button);

                var marca = button.BindingContext as Tuple<string, int>; // Contexto de enlace es un Tuple<string, int>
                if (marca != null && !string.IsNullOrWhiteSpace(descripcionProducto))
                {
                    var descripcionMarca = marca.Item1; // La descripción de la marca es el primer elemento del Tuple

                    // Construir la URL con la descripción del producto y la descripción de la marca
                    var url = $"http://" + IP.SERVIDOR + $":5210/Reportes/ObtenerNumerosParte?descripcionProducto={descripcionProducto}&descripcionMarca={descripcionMarca}";

                    var noPartes = await Http.GetNumerosParteAsync(url);

                    foreach (var noParte in noPartes)
                    {
                        var label = new Label { Text = noParte, Style = (Style)Resources["NoParteLabelStyle"] };
                        noParteStackLayout.Children.Add(label);
                    }
                }
                else
                {
                    // Manejar el caso en que el contexto de enlace de la marca o la descripción del producto sean nulos o vacíos
                }
            }
            else
            {
                // Manejar el caso en que el corchete "[" no esté presente en el texto del botón
            }
        }

        noParteStackLayout.IsVisible = !noParteStackLayout.IsVisible;
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
    private async void Actualizar_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new Code39());
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

