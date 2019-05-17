using Cine.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cine.Models
{
    public class Movie
    {
        public int Id { get; set; }
        public int Year { get; set; }
        public string ImageUrl { get; set; }
        public string Title { get; set; }
        public string Sinopsis { get; set; }
        public double Rating { get; set; }
        public Genders Gender { get; set; }
    }
}
