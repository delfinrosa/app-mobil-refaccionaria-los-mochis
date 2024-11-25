using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Maui.Controls;
using CapaEntidad;
using Refaccionaria_los_Mochis.Generic;
using Newtonsoft.Json;


namespace Refaccionaria_los_Mochis.Pages.Compra
{
    public partial class CompraDetallesPage : ContentPage
    {
        public List<EstadoProducto> _estadoProductos;

        public CompraDetallesPage(Dictionary<string, object> navigationParameter)
        {
            InitializeComponent();

            // Obtener la lista de detalles de compra
            var detallesCompra = (List<CompraDtl>)navigationParameter["detallesCompra"];
            DetallesCollectionView.ItemsSource = detallesCompra;

            // Inicializar la lista con EstadoProducto y estado falso
            _estadoProductos = detallesCompra
                .Select(x => new EstadoProducto
                {
                    CompraDtlId = x.CompraDtlId.ToString(),
                    IsPrecioChecked = false,
                    IsCantidadChecked = false
                })
                .ToList();
        }

        private void OnPrecioCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                var grid = checkBox.Parent as Grid;
                if (grid != null)
                {
                    // Obtener el CompraDtlId a partir del contexto de datos
                    var item = grid.BindingContext as CompraDtl;
                    if (item != null)
                    {
                        var CompraDtlId = item.CompraDtlId.ToString();

                        // Actualizar la lista
                        var index = _estadoProductos.FindIndex(p => p.CompraDtlId == CompraDtlId);
                        if (index >= 0)
                        {
                            _estadoProductos[index] = new EstadoProducto
                            {
                                CompraDtlId = _estadoProductos[index].CompraDtlId,
                                IsPrecioChecked = checkBox.IsChecked,
                                IsCantidadChecked = _estadoProductos[index].IsCantidadChecked
                            };
                        }
                    }
                }
            }
        }

        private void OnCantidadCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (sender is CheckBox checkBox)
            {
                var grid = checkBox.Parent as Grid;
                if (grid != null)
                {
                    // Obtener el CompraDtlId a partir del contexto de datos
                    var item = grid.BindingContext as CompraDtl;
                    if (item != null)
                    {
                        var CompraDtlId = item.CompraDtlId.ToString();

                        // Actualizar la lista
                        var index = _estadoProductos.FindIndex(p => p.CompraDtlId == CompraDtlId);
                        if (index >= 0)
                        {
                            _estadoProductos[index] = new EstadoProducto
                            {
                                CompraDtlId = _estadoProductos[index].CompraDtlId,
                                IsPrecioChecked = _estadoProductos[index].IsPrecioChecked,
                                IsCantidadChecked = checkBox.IsChecked
                            };
                        }
                    }
                }
            }
        }



        private async void btnGuardar_Clicked(object sender, EventArgs e)
        {
            var url = $"http://{IP.SERVIDOR}:5210/Compra/ActualizarCompraDtl";
            string idUsuario = await SecureStorage.GetAsync("UserId");

            var datos = new
            {
                estadoProductos = _estadoProductos,
                idUsuario = Convert.ToInt16(idUsuario)
            };

            var (exito, mensaje) = await Http.EnviarDatosCompraDtlAsync(datos, url);

            // Manejo de la respuesta
            if (exito)
            {
                await DisplayAlert("Éxito", "Datos actualizados correctamente.", "OK");

                // Filtrar los productos sin cantidad marcada
                var productosSinCantidad = _estadoProductos
                    .Where(estadoProducto => !estadoProducto.IsCantidadChecked)
                    .Select(estadoProducto => DetallesCollectionView.ItemsSource.Cast<CompraDtl>()
                        .FirstOrDefault(x => x.CompraDtlId.ToString() == estadoProducto.CompraDtlId))
                    .Where(producto => producto != null)
                    .ToList();

                // Verificar si la lista no está vacía
                if (productosSinCantidad.Count > 0)
                {
                    // Crear instancia de ProductosSinCantidadPage directamente con la lista
                    var detallesPage = new ProductosSinCantidadPage(productosSinCantidad);
                    App.Current.MainPage = new NavigationPage(detallesPage);

                }
                else
                {
                    await DisplayAlert("Información", "No hay productos sin cantidad marcada.", "OK");
                    App.Current.MainPage = new PrincipalPage();

                }
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
