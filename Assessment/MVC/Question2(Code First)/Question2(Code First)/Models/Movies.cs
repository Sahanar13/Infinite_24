using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Question2_Code_First_.Models
{
    public class Movie
    {
        [Key]
        public int Mid { get; set; }

        public string Moviename { get; set; }

        [Display(Name = "Release Date")]
        public DateTime DateofRelease { get; set; }
    }
}