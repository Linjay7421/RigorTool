using FluentValidation;
using System.Text.RegularExpressions;

namespace Web.Library.Application.Features.Product
{
    public class GetPagedSummaryQueryValidator : AbstractValidator<GetPagedSummaryQuery>
    {
        public GetPagedSummaryQueryValidator()
        {
            // Page index
            RuleFor(x => x.Page)
                .GreaterThan(0).WithMessage("Page must be greater than 0.");
            // Page size
            RuleFor(x => x.PageSize)
                .GreaterThan(0).WithMessage("PageSize must be greater than 0.")
                .LessThanOrEqualTo(100).WithMessage("PageSize must be less than or equal to 100.");
            // Category id
            RuleFor(x => x.CategoryId)
                .Must(id => id == null || id != Guid.Empty).WithMessage("CategoryId must be a valid GUID or null.");
            // Search term
            RuleFor(x => x.SearchTerm)
                .MaximumLength(100).WithMessage("SearchTerm must be less than or equal to 100 characters.")
                .Must(BeValidSearchTerm).WithMessage("SearchTerm contains invalid characters.");
        }

        private static bool BeValidSearchTerm(string? searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return true;
            }

            return !ContainsDangerousCharacters(searchTerm);
        }

        private static bool ContainsDangerousCharacters(string value)
        {
            return Regex.IsMatch(value, @"[<>;{}]");
        }
    }
}
