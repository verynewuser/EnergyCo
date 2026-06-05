using FluentValidation;

namespace EnergyCo.Application.Commands;

public class CalculateDiscountCommandValidator : AbstractValidator<CalculateDiscountCommand>
{
    public CalculateDiscountCommandValidator()
    {
        RuleFor(v => v.CustomerId)
            .NotEmpty();

        RuleFor(v => v.Basket.Count)
            .GreaterThan(0);

        RuleForEach(v => v.Basket)
            .SetValidator(new CalculateDiscountCommandChildValidator());
    }
}