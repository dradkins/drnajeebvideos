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
    
    public partial class Video
    {
        public Video()
        {
            this.CategoryVideos = new HashSet<CategoryVideo>();
            this.CategoryVideos1 = new HashSet<CategoryVideo>();
            this.UserFavoriteVideos = new HashSet<UserFavoriteVideo>();
            this.UserVideoHistories = new HashSet<UserVideoHistory>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Duration { get; set; }
        public int ReleaseYear { get; set; }
        public System.DateTime DateLive { get; set; }
        public string BackgroundColor { get; set; }
        public string ThumbnailURL { get; set; }
        public string PreviewURL { get; set; }
        public string SpecialFeatureURL { get; set; }
        public string BackgroundImageURL { get; set; }
        public string VideoPlayerImageURL { get; set; }
        public bool IsEnabled { get; set; }
        public bool IsFeatured { get; set; }
        public bool IsMatureContent { get; set; }
        public bool IsShowInPromo { get; set; }
        public string StandardVideoId { get; set; }
        public string FastVideoId { get; set; }
        public bool Active { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
    
        public virtual AspNetUser AspNetUser { get; set; }
        public virtual AspNetUser AspNetUser1 { get; set; }
        public virtual AspNetUser AspNetUser2 { get; set; }
        public virtual AspNetUser AspNetUser3 { get; set; }
        public virtual ICollection<CategoryVideo> CategoryVideos { get; set; }
        public virtual ICollection<CategoryVideo> CategoryVideos1 { get; set; }
        public virtual ICollection<UserFavoriteVideo> UserFavoriteVideos { get; set; }
        public virtual ICollection<UserVideoHistory> UserVideoHistories { get; set; }
    }
}
