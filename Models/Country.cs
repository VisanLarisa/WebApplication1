using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class Country
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Continent { get; set; }
        public string Region { get; set; }
        public float SurfaceArea { get; set; }
        public int IndepYear { get; set; }
        public int Population { get; set; }
        public float LifeExpectency { get; set; }
        public float GNP { get; set; }
        public float GNPOId { get; set; }
        public string LocalName { get; set; }
        public string GovernmentForm { get; set; }
        public string HeadOfState { get; set; }
        public int Capital { get; set; }
        public string Code2 { get; set; }
        public string ContinentCode { get; set; }


       // public List<City> Cities { get; set; }
    }
}
