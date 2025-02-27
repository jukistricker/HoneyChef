using System.ComponentModel.DataAnnotations;
using IOITCore.Entities.Bases;

namespace HoneyChef.Api.Entities
{
    public class Country : AbstractEntity<int>
    {
        public int? IdParent { get; set; }
        public string CountryName { get; set; } = string.Empty;
    }
}
