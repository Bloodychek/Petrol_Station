using Microsoft.AspNetCore.Identity;

namespace Petrol_Station.Models
{
    public class User : IdentityUser
    {
        public int Year { get; set; }
    }
}
