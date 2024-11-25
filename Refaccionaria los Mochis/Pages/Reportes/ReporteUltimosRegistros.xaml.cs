using Refaccionaria_los_Mochis.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
namespace Refaccionaria_los_Mochis.Pages.Reportes;

public partial class ReporteUltimosRegistros : ContentPage
{
	public ReporteUltimosRegistros()
	{
		InitializeComponent();
        CargarProductosRecientes();

    }


    private async void CargarProductosRecientes()
    {
        string url = $"http://{IP.SERVIDOR}:5210/Productos/SeleccionarProductosRecientes";
        var productos = await Http.GetProductosRecientesAsync(url);

        if (productos != null)
        {
            ProductosListView.ItemsSource = productos;
        }
        else
        {
            await DisplayAlert("Error", "No se pudieron cargar los productos.", "OK");
        }
    }


    private async void Button_Clicked(object sender, EventArgs e)
    {
        App.Current.MainPage = new PrincipalPage();

    }




    private async void Button_Clicked_1(object sender, EventArgs e)
    {

        bool isUsuarioChecked =  CheckBoxUsuario.IsChecked;
        bool isCantidadChecked = CheckBoxCantidad.IsChecked;

        if (isUsuarioChecked && isCantidadChecked)
        {

            string url = $"http://{IP.SERVIDOR}:5210/Reportes/SeleccionarProductosRecientesAvanzadoTodo?cantidad=" +BusquedaCantidadEntry.Text+ "&idusuario="+BusquedaUsuarioaEntry.Text;
            var productos = await Http.GetProductosRecientesusuario(url);

            if (productos != null)
            {
                ProductosListView.ItemsSource = productos;
            }
            else
            {
                await DisplayAlert("Error", "No se pudieron cargar los productos con el filtro de usuario y cantidad.", "OK");
            }
        }
        else if (isUsuarioChecked)
        {
            string url = $"http://{IP.SERVIDOR}:5210/Reportes/SeleccionarProductosRecientesAvanzadoUsuario?idusuario=" + BusquedaUsuarioaEntry.Text;
            var productos = await Http.GetProductosRecientesusuario(url);

            if (productos != null)
            {
                ProductosListView.ItemsSource = productos;
            }
            else
            {
                await DisplayAlert("Error", "No se pudieron cargar los productos con el filtro de usuario .", "OK");
            }
        }
        else if (isCantidadChecked)
        {
            string url = $"http://{IP.SERVIDOR}:5210/Reportes/SeleccionarProductosRecientesAvanzadoCantidad?cantidad=" + BusquedaCantidadEntry.Text ;
            var productos = await Http.GetProductosRecientesusuario(url);

            if (productos != null)
            {
                ProductosListView.ItemsSource = productos; 
            }
            else
            {
                await DisplayAlert("Error", "No se pudieron cargar los productos con el filtro de cantidad.", "OK");
            }
        }
        else
        {
            string url = $"http://{IP.SERVIDOR}:5210/Productos/SeleccionarProductosRecientes";
            var productos = await Http.GetProductosRecientesAsync(url);

            if (productos != null)
            {
                ProductosListView.ItemsSource = productos;
            }
            else
            {
                await DisplayAlert("Error", "No se pudieron cargar los productos.", "OK");
            }
        }
    }
}