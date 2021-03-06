﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DrNajeeb.Web.Models
{
    public class VideoModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Duration { get; set; }
        public int ReleaseYear { get; set; }
        public Nullable<DateTime> DateLive { get; set; }
        public string BackgroundColor { get; set; }
        public bool IsEnabled { get; set; }
        public string StandardVideoId { get; set; }
        public string FastVideoId { get; set; }

    }
}