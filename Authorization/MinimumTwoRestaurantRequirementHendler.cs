using Microsoft.AspNetCore.Authorization;
using RestaurantApii.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RestaurantApi.Authorization
{
    public class MinimumTwoRestaurantRequirementHendler : AuthorizationHandler<MinimumTwoRestaurantRequirement>
    {
        private readonly RestaurantDbContext _restaurantDbContext;
        public MinimumTwoRestaurantRequirementHendler(RestaurantDbContext restaurantDbContext)
        {
            _restaurantDbContext = restaurantDbContext;
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MinimumTwoRestaurantRequirement requirement)
        {
            var userId = int.Parse(context.User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var createdRestaurantsCount=_restaurantDbContext.Restaurants.Count(r => r.CreatedById == userId);

            if (createdRestaurantsCount >=requirement.QuantityRestaurant)
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
