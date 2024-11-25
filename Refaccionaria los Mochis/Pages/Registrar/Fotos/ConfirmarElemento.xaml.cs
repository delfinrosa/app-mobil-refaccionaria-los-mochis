using CapaEntidad;
namespace Refaccionaria_los_Mochis.Pages.Registrar.Fotos;

public partial class ConfirmarElemento : ContentPage
{
    public ConfirmarElemento(List<Producto> productos)
    {
        InitializeComponent();
        InitProductos(productos);

    }

    private void InitProductos(List<Producto> data)
    {
        foreach (var item in data)
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
                            Text = $"NoParte: {item.NoParte} Descripcion: {item.Descripcion} Precio: {item.Precio}$$ Marca: {item.oMarca.Descripcion} Rack: {item.oRack.Ubicacion} Seccion: {item.oSeccion.Ubicacion} Aplicacion:{item.Valor}",
                            FontSize = 18,
                            FontAttributes = FontAttributes.Bold,
                            VerticalTextAlignment = TextAlignment.Center
                        },
                        new Button
                        {
                            Text = "Guardar Imagen",
                            HorizontalOptions = LayoutOptions.Center,
                            VerticalOptions = LayoutOptions.Center,
                            BackgroundColor = Color.FromHex("#808080"),
                            TextColor = Color.FromHex("#FFFFFF"),
                            BorderColor = Color.FromHex("#FCC208"),
                            BorderWidth = 2,
                            Command = new Command(async () =>
                            {
                                var tomarFoto = new TomarFoto(item.IdProducto);
                                App.Current.MainPage = tomarFoto;
                            })
                        }
                    }
                }
            };
            CardsContainer.Children.Add(frame);


        }
    }

    private void btnRegresar_Clicked(object sender, EventArgs e)
    {
        App.Current.MainPage = new PrincipalPage();

    }
}