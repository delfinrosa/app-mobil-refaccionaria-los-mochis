using Plugin.LocalNotification;
using System;
using System.Threading;
using System.Threading.Tasks;
using Plugin.LocalNotification.AndroidOption;
using Microsoft.Maui.ApplicationModel;

namespace Refaccionaria_los_Mochis.Models
{
    public class NotificationService : INotificationService
    {
        private Timer _timer;

        public NotificationService()
        {
            StartTimer();
        }

        private void StartTimer()
        {
            _timer = new Timer(async (state) => await RepeatEvery5Minutes(), null, TimeSpan.Zero, TimeSpan.FromMinutes(5));
        }

        private async Task RepeatEvery5Minutes()
        {
            // Ejecutar la solicitud de permisos en el hilo principal
            await MainThread.InvokeOnMainThreadAsync(static async () =>
            {
                // Crear y configurar la notificación
                var notification = new NotificationRequest
                {
                    NotificationId = 1001,
                    Title = "Recordatorio",
                    Description = "Han pasado 5 minutos.",
                    Android = new AndroidOptions
                    {
                        ChannelId = "reminder_channel"
                    }
                };

                // Mostrar la notificación
                LocalNotificationCenter.Current.Show(notification);
            });
        }


}
        public interface INotificationService
    {
        // La interfaz puede permanecer vacía si no se necesita agregar métodos
    }
    }
