using FluentValidation;

namespace Web.Public.Features.Category
{
    public class GetCategoryTreeQueryValidator: AbstractValidator<GetCategoryTreeQuery>
    {
        public GetCategoryTreeQueryValidator() 
        {
            // Category id
            RuleFor(x => x.CategoryId)
                .Must(id => id == null || id != Guid.Empty).WithMessage("CategoryId must be a valid GUID or null.");
        }
    }
}
