//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DrNajeeb.EF
{
    using System;
    using System.Collections.Generic;
    
    public partial class AspNetUser
    {
        public AspNetUser()
        {
            this.AspNetUserClaims = new HashSet<AspNetUserClaim>();
            this.AspNetUserLogins = new HashSet<AspNetUserLogin>();
            this.AspNetUsers1 = new HashSet<AspNetUser>();
            this.AspNetUsers11 = new HashSet<AspNetUser>();
            this.AspNetUsers12 = new HashSet<AspNetUser>();
            this.AspNetUsers13 = new HashSet<AspNetUser>();
            this.AspNetUsers14 = new HashSet<AspNetUser>();
            this.AspNetUsers15 = new HashSet<AspNetUser>();
            this.AspNetUsers16 = new HashSet<AspNetUser>();
            this.AspNetUsers17 = new HashSet<AspNetUser>();
            this.AspNetUsers18 = new HashSet<AspNetUser>();
            this.AspNetUsers19 = new HashSet<AspNetUser>();
            this.AspNetUsers110 = new HashSet<AspNetUser>();
            this.AspNetUsers111 = new HashSet<AspNetUser>();
            this.AspNetUsers112 = new HashSet<AspNetUser>();
            this.AspNetUsers113 = new HashSet<AspNetUser>();
            this.AspNetUsers114 = new HashSet<AspNetUser>();
            this.AspNetUsers115 = new HashSet<AspNetUser>();
            this.Categories = new HashSet<Category>();
            this.Categories1 = new HashSet<Category>();
            this.Categories2 = new HashSet<Category>();
            this.Categories3 = new HashSet<Category>();
            this.Categories4 = new HashSet<Category>();
            this.Categories5 = new HashSet<Category>();
            this.Categories6 = new HashSet<Category>();
            this.Categories7 = new HashSet<Category>();
            this.Categories8 = new HashSet<Category>();
            this.Categories9 = new HashSet<Category>();
            this.Categories10 = new HashSet<Category>();
            this.Categories11 = new HashSet<Category>();
            this.Categories12 = new HashSet<Category>();
            this.Categories13 = new HashSet<Category>();
            this.Categories14 = new HashSet<Category>();
            this.Categories15 = new HashSet<Category>();
            this.CategoryVideos = new HashSet<CategoryVideo>();
            this.CategoryVideos1 = new HashSet<CategoryVideo>();
            this.CategoryVideos2 = new HashSet<CategoryVideo>();
            this.CategoryVideos3 = new HashSet<CategoryVideo>();
            this.CategoryVideos4 = new HashSet<CategoryVideo>();
            this.CategoryVideos5 = new HashSet<CategoryVideo>();
            this.CategoryVideos6 = new HashSet<CategoryVideo>();
            this.CategoryVideos7 = new HashSet<CategoryVideo>();
            this.IpAddressFilters = new HashSet<IpAddressFilter>();
            this.IpAddressFilters1 = new HashSet<IpAddressFilter>();
            this.IpAddressFilters2 = new HashSet<IpAddressFilter>();
            this.IpAddressFilters3 = new HashSet<IpAddressFilter>();
            this.IpAddressFilters4 = new HashSet<IpAddressFilter>();
            this.IpAddressFilters5 = new HashSet<IpAddressFilter>();
            this.IpAddressFilters6 = new HashSet<IpAddressFilter>();
            this.IpAddressFilters7 = new HashSet<IpAddressFilter>();
            this.IpAddressFilters8 = new HashSet<IpAddressFilter>();
            this.IpAddressFilters9 = new HashSet<IpAddressFilter>();
            this.IpAddressFilters10 = new HashSet<IpAddressFilter>();
            this.IpAddressFilters11 = new HashSet<IpAddressFilter>();
            this.IpAddressFilters12 = new HashSet<IpAddressFilter>();
            this.IpAddressFilters13 = new HashSet<IpAddressFilter>();
            this.IpAddressFilters14 = new HashSet<IpAddressFilter>();
            this.IpAddressFilters15 = new HashSet<IpAddressFilter>();
            this.LoggedInTrackings = new HashSet<LoggedInTracking>();
            this.LoggedInTrackings1 = new HashSet<LoggedInTracking>();
            this.LoggedInTrackings2 = new HashSet<LoggedInTracking>();
            this.LoggedInTrackings3 = new HashSet<LoggedInTracking>();
            this.MessageToAlls = new HashSet<MessageToAll>();
            this.NewFeatures = new HashSet<NewFeature>();
            this.Subscriptions = new HashSet<Subscription>();
            this.Subscriptions1 = new HashSet<Subscription>();
            this.Subscriptions2 = new HashSet<Subscription>();
            this.Subscriptions3 = new HashSet<Subscription>();
            this.Subscriptions4 = new HashSet<Subscription>();
            this.Subscriptions5 = new HashSet<Subscription>();
            this.Subscriptions6 = new HashSet<Subscription>();
            this.Subscriptions7 = new HashSet<Subscription>();
            this.Subscriptions8 = new HashSet<Subscription>();
            this.Subscriptions9 = new HashSet<Subscription>();
            this.Subscriptions10 = new HashSet<Subscription>();
            this.Subscriptions11 = new HashSet<Subscription>();
            this.Subscriptions12 = new HashSet<Subscription>();
            this.Subscriptions13 = new HashSet<Subscription>();
            this.Subscriptions14 = new HashSet<Subscription>();
            this.Subscriptions15 = new HashSet<Subscription>();
            this.SupportMessages = new HashSet<SupportMessage>();
            this.SupportMessages1 = new HashSet<SupportMessage>();
            this.SupportMessages2 = new HashSet<SupportMessage>();
            this.SupportMessages3 = new HashSet<SupportMessage>();
            this.SupportMessages4 = new HashSet<SupportMessage>();
            this.SupportMessages5 = new HashSet<SupportMessage>();
            this.SupportMessages6 = new HashSet<SupportMessage>();
            this.SupportMessages7 = new HashSet<SupportMessage>();
            this.UserFavoriteVideos = new HashSet<UserFavoriteVideo>();
            this.UserFavoriteVideos1 = new HashSet<UserFavoriteVideo>();
            this.UserFavoriteVideos2 = new HashSet<UserFavoriteVideo>();
            this.UserFavoriteVideos3 = new HashSet<UserFavoriteVideo>();
            this.UserVideoHistories = new HashSet<UserVideoHistory>();
            this.UserVideoHistories1 = new HashSet<UserVideoHistory>();
            this.UserVideoHistories2 = new HashSet<UserVideoHistory>();
            this.UserVideoHistories3 = new HashSet<UserVideoHistory>();
            this.VideoDownloadhistories = new HashSet<VideoDownloadhistory>();
            this.Videos = new HashSet<Video>();
            this.Videos1 = new HashSet<Video>();
            this.Videos2 = new HashSet<Video>();
            this.Videos3 = new HashSet<Video>();
            this.Videos4 = new HashSet<Video>();
            this.Videos5 = new HashSet<Video>();
            this.Videos6 = new HashSet<Video>();
            this.Videos7 = new HashSet<Video>();
            this.Videos8 = new HashSet<Video>();
            this.Videos9 = new HashSet<Video>();
            this.Videos10 = new HashSet<Video>();
            this.Videos11 = new HashSet<Video>();
            this.Videos12 = new HashSet<Video>();
            this.Videos13 = new HashSet<Video>();
            this.Videos14 = new HashSet<Video>();
            this.Videos15 = new HashSet<Video>();
            this.AspNetRoles = new HashSet<AspNetRole>();
        }
    
        public string Id { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public Nullable<System.DateTime> LockoutEndDateUtc { get; set; }
        public bool LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public bool IsPasswordReset { get; set; }
        public bool IsAllowMobileVideos { get; set; }
        public bool IsParentalControl { get; set; }
        public string ParentalControlPin { get; set; }
        public bool IsFilterByIP { get; set; }
        public int NoOfConcurentViews { get; set; }
        public Nullable<int> CountryId { get; set; }
        public Nullable<int> SubscriptionId { get; set; }
        public bool IsActiveUSer { get; set; }
        public int CurrentViews { get; set; }
        public bool Active { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<System.DateTime> SubscriptionDate { get; set; }
        public Nullable<System.DateTime> ExpirationDate { get; set; }
        public string ProfilePicture { get; set; }
        public Nullable<bool> IsFreeUser { get; set; }
        public Nullable<bool> IsInstitutionalAccount { get; set; }
    
        public virtual ICollection<AspNetUserClaim> AspNetUserClaims { get; set; }
        public virtual ICollection<AspNetUserLogin> AspNetUserLogins { get; set; }
        public virtual Country Country { get; set; }
        public virtual Country Country1 { get; set; }
        public virtual Country Country2 { get; set; }
        public virtual Country Country3 { get; set; }
        public virtual Country Country4 { get; set; }
        public virtual Country Country5 { get; set; }
        public virtual Country Country6 { get; set; }
        public virtual Country Country7 { get; set; }
        public virtual ICollection<AspNetUser> AspNetUsers1 { get; set; }
        public virtual AspNetUser AspNetUser1 { get; set; }
        public virtual ICollection<AspNetUser> AspNetUsers11 { get; set; }
        public virtual AspNetUser AspNetUser2 { get; set; }
        public virtual ICollection<AspNetUser> AspNetUsers12 { get; set; }
        public virtual AspNetUser AspNetUser3 { get; set; }
        public virtual ICollection<AspNetUser> AspNetUsers13 { get; set; }
        public virtual AspNetUser AspNetUser4 { get; set; }
        public virtual ICollection<AspNetUser> AspNetUsers14 { get; set; }
        public virtual AspNetUser AspNetUser5 { get; set; }
        public virtual ICollection<AspNetUser> AspNetUsers15 { get; set; }
        public virtual AspNetUser AspNetUser6 { get; set; }
        public virtual ICollection<AspNetUser> AspNetUsers16 { get; set; }
        public virtual AspNetUser AspNetUser7 { get; set; }
        public virtual ICollection<AspNetUser> AspNetUsers17 { get; set; }
        public virtual AspNetUser AspNetUser8 { get; set; }
        public virtual Subscription Subscription { get; set; }
        public virtual Subscription Subscription1 { get; set; }
        public virtual Subscription Subscription2 { get; set; }
        public virtual Subscription Subscription3 { get; set; }
        public virtual Subscription Subscription4 { get; set; }
        public virtual Subscription Subscription5 { get; set; }
        public virtual Subscription Subscription6 { get; set; }
        public virtual Subscription Subscription7 { get; set; }
        public virtual ICollection<AspNetUser> AspNetUsers18 { get; set; }
        public virtual AspNetUser AspNetUser9 { get; set; }
        public virtual ICollection<AspNetUser> AspNetUsers19 { get; set; }
        public virtual AspNetUser AspNetUser10 { get; set; }
        public virtual ICollection<AspNetUser> AspNetUsers110 { get; set; }
        public virtual AspNetUser AspNetUser11 { get; set; }
        public virtual ICollection<AspNetUser> AspNetUsers111 { get; set; }
        public virtual AspNetUser AspNetUser12 { get; set; }
        public virtual ICollection<AspNetUser> AspNetUsers112 { get; set; }
        public virtual AspNetUser AspNetUser13 { get; set; }
        public virtual ICollection<AspNetUser> AspNetUsers113 { get; set; }
        public virtual AspNetUser AspNetUser14 { get; set; }
        public virtual ICollection<AspNetUser> AspNetUsers114 { get; set; }
        public virtual AspNetUser AspNetUser15 { get; set; }
        public virtual ICollection<AspNetUser> AspNetUsers115 { get; set; }
        public virtual AspNetUser AspNetUser16 { get; set; }
        public virtual ICollection<Category> Categories { get; set; }
        public virtual ICollection<Category> Categories1 { get; set; }
        public virtual ICollection<Category> Categories2 { get; set; }
        public virtual ICollection<Category> Categories3 { get; set; }
        public virtual ICollection<Category> Categories4 { get; set; }
        public virtual ICollection<Category> Categories5 { get; set; }
        public virtual ICollection<Category> Categories6 { get; set; }
        public virtual ICollection<Category> Categories7 { get; set; }
        public virtual ICollection<Category> Categories8 { get; set; }
        public virtual ICollection<Category> Categories9 { get; set; }
        public virtual ICollection<Category> Categories10 { get; set; }
        public virtual ICollection<Category> Categories11 { get; set; }
        public virtual ICollection<Category> Categories12 { get; set; }
        public virtual ICollection<Category> Categories13 { get; set; }
        public virtual ICollection<Category> Categories14 { get; set; }
        public virtual ICollection<Category> Categories15 { get; set; }
        public virtual ICollection<CategoryVideo> CategoryVideos { get; set; }
        public virtual ICollection<CategoryVideo> CategoryVideos1 { get; set; }
        public virtual ICollection<CategoryVideo> CategoryVideos2 { get; set; }
        public virtual ICollection<CategoryVideo> CategoryVideos3 { get; set; }
        public virtual ICollection<CategoryVideo> CategoryVideos4 { get; set; }
        public virtual ICollection<CategoryVideo> CategoryVideos5 { get; set; }
        public virtual ICollection<CategoryVideo> CategoryVideos6 { get; set; }
        public virtual ICollection<CategoryVideo> CategoryVideos7 { get; set; }
        public virtual ICollection<IpAddressFilter> IpAddressFilters { get; set; }
        public virtual ICollection<IpAddressFilter> IpAddressFilters1 { get; set; }
        public virtual ICollection<IpAddressFilter> IpAddressFilters2 { get; set; }
        public virtual ICollection<IpAddressFilter> IpAddressFilters3 { get; set; }
        public virtual ICollection<IpAddressFilter> IpAddressFilters4 { get; set; }
        public virtual ICollection<IpAddressFilter> IpAddressFilters5 { get; set; }
        public virtual ICollection<IpAddressFilter> IpAddressFilters6 { get; set; }
        public virtual ICollection<IpAddressFilter> IpAddressFilters7 { get; set; }
        public virtual ICollection<IpAddressFilter> IpAddressFilters8 { get; set; }
        public virtual ICollection<IpAddressFilter> IpAddressFilters9 { get; set; }
        public virtual ICollection<IpAddressFilter> IpAddressFilters10 { get; set; }
        public virtual ICollection<IpAddressFilter> IpAddressFilters11 { get; set; }
        public virtual ICollection<IpAddressFilter> IpAddressFilters12 { get; set; }
        public virtual ICollection<IpAddressFilter> IpAddressFilters13 { get; set; }
        public virtual ICollection<IpAddressFilter> IpAddressFilters14 { get; set; }
        public virtual ICollection<IpAddressFilter> IpAddressFilters15 { get; set; }
        public virtual ICollection<LoggedInTracking> LoggedInTrackings { get; set; }
        public virtual ICollection<LoggedInTracking> LoggedInTrackings1 { get; set; }
        public virtual ICollection<LoggedInTracking> LoggedInTrackings2 { get; set; }
        public virtual ICollection<LoggedInTracking> LoggedInTrackings3 { get; set; }
        public virtual ICollection<MessageToAll> MessageToAlls { get; set; }
        public virtual ICollection<NewFeature> NewFeatures { get; set; }
        public virtual ICollection<Subscription> Subscriptions { get; set; }
        public virtual ICollection<Subscription> Subscriptions1 { get; set; }
        public virtual ICollection<Subscription> Subscriptions2 { get; set; }
        public virtual ICollection<Subscription> Subscriptions3 { get; set; }
        public virtual ICollection<Subscription> Subscriptions4 { get; set; }
        public virtual ICollection<Subscription> Subscriptions5 { get; set; }
        public virtual ICollection<Subscription> Subscriptions6 { get; set; }
        public virtual ICollection<Subscription> Subscriptions7 { get; set; }
        public virtual ICollection<Subscription> Subscriptions8 { get; set; }
        public virtual ICollection<Subscription> Subscriptions9 { get; set; }
        public virtual ICollection<Subscription> Subscriptions10 { get; set; }
        public virtual ICollection<Subscription> Subscriptions11 { get; set; }
        public virtual ICollection<Subscription> Subscriptions12 { get; set; }
        public virtual ICollection<Subscription> Subscriptions13 { get; set; }
        public virtual ICollection<Subscription> Subscriptions14 { get; set; }
        public virtual ICollection<Subscription> Subscriptions15 { get; set; }
        public virtual ICollection<SupportMessage> SupportMessages { get; set; }
        public virtual ICollection<SupportMessage> SupportMessages1 { get; set; }
        public virtual ICollection<SupportMessage> SupportMessages2 { get; set; }
        public virtual ICollection<SupportMessage> SupportMessages3 { get; set; }
        public virtual ICollection<SupportMessage> SupportMessages4 { get; set; }
        public virtual ICollection<SupportMessage> SupportMessages5 { get; set; }
        public virtual ICollection<SupportMessage> SupportMessages6 { get; set; }
        public virtual ICollection<SupportMessage> SupportMessages7 { get; set; }
        public virtual ICollection<UserFavoriteVideo> UserFavoriteVideos { get; set; }
        public virtual ICollection<UserFavoriteVideo> UserFavoriteVideos1 { get; set; }
        public virtual ICollection<UserFavoriteVideo> UserFavoriteVideos2 { get; set; }
        public virtual ICollection<UserFavoriteVideo> UserFavoriteVideos3 { get; set; }
        public virtual ICollection<UserVideoHistory> UserVideoHistories { get; set; }
        public virtual ICollection<UserVideoHistory> UserVideoHistories1 { get; set; }
        public virtual ICollection<UserVideoHistory> UserVideoHistories2 { get; set; }
        public virtual ICollection<UserVideoHistory> UserVideoHistories3 { get; set; }
        public virtual ICollection<VideoDownloadhistory> VideoDownloadhistories { get; set; }
        public virtual ICollection<Video> Videos { get; set; }
        public virtual ICollection<Video> Videos1 { get; set; }
        public virtual ICollection<Video> Videos2 { get; set; }
        public virtual ICollection<Video> Videos3 { get; set; }
        public virtual ICollection<Video> Videos4 { get; set; }
        public virtual ICollection<Video> Videos5 { get; set; }
        public virtual ICollection<Video> Videos6 { get; set; }
        public virtual ICollection<Video> Videos7 { get; set; }
        public virtual ICollection<Video> Videos8 { get; set; }
        public virtual ICollection<Video> Videos9 { get; set; }
        public virtual ICollection<Video> Videos10 { get; set; }
        public virtual ICollection<Video> Videos11 { get; set; }
        public virtual ICollection<Video> Videos12 { get; set; }
        public virtual ICollection<Video> Videos13 { get; set; }
        public virtual ICollection<Video> Videos14 { get; set; }
        public virtual ICollection<Video> Videos15 { get; set; }
        public virtual ICollection<AspNetRole> AspNetRoles { get; set; }
    }
}
