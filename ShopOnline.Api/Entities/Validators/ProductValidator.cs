using FluentValidation;

namespace ShopOnline.Api.Entities.Validators
{
    public class ProductValidator: AbstractValidator<Product>
    {
        public ProductValidator()
        {
            RuleFor(p => p.Name).NotEmpty();
            RuleFor(p => p.Description).NotEmpty();
            RuleFor(p => p.ImageURL).NotEmpty();
            RuleFor(p => p.Price).GreaterThan(0);
            RuleFor(p => p.Qty).GreaterThan(0);
            RuleFor(p => p.CategoryId).NotNull();
        }

    }
}
