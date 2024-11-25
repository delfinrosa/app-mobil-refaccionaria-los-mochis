using System;
using System.Collections.Generic;
using CapaEntidad;
using Refaccionaria_los_Mochis.Generic;

namespace Refaccionaria_los_Mochis.Pages.Mantenimiento
{
    public partial class MantenimientoPage : ContentPage
    {
        public MantenimientoPage(Dictionary<string, object> navigationParameter)
        {
            InitializeComponent();
            if (navigationParameter.TryGetValue("idEquipo", out var idEquipo))
            {
                lblEquipo.Text = $"Equipo: {idEquipo}";
            }
        }

        private async void OnGuardarClicked(object sender, EventArgs e)
        {
            var equipo = lblEquipo.Text?.Replace("Equipo: ", "");
            var limpieza = chkLimpieza.IsChecked;
            var fallo = chkFallo.IsChecked;
            var observaciones = txtObservaciones.Text;

            string userId = await SecureStorage.GetAsync("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                await DisplayAlert("Error", "No se pudo obtener el ID de usuario.", "OK");
                App.Current.MainPage = new login();
                return;
            }

            var obj = new MantenimientoEquipo
            {
                Limpieza = limpieza,
                Fallo = fallo,
                Observacion = observaciones,
                Usuario = new Usuario { IdUsuario = int.Parse(userId) },
                Equipo = new Equipo { IDEquipo = equipo }
            };

            var url = $"http://{IP.SERVIDOR}:5210/Mantenimiento/GuardarMantenimiento";

            var (exito, mensaje) = await Http.PostRegistrarMantenimiento(obj, url);

            if (exito)
            {
                await DisplayAlert("Éxito", "Mantenimiento registrado correctamente.", "OK");
                App.Current.MainPage = new PrincipalPage();

            }
            else
            {
                await DisplayAlert("Error", mensaje, "OK");
            }
        }



        private void btnRegresar_Clicked(object sender, EventArgs e)
        {
            App.Current.MainPage = new PrincipalPage();
        }
    }
}
