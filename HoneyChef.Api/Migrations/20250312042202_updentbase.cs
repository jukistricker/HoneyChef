using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HoneyChef.Api.Migrations
{
    /// <inheritdoc />
    public partial class updentbase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserCreated",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UserUpdated",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UserCreated",
                table: "UserRoles");

            migrationBuilder.DropColumn(
                name: "UserUpdated",
                table: "UserRoles");

            migrationBuilder.DropColumn(
                name: "UserCreated",
                table: "UserMappings");

            migrationBuilder.DropColumn(
                name: "UserUpdated",
                table: "UserMappings");

            migrationBuilder.DropColumn(
                name: "UserCreated",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "UserUpdated",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "UserCreated",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "UserUpdated",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "UserCreated",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "UserUpdated",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "UserCreated",
                table: "RecipeCategories");

            migrationBuilder.DropColumn(
                name: "UserUpdated",
                table: "RecipeCategories");

            migrationBuilder.DropColumn(
                name: "UserCreated",
                table: "LogActions");

            migrationBuilder.DropColumn(
                name: "UserUpdated",
                table: "LogActions");

            migrationBuilder.DropColumn(
                name: "UserCreated",
                table: "Ingredients");

            migrationBuilder.DropColumn(
                name: "UserUpdated",
                table: "Ingredients");

            migrationBuilder.DropColumn(
                name: "UserCreated",
                table: "Functions");

            migrationBuilder.DropColumn(
                name: "UserUpdated",
                table: "Functions");

            migrationBuilder.DropColumn(
                name: "UserCreated",
                table: "FunctionRoles");

            migrationBuilder.DropColumn(
                name: "UserUpdated",
                table: "FunctionRoles");

            migrationBuilder.DropColumn(
                name: "UserCreated",
                table: "Foods");

            migrationBuilder.DropColumn(
                name: "UserUpdated",
                table: "Foods");

            migrationBuilder.DropColumn(
                name: "UserCreated",
                table: "Directions");

            migrationBuilder.DropColumn(
                name: "UserUpdated",
                table: "Directions");

            migrationBuilder.DropColumn(
                name: "UserCreated",
                table: "DirectionDetails");

            migrationBuilder.DropColumn(
                name: "UserUpdated",
                table: "DirectionDetails");

            migrationBuilder.DropColumn(
                name: "UserCreated",
                table: "Countries");

            migrationBuilder.DropColumn(
                name: "UserUpdated",
                table: "Countries");

            migrationBuilder.DropColumn(
                name: "UserCreated",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "UserUpdated",
                table: "Categories");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserCreated",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserUpdated",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserCreated",
                table: "UserRoles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserUpdated",
                table: "UserRoles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserCreated",
                table: "UserMappings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserUpdated",
                table: "UserMappings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserCreated",
                table: "Roles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserUpdated",
                table: "Roles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserCreated",
                table: "Reviews",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserUpdated",
                table: "Reviews",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserCreated",
                table: "Recipes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserUpdated",
                table: "Recipes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserCreated",
                table: "RecipeCategories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserUpdated",
                table: "RecipeCategories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserCreated",
                table: "LogActions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserUpdated",
                table: "LogActions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserCreated",
                table: "Ingredients",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserUpdated",
                table: "Ingredients",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserCreated",
                table: "Functions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserUpdated",
                table: "Functions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserCreated",
                table: "FunctionRoles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserUpdated",
                table: "FunctionRoles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserCreated",
                table: "Foods",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserUpdated",
                table: "Foods",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserCreated",
                table: "Directions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserUpdated",
                table: "Directions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserCreated",
                table: "DirectionDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserUpdated",
                table: "DirectionDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserCreated",
                table: "Countries",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserUpdated",
                table: "Countries",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserCreated",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserUpdated",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
