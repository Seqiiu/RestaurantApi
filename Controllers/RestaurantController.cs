using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantApi.Models;
using RestaurantApi.Services;
using RestaurantApii.DataBase;
using RestaurantApii.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RestaurantApi.Controllers
{
    [Route("api/restaurant")]
    [ApiController]
    [Authorize]
    public class RestaurantController : ControllerBase
    {
        private readonly IRestaurantServies _restaurantServies;
        public RestaurantController(IRestaurantServies restaurantSerives)
        {
            _restaurantServies = restaurantSerives;
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Menager")]
        public ActionResult CreateRestaurant([FromBody] CreateRestaurantDto dto)
        {
            var userId = int.Parse(User.FindFirst(c => c.Type ==ClaimTypes.NameIdentifier).Value);
            var id = _restaurantServies.Create(dto, userId);
            return Created($"/api/restaurant/{id}", null);
        }

        [HttpGet]
        [Authorize(Policy = "Atleast20")]
        public ActionResult<IEnumerable<RestaurantDto>> GetAll()
        {
            var restaurantsDtos = _restaurantServies.GetAll();
            return Ok(restaurantsDtos);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public ActionResult<RestaurantDto> Get([FromRoute] int id)
        {
            var restaurant = _restaurantServies.GetById(id);
            return Ok(restaurant);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Menager")]
        public ActionResult<RestaurantDto> Delete([FromRoute] int id)
        {
            _restaurantServies.Delete(id, User);

            return NoContent();
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Menager")]
        public ActionResult Modify([FromBody] ModifyRestaurantDto dto, [FromRoute] int id)
        {
            _restaurantServies.Modify(id, dto, User);
            return Ok();
        }
    }
}
