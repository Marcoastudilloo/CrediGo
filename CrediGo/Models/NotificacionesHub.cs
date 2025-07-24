using Microsoft.AspNetCore.SignalR;

namespace CrediGo.Models
{
    public class NotificacionesHub : Hub
    {
        public async Task EnviarNotificacion(string mensaje)
        {
            await Clients.All.SendAsync("RecibirNotificacion", mensaje);
        }
    }
    
}
