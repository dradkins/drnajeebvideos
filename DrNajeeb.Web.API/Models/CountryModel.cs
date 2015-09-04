using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DrNajeeb.Web.API.Models
{
    public class CountryModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string FlagImage { get; set; }
        public string CountryCode { get; set; }
    }
}