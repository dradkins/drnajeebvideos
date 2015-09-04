using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DrNajeeb.Web.API.Models
{
    public class VideoSortingModel
    {
        public int CategoryId { get; set; }
        public int VideoId { get; set; }
        public int LocationNo { get; set; }
    }
}