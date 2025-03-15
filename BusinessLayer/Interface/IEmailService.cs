namespace BusinessLayer.Interface
{
    public interface IEmailService
    {
        Task SendResetEmail(string email, string token);
    }
}
