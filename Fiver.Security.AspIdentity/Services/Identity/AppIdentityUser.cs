using Microsoft.AspNetCore.Identity;

namespace Fiver.Security.AspIdentity.Services.Identity
{
    public class AppIdentityUser : IdentityUser
    {
        public int Age { get; set; }
    }
}
