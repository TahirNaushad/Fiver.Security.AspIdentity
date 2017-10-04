using System.Threading.Tasks;

namespace Fiver.Security.AspIdentity.Services.Email
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
