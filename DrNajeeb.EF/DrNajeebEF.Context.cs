﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class Entities : DbContext
    {
        public Entities()
            : base("name=Entities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<AspNetRole> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }
        public virtual DbSet<CategoriesInSubscription> CategoriesInSubscriptions { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<CategoryVideo> CategoryVideos { get; set; }
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<IpAddressFilter> IpAddressFilters { get; set; }
        public virtual DbSet<NewFeature> NewFeatures { get; set; }
        public virtual DbSet<Subscription> Subscriptions { get; set; }
        public virtual DbSet<UserFavoriteVideo> UserFavoriteVideos { get; set; }
        public virtual DbSet<UserVideoHistory> UserVideoHistories { get; set; }
        public virtual DbSet<Video> Videos { get; set; }
        public virtual DbSet<LoggedInTracking> LoggedInTrackings { get; set; }
        public virtual DbSet<SupportMessage> SupportMessages { get; set; }
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
    }
}
