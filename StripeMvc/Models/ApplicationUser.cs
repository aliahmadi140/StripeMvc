using Microsoft.AspNetCore.Identity;

namespace StripeMvc.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string StripeCustomerId { get; set; }
    }
}
