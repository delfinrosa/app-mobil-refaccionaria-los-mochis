using Refaccionaria_los_Mochis.Generic;
namespace Refaccionaria_los_Mochis.Pages;

public partial class login : ContentPage
{
    public login()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        string userId = await SecureStorage.GetAsync("UserId");

        if (!string.IsNullOrEmpty(userId))
        {
            int idUsuario = int.Parse(userId);
            if (idUsuario > 0)
            {
                App.Current.MainPage = new PrincipalPage();
            }
        }
    }

    private async void btnlogin_Clicked(object sender, EventArgs e)
    {
        Recursos objRecursos = new Recursos();
        if (!string.IsNullOrEmpty(passwordEntry.Text) && !string.IsNullOrEmpty(usernameEntry.Text))
        {
            string con = objRecursos.ConvertirSha256(passwordEntry.Text);

            var url = $"http://{IP.SERVIDOR}:5210/Usuario/VerificarUsuario?correo={usernameEntry.Text}&clave={con}";
            var usuario = await Http.GetUsuario(url);

            if (usuario?.Nombre != null)
            {
                await SecureStorage.SetAsync("UserId", usuario.IdUsuario.ToString());
                await SecureStorage.SetAsync("UserNombre", usuario.Nombre);

                App.Current.MainPage = new PrincipalPage();
            }
            else
            {
                await DisplayAlert("Error", "Login fallido. Por favor, inténtelo de nuevo.", "OK");
            }
        }
        else
        {
            await DisplayAlert("Error", "Por favor, complete todos los campos.", "OK");
        }
    }
}