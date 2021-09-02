using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantApi.Models;
using RestaurantApi.Services;
using RestaurantApii.DataBase;
using RestaurantApii.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantApi.Controllers
{
    [Route("api/restaurant")]
    [ApiController]
    public class RestaurantController : ControllerBase
    {
        private readonly IRestaurantServies _restaurantServies;
        public RestaurantController(IRestaurantServies restaurantSerives)
        {
            _restaurantServies = restaurantSerives;
        }
        [HttpPost]
        public ActionResult CreateRestaurant([FromBody] CreateRestaurantDto dto)
        {
            var id =_restaurantServies.Create(dto);
            return Created($"/api/restaurant/{id}", null);
        }

        [HttpGet]
        public ActionResult<IEnumerable<RestaurantDto>> GetAll()
        {
            var restaurantsDtos = _restaurantServies.GetAll();
            return Ok(restaurantsDtos);
        }

        [HttpGet("{id}")]
        public ActionResult<RestaurantDto> Get([FromRoute] int id)
        {
            var restaurant = _restaurantServies.GetById(id);
            return Ok(restaurant);
        }

        [HttpDelete("{id}")]
        public ActionResult<RestaurantDto> Delete([FromRoute] int id)
        {
            _restaurantServies.Delete(id);

            return NoContent();
        }

        [HttpPut("{id}")]
        public ActionResult Modify([FromBody] ModifyRestaurantDto dto, [FromRoute] int id)
        {
            _restaurantServies.Modify(id, dto);
            return Ok();
        }
    }
}
