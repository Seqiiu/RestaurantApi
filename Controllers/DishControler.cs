using Microsoft.AspNetCore.Mvc;
using RestaurantApi.Models;
using RestaurantApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantApi.Controllers
{
    [Route("api/restaurant/{restaurantId}/dish")]
    [ApiController]
    public class DishControler : ControllerBase
    {
        public readonly IDishService _dishService;
        public DishControler(IDishService dishService)
        {
            _dishService = dishService;
        }
        [HttpPost]
        public ActionResult Post([FromRoute] int restaurantId,[FromBody] CreateDishDto dto)
        {
            var newDisId = _dishService.Create(restaurantId, dto);
            return Created($"api/restaurant/{restaurantId}/dish/{newDisId}", null);
        }
        [HttpGet("{dishId}")]
        public ActionResult<DishDto>Get([FromRoute] int restaurantId, [FromRoute] int dishId)
        {
            var dish = _dishService.GetById(restaurantId, dishId);
            return Ok(dish);

        }
        [HttpGet]
        public ActionResult<DishDto> Get([FromRoute] int restaurantId)
        {
            var result = _dishService.GetAll(restaurantId);
            return Ok(result);

        }

        [HttpDelete]
        public ActionResult Delete ([FromRoute] int restaurantId)
        {
            _dishService.RemoveAll( restaurantId);
            return NoContent();
        }

    }
}
