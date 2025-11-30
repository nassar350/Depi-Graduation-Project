using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventify.Service.DTOs.Categories
{
    public class CategoryCreationByEventDto
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public int Seats { get; set; }
        [Required]
        public decimal TicketPrice { get; set; }
    }
}
