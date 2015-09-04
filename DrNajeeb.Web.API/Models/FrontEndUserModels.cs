using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DrNajeeb.Web.API.Models
{
    public class UserCategoryModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class UserVideoModel
    {
        public UserVideoModel()
        {
            Categories = new List<UserCategoryModel>();
        }


        public int Id { get; set; }
        public string Name { get; set; }
        public double Duration { get; set; }
        public int ReleaseYear { get; set; }
        public DateTime DateLive { get; set; }
        public string BackgroundColor { get; set; }
        public string VzaarVideoId { get; set; }
        public string Description { get; set; }
        public string ThumbnailURL { get; set; }
        public bool IsFavoriteVideo { get; set; }
        public List<UserCategoryModel> Categories { get; set; }
    }

    public class UserFavoriteVideoModel
    {
        public string UserId { get; set; }
        public int VideoId { get; set; }
    }

    public class SupportModel
    {
        public string Subject { get; set; }
        public string Message { get; set; }
        public string ToUserId { get; set; }
    }

    public class UserMessagesModel
    {
        public int Id { get; set; }
        public DateTime MessageDateTime { get; set; }
        public string MessageText { get; set; }
        public bool IsRead { get; set; }
        public bool IsFromAdmin { get; set; }
        public bool IsFromUser { get; set; }
        public string ReplierName { get; set; }
    }
}