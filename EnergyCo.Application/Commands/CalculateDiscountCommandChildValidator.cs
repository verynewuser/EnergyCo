using FluentValidation;

namespace EnergyCo.Application.Commands;

public class CalculateDiscountCommandChildValidator : AbstractValidator<CalculateDiscountCommandChild>
{
    public CalculateDiscountCommandChildValidator()
    {
        RuleFor(v => v.ProductId)
            .NotEmpty();
    }
}