using FluentValidation;
using HoneyChef.Api.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using System;

public class RecipeDtoValidator : AbstractValidator<RecipeDTO>
{
    public RecipeDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Tiêu đề không được để trống.")
            .MaximumLength(255).WithMessage("Tiêu đề không được dài hơn 255 ký tự.");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Mô tả không được dài hơn 1000 ký tự.");

        RuleFor(x => x.FeaturedImage)
            .Must(BeAValidImage).When(x => x.FeaturedImage != null)
            .WithMessage("Ảnh đại diện phải có định dạng hợp lệ (jpg, jpeg, png) và kích thước dưới 6MB.");

        // RuleFor(x => x.Duration)
        //     .GreaterThan(TimeSpan.Zero).When(x => x.Duration.HasValue)
        //     .WithMessage("Thời gian chế biến phải lớn hơn 0.");

         RuleFor(x => x.DurationDTO)
            .NotEmpty().WithMessage("Thời lượng không được để trống.")
            .Must((dto, duration) => dto.GetParsedDuration() != null)
            .WithMessage("Sai định dạng. Sử dụng HH:mm:ss.")
            .Must((dto, duration) => dto.GetParsedDuration()?.TotalSeconds > 0)
            .WithMessage("Thời lượng phải lớn hơn 00:00:00.");

        RuleFor(x => x.AvgRating)
            .InclusiveBetween(0, 5).When(x => x.AvgRating.HasValue)
            .WithMessage("Đánh giá trung bình phải trong khoảng từ 0 đến 5.");

        RuleFor(x => x.CountryId)
            .NotEmpty().WithMessage("Quốc gia không được để trống");
    }

    private bool BeAValidImage(IFormFile file)
    {
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
        var maxFileSize = 6 * 1024 * 1024; // 6MB

        var fileExtension = System.IO.Path.GetExtension(file.FileName).ToLower();
        return allowedExtensions.Contains(fileExtension) && file.Length <= maxFileSize;
    }
}
