namespace CrediGo.Models
{
    public interface IEmailService
    {
        Task EnviarCorreoAsync(string destino, string asunto, string mensaje);
    }
}
