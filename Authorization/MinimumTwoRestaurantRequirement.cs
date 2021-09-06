using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantApi.Authorization
{
    public class MinimumTwoRestaurantRequirement : IAuthorizationRequirement
    {
        public int QuantityRestaurant { get; }
        public MinimumTwoRestaurantRequirement(int quantityRestaurant)
        {
            QuantityRestaurant = quantityRestaurant;
        }
    }
}
