using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CWheelsApi.Models
{
    public class Vehicle  
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public string Model { get; set; }
        public string Engine { get; set; }
        public string Color { get; set; }
        public string Company { get; set; }
         public DateTime DatePosted { get; set; }
        public bool IsHotAndNew { get; set; }
        public bool IsFeatured { get; set; }
        public string Location { get; set; }    
        public string Condition { get; set; }   

        public int UserId { get; set; }
        public int CategoryId { get; set; }
        public ICollection<Image> Images { get; set; }  
    }
}
