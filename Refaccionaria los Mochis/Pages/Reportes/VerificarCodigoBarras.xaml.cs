using CapaEntidad;
using Refaccionaria_los_Mochis.Generic;
namespace Refaccionaria_los_Mochis.Pages.Reportes;

public partial class VerificarCodigoBarras : ContentPage
{
	public VerificarCodigoBarras()
	{
		InitializeComponent();
        InitCodigosBarras();
    }
    public async Task InitCodigosBarras() // Cambiado a Task
    {
        var url = $"http://{IP.SERVIDOR}:5210/Reportes/SeleccionarProductosSinCodigoBarras";
        var info = await Http.VerificarCodigoBarras(url);

        if (info.Count != 0)
        {
            CardContainer.Clear()   ; 
            CardContainer.Children.Clear() ;
            
            foreach (var item in info)
            {
                var card = CreateCard(item);
                CardContainer.Children.Add(card);
            }
        }
    }


    private View CreateCard(Producto data) // Cambié el tipo a Producto en lugar de List<Producto>
    {
        var frame = new Frame
        {
            BackgroundColor = Colors.Gray,
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
                    Text = $"NoParte: {data.NoParte} \n" +
                           $"Descripcion: {data.Descripcion} \n" +
                           $"CodigoBarras: {data.CodigoBarras} \n" +
                           $"Rack: {data.oRack.Ubicacion} \n" +
                           $"Seccion: {data.oSeccion.Ubicacion} \n" +
                           $"Marca: {data.oMarca.Descripcion} \n", // Asumiendo que Marca tiene una propiedad Valor
                    FontSize = 18,
                    FontAttributes = FontAttributes.Bold,
                    VerticalTextAlignment = TextAlignment.Center,
                    TextColor= Colors.White
                },
                new Button
                {
                    Text = "Si Ocupa",
                    Command = new Command(() => OnButtonClicked(data.IdProducto)),
                    BackgroundColor = Colors.Green,
                    BorderColor= Colors.White
                },
                new Button
                {
                    Text = "No Ocupa",
                    Command = new Command(() => OnButtonClicked2(data.IdProducto)),
                    BackgroundColor = Colors.Red,
                    BorderColor= Colors.White

                }
            }
            }
        };

        return frame;
    }

    private async void OnButtonClicked(int data)
    {
        var url = $"http://{IP.SERVIDOR}:5210/Reportes/RegistrarCodigoBarras";

        bool mensaje = await Http.RegistrarCodigoBarrasAsync(url, data, true);
        InitCodigosBarras();
    }
    private async void OnButtonClicked2(int data)
    {
        var url = $"http://{IP.SERVIDOR}:5210/Reportes/RegistrarCodigoBarras";

        bool mensaje = await Http.RegistrarCodigoBarrasAsync(url, data, false);
        InitCodigosBarras();
    }


    private void btnRegresar_Clicked(object sender, EventArgs e)
    {
        // Regresar a la página principal
        App.Current.MainPage = new PrincipalPage();
    }
}