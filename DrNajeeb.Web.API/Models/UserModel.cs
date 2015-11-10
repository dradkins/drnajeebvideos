using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DrNajeeb.Web.API.Models
{
    public class UserModel
    {
        public UserModel()
        {
            Roles = new List<string>();
            RolesModel = new List<RoleModel>();
            Country = new CountryModel();
            Subscription = new SubscriptionModel();
        }

        public string Id { get; set; }
        public string FullName { get; set; }
        public string EmailAddress { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public bool IsPasswordReset { get; set; }
        public int NoOfConcurrentViews { get; set; }
        public bool IsFilterByIP { get; set; }
        public bool IsActiveUser { get; set; }
        public bool IsFreeUser { get; set; }
        public Nullable<int> CountryID { get; set; }
        public Nullable<int> SubscriptionID { get; set; }
        public List<string> Roles{get;set;}
        public List<string> FilteredIPs { get; set; }
        public List<RoleModel> RolesModel { get; set; }
        public CountryModel Country { get; set; }
        public SubscriptionModel Subscription { get; set; }
        public bool IsInstitutionalAccount { get; set; }

    }
}