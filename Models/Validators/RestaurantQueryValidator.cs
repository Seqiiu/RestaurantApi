using FluentValidation;
using RestaurantApii.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantApi.Models.Validators
{
    public class RestaurantQueryValidator : AbstractValidator<RestaurantQuery>
    {
        private int[] allowedPageSizes = new[] { 5, 10, 15 };
        private string[] allowedSortByColumnName = { nameof(Restaurant.Category), nameof(Restaurant.Name), nameof(Restaurant.Description) };
        public RestaurantQueryValidator()
        {
            RuleFor(r => r.PageNumber).GreaterThanOrEqualTo(1);
            RuleFor(r => r.PageSize).Custom((value, context) =>
             {
                 if (!allowedPageSizes.Contains(value))
                 {
                     context.AddFailure("PageSize", $"Page must in [{string.Join(", ", allowedPageSizes)}]");
                 }
             });


            RuleFor(r => r.SortBy)
                .Must(value => string.IsNullOrEmpty(value) || allowedSortByColumnName.Contains(value))
                .WithMessage($"Sort by is optcional, or must be in [{string.Join(", ", allowedSortByColumnName)}]");
        }
    }
}
