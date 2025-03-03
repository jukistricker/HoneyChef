using IOITCore.Entities.Bases;

namespace HoneyChef.Api.Entities
{
    public class Recipe: AbstractEntity<long>
    {
        public long? UserId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? FeatureImgUrl { get; set; }
        public TimeSpan? Duration { get; set; }
        public double? AvgRating { get; set; } = 0;
        public long? CountryId { get; set; }
    }
}

//PK
//Id int NOT NULL 
//FK1 
//Userld int NOT NULL 
//Title nvarchar NOT NULL 
//Description nvarchar NOT NULL 
//FeatureImgUrl varchar NOT NULL 
//Duration time(0) NOT NULL 
//FK2 
//Countryld int NOT NULL 
//AvgRating float NOT NULL
