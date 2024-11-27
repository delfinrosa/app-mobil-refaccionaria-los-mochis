using CapaEntidad;
using CommunityToolkit.Maui.Core.Platform;
using Newtonsoft.Json;
using Refaccionaria_los_Mochis.Generic;
using Refaccionaria_los_Mochis.Pages.Registrar;

namespace Refaccionaria_los_Mochis.Pages;

public partial class CrearProducto : ContentPage
{

    Producto ProductoActual = new Producto();

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        string userId = await SecureStorage.GetAsync("UserId");

        if (!string.IsNullOrEmpty(userId))
        {
            int idUsuario = int.Parse(userId);
            if (idUsuario <= 0)
            {
                App.Current.MainPage = new login();
            }
        }
        else
        {
            App.Current.MainPage = new login();

        }
    }


    public CrearProducto(Dictionary<string, object> navigationParameter)
    {
        InitializeComponent();
        InitializeProduct(navigationParameter);
    }

    private async void InitializeProduct(Dictionary<string, object> navigationParameter)
    {
        if (navigationParameter != null && navigationParameter.ContainsKey("producto"))
        {
            var producto = navigationParameter["producto"] as Producto;
            if (producto != null)
            {
                ProductoActual = producto;
                BindingContext = ProductoActual;
            }
        }

        if (ProductoActual.oAlmacen == null)
        {
            ProductoActual.oAlmacen = new Almacen();
        }
        ProductoActual.oAlmacen.AlmacenId = 1;

        if (ProductoActual.oMarca == null)
        {
            ProductoActual.oMarca = new Marca();
        }
        ProductoActual.oMarca.IdMarca = 0;

        if (ProductoActual.oLinea == null)
        {
            ProductoActual.oLinea = new Linea();
        }
        ProductoActual.oLinea.IdLinea = 14;

        if (ProductoActual.oRack == null)
        {
            ProductoActual.oRack = new AlmacenRack();
        }
        ProductoActual.oRack.RackId = 28;

        var url = $"http://{IP.SERVIDOR}:5210/AlmacenRackSeccion/ObtenerRackSeccion?rackId={ProductoActual.oRack.RackId}";
        var marca = await Http.GetAlmacenRackSeccion(url);
        SeccionListView.ItemsSource = marca;
        SeccionListView.IsVisible = true;
        BusquedaAlmacenEntry.IsVisible = true;
    }











    private void btnRegresar_Clicked(object sender, EventArgs e)
    {
        App.Current.MainPage = new PrincipalPage();
    }

    private async void GuardarProducto_Clicked(object sender, EventArgs e)
    {
        string idCompra = string.Empty;

        try
        {
            if (string.IsNullOrEmpty(PrecioEntry.Text))
            {
                await DisplayAlert("Error", "El campo Precio es obligatorio.", "OK");
                return;
            }
            else if (string.IsNullOrEmpty(DescripcionEntry.Text))
            {
                await DisplayAlert("Error", "El campo Descripción es obligatorio.", "OK");
                return;
            }
            else if (string.IsNullOrEmpty(MinimoEntry.Text))
            {
                await DisplayAlert("Error", "El campo Mínimo es obligatorio.", "OK");
                return;
            }
            else if (string.IsNullOrEmpty(MaximoEntry.Text))
            {
                await DisplayAlert("Error", "El campo Máximo es obligatorio.", "OK");
                return;
            }
            else if (string.IsNullOrEmpty(NoParteEntry.Text))
            {
                await DisplayAlert("Error", "El campo Número de Parte es obligatorio.", "OK");
                return;
            }
            else if (string.IsNullOrEmpty(CodigoBarras.Text))
            {
                await DisplayAlert("Error", "El campo Código de Barras es obligatorio.", "OK");
                return;
            }
            //else if (string.IsNullOrEmpty(CantidadEntry.Text))
            //{
            //    await DisplayAlert("Error", "El campo Cantidad es obligatorio.", "OK");
            //    return;
            //}
            else if (ProductoActual.oMarca.IdMarca == 0)
            {
                await DisplayAlert("Error", "Seleccione una marca.", "OK");
                return;
            }
            else if (ProductoActual.oLinea.IdLinea == 0)
            {
                await DisplayAlert("Error", "Seleccione una línea.", "OK");
                return;
            }

            // Asignar valores al objeto ProductoActual
            ProductoActual.CodigoBarras = CodigoBarras.Text.ToUpper();
            ProductoActual.Descripcion = DescripcionEntry.Text.ToUpper();
            ProductoActual.Precio = string.IsNullOrWhiteSpace(PrecioEntry.Text) ? 0 : decimal.Parse(PrecioEntry.Text);
            ProductoActual.Minimo = string.IsNullOrWhiteSpace(MinimoEntry.Text) ? 0 : int.Parse(MinimoEntry.Text);
            ProductoActual.Maximo = string.IsNullOrWhiteSpace(MaximoEntry.Text) ? 0 : int.Parse(MaximoEntry.Text);
            ProductoActual.NoParte = NoParteEntry.Text.ToUpper();
            ProductoActual.IdProducto = 0; // Asumiendo que es nuevo
            ProductoActual.Activo = "A"; // Activo por defecto
            ProductoActual.Valor = "PENDIENTE"; // Valor por defecto

            // Verificar y obtener el ID de usuario
            if (ProductoActual.oUsuario == null)
            {
                ProductoActual.oUsuario = new Usuario();
            }

            string userId = await SecureStorage.GetAsync("UserId");
            if (!string.IsNullOrEmpty(userId))
            {
                ProductoActual.oUsuario.IdUsuario = int.Parse(userId);
            }
            else
            {
                await DisplayAlert("Error", "No se pudo obtener el ID de usuario.", "OK");
                App.Current.MainPage = new login();
                return;
            }

            // Obtener el ID de compra escaneado
            //idCompra = await SecureStorage.GetAsync("Compra");
            //if (string.IsNullOrEmpty(idCompra))
            //{
            //    await DisplayAlert("Error", "No se pudo obtener el ID de compra.", "OK");
            //    App.Current.MainPage = new ScanQR();
            //    return;
            //}

            // Construir el objeto de datos a enviar
            var data = new
            {
                objeto = ProductoActual,
                //cantidad = int.Parse(CantidadEntry.Text),
                //idcompra = idCompra
            };

            var url = $"http://{IP.SERVIDOR}:5210/Productos/GuardarProducto";

            var (idGenerado, mensaje) = await Http.PostDataAsync(data, url);

            if (idGenerado > 0)
            {
                await DisplayAlert("Éxito", "Producto registrado correctamente.", "OK");
                LimpiarCampos();
                App.Current.MainPage = new LeerProductoInsert();
            }
            else
            {
                await DisplayAlert("Error", $"No se pudo registrar el producto. Detalle: {mensaje}", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Ocurrió un error: {ex.Message}", "OK");
        }
    }
    private void LimpiarCampos()
    {
        DescripcionEntry.Text = string.Empty;
        PrecioEntry.Text = string.Empty;
        MinimoEntry.Text = string.Empty;
        MaximoEntry.Text = string.Empty;
        NoParteEntry.Text = string.Empty;
        CodigoBarras.Text = string.Empty;
    }
    private async void Button_Clicked(object sender, EventArgs e)
    {

        var nombre = BusquedaMarcaEntry.Text;

        if (!string.IsNullOrEmpty(nombre))
        {
            try
            {
                var url = $"http://{IP.SERVIDOR}:5210/Marcas/SeleccionarDescripcion?nombre={nombre}";
                var (marcas, mensaje) = await Http.GetMarca(url);

                if (!string.IsNullOrEmpty(mensaje))
                {
                    var result = await DisplayAlert("Error", mensaje, "Continuar", "Registrar");

                    if (result)
                    {
                        BusquedaMarcaEntry.Text = "";
                    }
                    else
                    {
                        App.Current.MainPage = new RegistrarMarca();
                    }

                }
                else
                {
                    MarcasListView.ItemsSource = marcas;
                    MarcasListView.IsVisible = true;
                    if (KeyboardExtensions.IsSoftKeyboardShowing(BusquedaMarcaEntry))
                    {
                        await KeyboardExtensions.HideKeyboardAsync(BusquedaMarcaEntry, default);
                    }
                    else
                    {
                        await KeyboardExtensions.ShowKeyboardAsync(BusquedaMarcaEntry, default);
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al obtener las marcas: {ex.Message}", "OK");
            }
        }
        else
        {
            await DisplayAlert("Error", "Debe ingresar un nombre de marca.", "OK");
        }
    }

    private void MarcasListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {

        try
        {
            if (e.SelectedItem is Marca selectedMarca)
            {

                if (ProductoActual.oMarca == null)
                {
                    ProductoActual.oMarca = new Marca();
                }
                ProductoActual.oMarca.IdMarca = selectedMarca.IdMarca;

                BusquedaMarcaEntry.Text = selectedMarca.Descripcion;

                MarcasListView.IsVisible = false;
                BusquedaMarcaEntry.IsVisible = true;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al seleccionar la marca: {ex.Message}");
            DisplayAlert("Error", $"Ocurrió un error: {ex.Message}", "OK");
        }
    }
    private async void BtnLineaBusqueda_Clicked(object sender, EventArgs e)
    {
        var nombre = BusquedaLineaEntry.Text;
        if (!string.IsNullOrEmpty(nombre))
        {
            try
            {
                var url = $"http://{IP.SERVIDOR}:5210/Lineas/SeleccionarDescripcion?nombre={nombre}";
                var (lineas, mensaje) = await Http.GetLinea(url);

                if (!string.IsNullOrEmpty(mensaje))
                {
                    await DisplayAlert("Error", mensaje, "Seguir");
                    BusquedaLineaEntry.Text = "";
                }
                else
                {
                    LineasListView.ItemsSource = lineas;
                    LineasListView.IsVisible = true;
                    BusquedaLineaEntry.IsVisible = true;
                    if (KeyboardExtensions.IsSoftKeyboardShowing(BusquedaLineaEntry))
                    {
                        await KeyboardExtensions.HideKeyboardAsync(BusquedaLineaEntry, default);
                    }
                    else
                    {
                        await KeyboardExtensions.ShowKeyboardAsync(BusquedaLineaEntry, default);
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Error al obtener las líneas: {ex.Message}", "OK");
            }
        }
        else
        {
            await DisplayAlert("Error", "Debe ingresar un nombre de línea.", "OK");
        }
    }


    private void LineaListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {

        try
        {
            if (e.SelectedItem is Linea selectedLineas)
            {

                if (ProductoActual.oLinea == null)
                {
                    ProductoActual.oLinea = new Linea();
                }
                ProductoActual.oLinea.IdLinea = selectedLineas.IdLinea;

                BusquedaLineaEntry.Text = selectedLineas.Descripcion;

                LineasListView.IsVisible = false;
                BusquedaLineaEntry.IsVisible = true;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al seleccionar la linea: {ex.Message}");
            DisplayAlert("Error", $"Ocurrió un error: {ex.Message}", "OK");
        }
    }

    private async void AlmacenesListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {

        try
        {
            if (e.SelectedItem is Almacen selectedAlmacenes)
            {

                if (ProductoActual.oAlmacen == null)
                {
                    ProductoActual.oAlmacen = new Almacen();
                }
                ProductoActual.oAlmacen.AlmacenId = selectedAlmacenes.AlmacenId;

                BusquedaAlmacenEntry.Text = selectedAlmacenes.Ubicacion;

                AlmacenesListView.IsVisible = false;
                BusquedaAlmacenEntry.IsVisible = true;
            }
        }
        catch (Exception ex)
        {
            DisplayAlert("Error", $"Ocurrió un error: {ex.Message}", "OK");
        }
    }
    private async void BtnAlmacenBusqueda_Clicked(object sender, EventArgs e)
    {

        var ubicacion = BusquedaAlmacenEntry.Text;

        if (!string.IsNullOrEmpty(ubicacion))
        {
            var url = $"http://" + IP.SERVIDOR + ":5210/Almacen/SelecionaUbicacion?ubicacion=" + ubicacion;
            var marca = await Http.GetAlmacen(url);
            AlmacenesListView.ItemsSource = marca;

            AlmacenesListView.IsVisible = true;
            BusquedaAlmacenEntry.IsVisible = true;
            if (KeyboardExtensions.IsSoftKeyboardShowing(BusquedaAlmacenEntry))
            {
                await KeyboardExtensions.HideKeyboardAsync(BusquedaAlmacenEntry, default);
            }
            else
            {
                await KeyboardExtensions.ShowKeyboardAsync(BusquedaAlmacenEntry, default);
            }
        }
        else
        {
            await DisplayAlert("Error", "No hay texto en Almacen.", "OK");
        }
    }

    private async void RacksListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {

        if (e.SelectedItem is AlmacenRack selectedAlmacenes)
        {

            if (ProductoActual.oRack == null)
            {
                ProductoActual.oRack = new AlmacenRack();
            }
            ProductoActual.oRack.RackId = selectedAlmacenes.RackId;

            BusquedaRackEntry.Text = selectedAlmacenes.Ubicacion;

            RacksListView.IsVisible = false;
            BusquedaRackEntry.IsVisible = true;
        }
        var RackId = ProductoActual.oRack.RackId;
        if (RackId != 0)
        {
            var url = $"http://" + IP.SERVIDOR + ":5210/AlmacenRackSeccion/ObtenerRackSeccion?rackId=" + RackId;
            var marca = await Http.GetAlmacenRackSeccion(url);
            SeccionListView.ItemsSource = marca;

            SeccionListView.IsVisible = true;
            BusquedaAlmacenEntry.IsVisible = true;


        }
        else
        {
            await DisplayAlert("Error", "No hay texto en Almacen.", "OK");
        }
    }

    private async void BtnRackBusqueda_Clicked(object sender, EventArgs e)
    {
        var ubicacion = BusquedaRackEntry.Text.Trim();
        if (!string.IsNullOrEmpty(ubicacion))
        {
            var almacenId = ProductoActual.oAlmacen.AlmacenId;
            var url = $"http://{IP.SERVIDOR}:5210/AlmacenRack/ObtenerAlmacenRack?almacenId={almacenId}&ubicacion={ubicacion}";

            var (racks, mensaje) = await Http.GetAlmacenRack(url);

            if (!string.IsNullOrEmpty(mensaje))
            {
                await DisplayAlert("Error", mensaje, "Continuar");
                BusquedaRackEntry.Text = "";
            }
            else
            {
                RacksListView.ItemsSource = racks;
                RacksListView.IsVisible = true;
                BusquedaRackEntry.IsVisible = true;

                if (KeyboardExtensions.IsSoftKeyboardShowing(BusquedaRackEntry))
                {
                    await KeyboardExtensions.HideKeyboardAsync(BusquedaRackEntry, default);
                }
                else
                {
                    await KeyboardExtensions.ShowKeyboardAsync(BusquedaRackEntry, default);
                }
            }
        }
        else
        {
            await DisplayAlert("Error", "No hay texto en Rack.", "OK");
        }
    }


    private void SeccionListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {

        if (e.SelectedItem is AlmacenRackSeccion selectedAlmacenes)
        {

            if (ProductoActual.oSeccion == null)
            {
                ProductoActual.oSeccion = new AlmacenRackSeccion();
            }
            ProductoActual.oSeccion.SeccionId = selectedAlmacenes.SeccionId;

            BusquedaSeccionEntry.Text = selectedAlmacenes.Ubicacion;

            SeccionListView.IsVisible = false;
            BusquedaSeccionEntry.IsVisible = true;
        }
    }

    private void BtnLineaLimpiar_Clicked(object sender, EventArgs e)
    {
        BusquedaLineaEntry.Text = "";
    }

    private void BtnLineaMarcar_Clicked(object sender, EventArgs e)
    {
        BusquedaMarcaEntry.Text = "";

    }

    private void BtnLineaRack_Clicked(object sender, EventArgs e)
    {
        BusquedaRackEntry.Text = "";

    }

    private void BtnDescripcionLIMPIAR_Clicked(object sender, EventArgs e)
    {
        DescripcionEntry.Text = "";
    }

    private void BtnPasarNoparte_Clicked(object sender, EventArgs e)
    {
        NoParteEntry.Text = CodigoBarras.Text;
    }
}