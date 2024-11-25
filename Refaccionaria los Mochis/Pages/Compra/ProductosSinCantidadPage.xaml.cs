using CapaEntidad;
using Microsoft.Maui.Controls;
using System.Collections.Generic;
using Refaccionaria_los_Mochis.Generic;
using Newtonsoft.Json;
namespace Refaccionaria_los_Mochis.Pages.Compra
{
    public partial class ProductosSinCantidadPage : ContentPage
    {
        private List<CompraDtl> _productosSinCantidad;

        public ProductosSinCantidadPage(List<CompraDtl> productosSinCantidad)
        {
            InitializeComponent();

            ProductosCollectionView.ItemsSource = productosSinCantidad;
        }




        private async void btnGuardarProductos_Clicked(object sender, EventArgs e)
        {
            var url = $"http://{IP.SERVIDOR}:5210/Compra/ActualizarCompraDtlIncompleto";

            // Obtener la lista de productos desde el CollectionView
            var productosSinCantidad = ProductosCollectionView.ItemsSource.Cast<CompraDtl>().ToList();
            string idUsuario = await SecureStorage.GetAsync("UserId");

            var datos = new
            {
                productosSinCantidad = productosSinCantidad,
                idUsuario = Convert.ToInt16(idUsuario)
            };

            // Enviar los datos a la Web API
            var (exito, mensaje) = await Http.EnviarDatosProductosSinCantidadAsync(datos, url);

            // Manejo de la respuesta
            if (exito)
            {
                await DisplayAlert("Éxito", "Datos actualizados correctamente.", "OK");
                App.Current.MainPage = new PrincipalPage();

            }
            else
            {
                await DisplayAlert("Error", $"Error al actualizar los datos: {mensaje}", "OK");
            }
        }





        private void btnRegresar_Clicked(object sender, EventArgs e)
        {
            App.Current.MainPage = new PrincipalPage();
        }









    }
}
