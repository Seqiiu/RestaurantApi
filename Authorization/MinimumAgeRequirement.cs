using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantApi.Authorization
{
    public class MinimumAgeRequirement : IAuthorizationRequirement
    {
        public int MiniumAge { get; }

        public MinimumAgeRequirement(int miniumAge)
        {
            MiniumAge = miniumAge;
        }
    }
}
