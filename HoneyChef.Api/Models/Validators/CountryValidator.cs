using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using HoneyChef.Api.Entities;
using HoneyChef.Api.Models.ViewModels;

namespace HoneyChef.Api.Models.Validators
{
    public class CountryValidator:AbstractValidator<CountryDTO>
    {
        public CountryValidator(){
            RuleFor(x => x.CountryName)
                .NotEmpty().WithMessage("Tên quốc gia không được để trống")
                .MaximumLength(100).WithMessage("Tên quốc gia không vượt quá 100 kí tự");
        }
    }
}