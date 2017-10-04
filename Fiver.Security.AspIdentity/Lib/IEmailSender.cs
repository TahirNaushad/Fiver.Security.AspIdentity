using System.Threading.Tasks;

namespace Fiver.Security.AspIdentity.Lib
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
