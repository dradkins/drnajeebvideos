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
    
    public partial class Category
    {
        public Category()
        {
            this.CategoriesInSubscriptions = new HashSet<CategoriesInSubscription>();
            this.CategoriesInSubscriptions1 = new HashSet<CategoriesInSubscription>();
            this.CategoriesInSubscriptions2 = new HashSet<CategoriesInSubscription>();
            this.CategoriesInSubscriptions3 = new HashSet<CategoriesInSubscription>();
            this.CategoriesInSubscriptions4 = new HashSet<CategoriesInSubscription>();
            this.CategoriesInSubscriptions5 = new HashSet<CategoriesInSubscription>();
            this.CategoriesInSubscriptions6 = new HashSet<CategoriesInSubscription>();
            this.CategoriesInSubscriptions7 = new HashSet<CategoriesInSubscription>();
            this.CategoryVideos = new HashSet<CategoryVideo>();
            this.CategoryVideos1 = new HashSet<CategoryVideo>();
            this.CategoryVideos2 = new HashSet<CategoryVideo>();
            this.CategoryVideos3 = new HashSet<CategoryVideo>();
            this.CategoryVideos4 = new HashSet<CategoryVideo>();
            this.CategoryVideos5 = new HashSet<CategoryVideo>();
            this.CategoryVideos6 = new HashSet<CategoryVideo>();
            this.CategoryVideos7 = new HashSet<CategoryVideo>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsShowOnFrontPage { get; set; }
        public string ImageURL { get; set; }
        public string SEOName { get; set; }
        public string CategoryURL { get; set; }
        public Nullable<int> DisplayOrder { get; set; }
        public bool Active { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
    
        public virtual AspNetUser AspNetUser { get; set; }
        public virtual AspNetUser AspNetUser1 { get; set; }
        public virtual AspNetUser AspNetUser2 { get; set; }
        public virtual AspNetUser AspNetUser3 { get; set; }
        public virtual AspNetUser AspNetUser4 { get; set; }
        public virtual AspNetUser AspNetUser5 { get; set; }
        public virtual AspNetUser AspNetUser6 { get; set; }
        public virtual AspNetUser AspNetUser7 { get; set; }
        public virtual AspNetUser AspNetUser8 { get; set; }
        public virtual AspNetUser AspNetUser9 { get; set; }
        public virtual AspNetUser AspNetUser10 { get; set; }
        public virtual AspNetUser AspNetUser11 { get; set; }
        public virtual AspNetUser AspNetUser12 { get; set; }
        public virtual AspNetUser AspNetUser13 { get; set; }
        public virtual AspNetUser AspNetUser14 { get; set; }
        public virtual AspNetUser AspNetUser15 { get; set; }
        public virtual ICollection<CategoriesInSubscription> CategoriesInSubscriptions { get; set; }
        public virtual ICollection<CategoriesInSubscription> CategoriesInSubscriptions1 { get; set; }
        public virtual ICollection<CategoriesInSubscription> CategoriesInSubscriptions2 { get; set; }
        public virtual ICollection<CategoriesInSubscription> CategoriesInSubscriptions3 { get; set; }
        public virtual ICollection<CategoriesInSubscription> CategoriesInSubscriptions4 { get; set; }
        public virtual ICollection<CategoriesInSubscription> CategoriesInSubscriptions5 { get; set; }
        public virtual ICollection<CategoriesInSubscription> CategoriesInSubscriptions6 { get; set; }
        public virtual ICollection<CategoriesInSubscription> CategoriesInSubscriptions7 { get; set; }
        public virtual ICollection<CategoryVideo> CategoryVideos { get; set; }
        public virtual ICollection<CategoryVideo> CategoryVideos1 { get; set; }
        public virtual ICollection<CategoryVideo> CategoryVideos2 { get; set; }
        public virtual ICollection<CategoryVideo> CategoryVideos3 { get; set; }
        public virtual ICollection<CategoryVideo> CategoryVideos4 { get; set; }
        public virtual ICollection<CategoryVideo> CategoryVideos5 { get; set; }
        public virtual ICollection<CategoryVideo> CategoryVideos6 { get; set; }
        public virtual ICollection<CategoryVideo> CategoryVideos7 { get; set; }
    }
}
