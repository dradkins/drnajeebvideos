using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DrNajeeb.Web.API.Models
{
    public class SupportUsersModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public int TotalUnreadMessages { get; set; }
        public string FullName { get; set; }
    }
}