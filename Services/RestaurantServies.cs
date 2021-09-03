using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RestaurantApi.Authorization;
using RestaurantApi.Models;
using RestaurantAPI.Exceptions;
using RestaurantApii.DataBase;
using RestaurantApii.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RestaurantApi.Services
{
    public interface IRestaurantServies
    {
        RestaurantDto GetById(int id);
        IEnumerable<RestaurantDto> GetAll();
        int Create(CreateRestaurantDto dto, int userId);
        void Delete(int id, ClaimsPrincipal user);
        void Modify(int id, ModifyRestaurantDto dto, ClaimsPrincipal user);

    }
    public class RestaurantServies : IRestaurantServies
    {
        private readonly RestaurantDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<RestaurantServies> _logger;
        private readonly IAuthorizationService _authorizationService;
        public RestaurantServies(RestaurantDbContext dbContext, IMapper mapper, ILogger<RestaurantServies> logger, IAuthorizationService authorizationService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
            _authorizationService = authorizationService;
        }
        //Pobiebanie Restauracji Po wskazanym ID
        public RestaurantDto GetById(int id )
        {
            var restaurant = _dbContext
                .Restaurants
                .Include(r => r.Address)
                .Include(r => r.Dishes)
                .FirstOrDefault(x => x.Id == id);
            if (restaurant is null)
            {
                throw new NotFoundException("Restaurant not found");
            }
            var result = _mapper.Map<RestaurantDto>(restaurant);
            return result;
        }
       //Pobieranie Wszystkich Restauracji 
        public IEnumerable<RestaurantDto> GetAll()
        {
            var restaurants = _dbContext
                .Restaurants
                .Include(r => r.Address)
                .Include(r => r.Dishes)
                .ToList();

            var restaurantsDtos = _mapper.Map<List<RestaurantDto>>(restaurants);

            return restaurantsDtos;
        }

       //Twozenie nowej Restauracji 
        public int Create(CreateRestaurantDto dto, int userId)
        {
            var restaurant = _mapper.Map<Restaurant>(dto);
            restaurant.CreatedById = userId;
            _dbContext.Restaurants.Add(restaurant);
            _dbContext.SaveChanges();

            return restaurant.Id;
        }

        //Usuwanie Restauracji z Adressem 
        public void Delete(int id, ClaimsPrincipal user)
        {
            _logger.LogTrace($"Restaurant with id: {id} DELETE action invoked");

            var restaurant = _dbContext
                .Restaurants
                .FirstOrDefault(x => x.Id == id);
            var address = _dbContext
                .Adresses.
                FirstOrDefault(x => x.Id == id);
            if (restaurant is null && address is null)
            {
                throw new NotFoundException("Restaurant not found");
            }
            var wynik = _authorizationService.AuthorizeAsync(user, restaurant, new ResourceOperationRequirement(ResourceOperation.Delete)).Result;
            if (!wynik.Succeeded)
            {
                throw new ForbidException();
            }
            _dbContext.Restaurants.Remove(restaurant);
            _dbContext.Adresses.Remove(address);
            _dbContext.SaveChanges();
        }
    
        public void Modify(int id, ModifyRestaurantDto dto, ClaimsPrincipal user)
        {
            var restaurant = _dbContext
                .Restaurants
                .FirstOrDefault(x => x.Id == id);

            if (restaurant is null)
            {
                throw new NotFoundException("Restaurant not found");
            }

            var wynik = _authorizationService.AuthorizeAsync(user, restaurant, new ResourceOperationRequirement(ResourceOperation.Update)).Result;
            if (!wynik.Succeeded)
            {
                throw new ForbidException();
            }

            restaurant.Name = dto.Name;
            restaurant.Description = dto.Description;
            restaurant.HasDelivery = dto.HasDelivery;

            _dbContext.SaveChanges();
        }
    }
}
