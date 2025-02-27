using IOITCore.Entities.Bases;

namespace HoneyChef.Api.Entities
{
    public class Recipe : AbstractEntity<Guid>
    {
        public int UserId { get; set; }
        public Guid IdFood { get; set; }
        public string? Title { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public string? FeatureImgUrl { get; set; } = string.Empty;
        public DateTime? Duration { get; set; } = new DateTime(0);
        public int CountryId { get; set; }
        public float? AvgRating { get; set; } = 0;

    }
} 
