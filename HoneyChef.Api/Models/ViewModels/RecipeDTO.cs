using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using HoneyChef.Api.Entities;

namespace HoneyChef.Api.Models.ViewModels
{
    public class RecipeDTO : Recipe
    {
        public string? DurationDTO { get; set; }
        public IFormFile? FeaturedImage { get; set; }

        public TimeSpan? GetParsedDuration()
        {
            if (string.IsNullOrWhiteSpace(DurationDTO))
                return null; // Không bắt lỗi nếu Duration không có giá trị

            if (TimeSpan.TryParse(DurationDTO, out var parsedDuration))
                return parsedDuration;
                
            return null; // Hoặc có thể log lỗi nếu cần
        }
    }
}