using HoneyChef.Api.Entities;
using IOITCore.Entities.Bases;

namespace HoneyChef.Api.Entities
{
    public class Ingredient:AbstractEntity<long>
    {
        public long? RecipeId { get; set; }
        public string? IngTitle { get; set; }
        public Guid? reference_mongo_id { get; set; }
    }
}

//Ingredient
//PK 
//Id int NOT NULL 
//-FK1 
//Recipeld int NOT NULL 
//Title nvarchar NOT NULL 


//IngredientAttribute 
//PK 
//Id int NOT NULL 
//FK1 
//Ingredientid int NOT NULL 
//Name nvarchar NOT NULL 
//Quantity int NOT NULL 
//Unit int (Enum - nullable)

