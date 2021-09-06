using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RestaurantApi.Services
{
    public interface IUserContextServives
    {
        ClaimsPrincipal User { get; }
        int? GetUserId { get; }
    }

    public class UserContextServives : IUserContextServives
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserContextServives(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }


        public ClaimsPrincipal User => _httpContextAccessor.HttpContext?.User;
        public int? GetUserId =>User is null ? null :(int?)int.Parse(User.FindFirst(c => c.Type ==ClaimTypes.NameIdentifier).Value);
    }
}
