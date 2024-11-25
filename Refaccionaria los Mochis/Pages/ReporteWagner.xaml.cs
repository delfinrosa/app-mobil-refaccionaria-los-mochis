using Refaccionaria_los_Mochis.Generic;
namespace Refaccionaria_los_Mochis.Pages;

public partial class ReporteWagner : ContentPage
{

    private Button btnRegresar;

    public ReporteWagner()
    {
        InitializeComponent();
        btnRegresar = new Button
        {
            Text = "Regresar",
            Margin = new Thickness(0, 0, 0, 30)
        };
        btnRegresar.Clicked += btnRegresar_Clicked;
        LoadData();
    }

    private async Task LoadData()
    {
        try
        {
            // URL para obtener los números de parte ordenados
            string url = $"http://{IP.SERVIDOR}:5210/Reportes/NumerosParteORDENADO";

            // Llamada al método estático para obtener los números de parte
            var numerosParte = await Http.GetNumerosParteORDENADO(url);

            // Limpiar el contenido existente antes de agregar nuevos elementos
            numeroParteStackLayout.Children.Clear();

            // Crear etiquetas para cada número de parte y agregarlas al StackLayout
            foreach (var numeroParte in numerosParte)
            {
                var label = new Label
                {
                    Text = numeroParte,
                    VerticalOptions = LayoutOptions.Center,
                    FontSize = 22 // Establecer el tamaño de la fuente a 22px
                };

                numeroParteStackLayout.Children.Add(label);
            }

            // Asegurarse de que el botón Regresar siempre esté al final
            numeroParteStackLayout.Children.Add(btnRegresar);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al cargar datos: {ex.Message}");
            // Manejar el error según sea necesario (mostrar mensaje de error, etc.)
        }
    }

    private void btnRegresar_Clicked(object sender, EventArgs e)
    {
        // Manejo del evento de clic para el botón de regreso
        // Puedes definir lo que necesitas que haga el botón de regresar
        Navigation.PopAsync();
    }
}