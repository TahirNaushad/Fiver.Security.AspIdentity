using Microsoft.AspNetCore.Identity;

namespace Fiver.Security.AspIdentity.Services.Identity
{
    public class User : IdentityUser<int>
    {
        public int Age { get; set; }
    }
}
