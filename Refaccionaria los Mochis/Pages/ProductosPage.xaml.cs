using Refaccionaria_los_Mochis.Models;
using Refaccionaria_los_Mochis.Generic;
using CapaEntidad;
namespace Refaccionaria_los_Mochis.Pages;

public partial class ProductosPage : ContentPage
{

    public ProductoModel oProductoModel { get; set; }

    public ProductosPage()
    {
        InitializeComponent();
        oProductoModel = new ProductoModel
        {
            lista = new List<Producto>()
        };
        BindingContext = this;
        listarProducto();
    }
    private async void listarProducto()
    {
        try
        {
            var productos = await Http.GetAll("http://192.168.10.103:5210/Productos/SeleccionarTodos");
            oProductoModel.lista = productos;
            OnPropertyChanged(nameof(oProductoModel));
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"No se pudo obtener la lista de productos: {ex.Message}", "OK");
        }
    }

    private void toolbarItemRegresar_Clicked(object sender, EventArgs e)
    {
        App.Current.MainPage = new PrincipalPage();
    }
}