﻿using System.ComponentModel.DataAnnotations;

namespace Eventify.APIs.DTOs.Categories
{
    public class CreateCategoryDto
    {
        [Required]
        public int EventId { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public int Seats { get; set; }
    }

}
