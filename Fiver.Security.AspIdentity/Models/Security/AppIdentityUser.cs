using Microsoft.AspNetCore.Identity;

namespace Fiver.Security.AspIdentity.Models.Security
{
    public class AppIdentityUser : IdentityUser
    {
        public int Age { get; set; }
    }
}
