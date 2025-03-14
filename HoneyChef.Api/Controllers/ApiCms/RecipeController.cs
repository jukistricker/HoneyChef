using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HoneyChef.Api.Entities;
using HoneyChef.Api.Models.ViewModels;
using HoneyChef.Api.Repositories;
using HoneyChef.Api.Repositories.Interfaces;
using HoneyChef.Api.Services;
using HoneyChef.Api.Services.Interfaces;
using IOITCore.Controllers;
using IOITCore.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HoneyChef.Api.Controllers.ApiCms
{
    [Authorize]
    [Route("api/[controller]")]
    public class RecipeController : BaseController
    {
        private readonly IMediaServices _mediaService;
        private readonly IRecipeRepository _entityRepo;
        private readonly IUnitOfWork _unitOfWork;

        public RecipeController(IMediaServices mediaService, IRecipeRepository entityRepo, IUnitOfWork unitOfWork)
        {
            _mediaService = mediaService;
            _entityRepo = entityRepo;
            _unitOfWork = unitOfWork;
        }



        [HttpPost("upload-image")]
        public async Task<IActionResult> UploadImage([FromForm] IFormFile file)
        {
            try
            {
                var imageUrl = await _mediaService.UploadImageAsync(file);
                return Ok(new { url = imageUrl });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPost("create")]
        public async Task<IActionResult> CreateProduct([FromForm] RecipeDTO recipeDTO)
        {
            try
            {
                if (recipeDTO.FeaturedImage != null)
                {
                    recipeDTO.FeatureImgUrl = await _mediaService.UploadImageAsync(recipeDTO.FeaturedImage);
                }

                // Lưu productDto vào database
                var newRecipe = new Recipe
                {
                    UserId = recipeDTO.UserId,
                    Title = recipeDTO.Title,
                    Description = recipeDTO.Description,
                    Duration = recipeDTO.Duration,
                    CountryId = recipeDTO.CountryId,
                    FeatureImgUrl = recipeDTO.FeatureImgUrl
                };

                await _entityRepo.AddAsync(newRecipe);
                await _unitOfWork.CommitChangesAsync();

                return Ok(newRecipe);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

    }
}