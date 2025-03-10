﻿using System.ComponentModel.DataAnnotations;

namespace FreeCourse.Web.Models.Catalogs
{
    public class CourseUpdateInput
    {
        public string Id { get; set; }
        [Display(Name = "Kurs ismi")]        
        public string Name { get; set; }
        [Display(Name = "Kurs açıklama")]      
        public string Description { get; set; }

        [Display(Name = "Kurs fiyat")]      
        public decimal Price { get; set; }
        public string UserId { get; set; }

        public string Picture { get; set; }
        public FeatureViewModel Feature { get; set; }
        [Display(Name = "Kurs kategori")]        
        public string CategoryId { get; set; }
        [Display(Name = "Kurs resimi")]
        public IFormFile PhotoFormfile { get; set; }
    }
}
