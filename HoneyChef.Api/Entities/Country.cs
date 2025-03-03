using IOITCore.Entities.Bases;

namespace HoneyChef.Api.Entities
{
    public class Country: AbstractEntity<long>
    {
        public string? CountryName { get; set; }
    }
}
