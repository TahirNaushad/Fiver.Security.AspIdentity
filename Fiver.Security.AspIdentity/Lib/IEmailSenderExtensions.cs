using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Fiver.Security.AspIdentity.Lib
{
    public static class IEmailSenderExtensions
    {
        public static Task SendConfirmationEmailAsync(
            this IEmailSender emailSender, string email, string link)
        {
            return emailSender.SendEmailAsync(email, "Confirm your email",
                $"Click to confirm email: <a href='{HtmlEncoder.Default.Encode(link)}'>link</a>");
        }
    }
}
