using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;

namespace DrNajeeb.Web.API.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }

        public int CountryId { get; set; }

        public int SubscriptionId { get; set; }
        public bool IsActiveUser { get; set; }
        public int CurrentViews { get; set; }
        public DateTime CreatedOn { get; set; }
        public Nullable<DateTime> UpdatedOn { get; set; }
        public bool IsPasswordReset { get; set; }
        public bool IsAllowMobileVideos { get; set; }
        public bool IsParentalControl { get; set; }
        public string ParentalControlPin { get; set; }
        public bool IsFilterByIP { get; set; }
        public int NoOfConcurentViews { get; set; }
        public DateTime SubscriptionDate { get; set; }
        public Nullable<DateTime> ExpirationDate { get; set; }
        public bool Active { get; set; }
        public Nullable<bool> IsFreeUser { get; set; }
        public string ProfilePicture { get; set; }
        public Nullable<bool> IsInstitutionalAccount { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}