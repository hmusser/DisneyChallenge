using DisneyChallenge.Helpers;

namespace DisneyChallenge.Servicios
{
    public interface IMailService
    {
        Task SendEmailAsync(MailRequest mailRequest);
    }
}
