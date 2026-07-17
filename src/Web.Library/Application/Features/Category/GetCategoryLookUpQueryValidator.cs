using FluentValidation;

namespace Web.Library.Application.Features.Category
{
    public class GetCategoryLookUpQueryValidator: AbstractValidator<GetCategoryLookUpQuery>
    {
        public GetCategoryLookUpQueryValidator() 
        {
            // Category id
            RuleFor(x => x.CategoryId)
                .Must(id => id == null || id != Guid.Empty).WithMessage("CategoryId must be a valid GUID or null.");
        }
    }
}
