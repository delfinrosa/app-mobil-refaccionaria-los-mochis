using CapaEntidad;
using Newtonsoft.Json;
using Refaccionaria_los_Mochis.Generic;


namespace Refaccionaria_los_Mochis.Pages.Registrar;

    public partial class RegistrarMarca : ContentPage
    {
        public RegistrarMarca()
        {
            InitializeComponent();
        }

    private async void BtnRegistrarMarca_Clicked(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(MarcaEntry.Text))
        {
            Marca marca = new Marca();
            marca.Descripcion = MarcaEntry.Text.ToUpperInvariant();

            string userId = await SecureStorage.GetAsync("UserId");
            if (!string.IsNullOrEmpty(userId))
            {
                marca.oUsuario = new Usuario();
                marca.oUsuario.IdUsuario = int.Parse(userId);
            }
            else
            {
                await DisplayAlert("Error", "No se pudo obtener el ID de usuario.", "OK");
                App.Current.MainPage = new login();
                return;
            }

            var obj = new
            {
                obj = marca
            };

            var url = $"http://{IP.SERVIDOR}:5210/Marcas/Registrar";

            // Llamar al método PostDataAsync para enviar los datos y obtener el ID generado y el mensaje
            var result = await Http.PostMarca(obj, url);

            if (result.IdGenerado > 0)
            {
                await DisplayAlert("Éxito", "Marca registrada correctamente.", "OK");
                App.Current.MainPage = new LeerProductoInsert();
            }
            else
            {
                await DisplayAlert("Error", result.Mensaje, "OK");
            }
        }
        else
        {
            await DisplayAlert("Error", "Debe de poner el nombre de la marca que quiere registrar", "Continuar");
        }
    }

    private void BtnRegresar_Clicked(object sender, EventArgs e)
    {
        App.Current.MainPage = new PrincipalPage();

    }
}

