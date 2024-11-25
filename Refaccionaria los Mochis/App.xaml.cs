using Plugin.LocalNotification;
using Refaccionaria_los_Mochis.Models;
using Refaccionaria_los_Mochis.Pages;

namespace Refaccionaria_los_Mochis
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            // Registrar el servicio de notificaciones correctamente
            DependencyService.Register<Models.INotificationService, NotificationService>();
            var notificationService = DependencyService.Get<Models.INotificationService>();

            MainPage = new login();

            // Iniciar el servicio de notificaciones (opcional)
        }
    }
}
