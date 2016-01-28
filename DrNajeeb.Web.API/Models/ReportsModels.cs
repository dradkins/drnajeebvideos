using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DrNajeeb.Web.API.Models
{
    public class MostWatchedVideoReportModel
    {
        public string VideoName { get; set; }
        public int Duration { get; set; }
        public int TotalViews { get; set; }
    }

    public class VideoDownloadHistoryModel
    {
        public string VideoName { get; set; }
        public int Duration { get; set; }
        public int TotalDownloads { get; set; }
    }

    public class UserVideoDownloadHistoryModel
    {
        public string VideoName { get; set; }
        public DateTime DateTimeDownload { get; set; }
    }

    public class MostActiveUsersViewModel
    {
        public int TotalVideosWatched { get; set; }
        public int TotalVideosDownloaded  { get; set; }
        public int TotalFavoritesVideos { get; set; }
        public string UserName  { get; set; }
    }
}