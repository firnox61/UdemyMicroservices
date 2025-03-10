﻿using FluentValidation;
using FreeCourse.Web.Models.Discounts;

namespace FreeCourse.Web.Validators
{
    public class DiscountApplyInputValidator : AbstractValidator<DiscountApplyInput>
    {
        public DiscountApplyInputValidator()
        {
            RuleFor(x => x.Code).NotEmpty().WithMessage("Kupon alanı boş olamaz");
        }
    }
}
