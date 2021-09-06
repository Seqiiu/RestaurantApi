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
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RestaurantApi.Services
{
    public interface IRestaurantServies
    {
        RestaurantDto GetById(int id);
        PageResult<RestaurantDto> GetAll(RestaurantQuery query);
        int Create(CreateRestaurantDto dto);
        void Delete(int id);
        void Modify(int id, ModifyRestaurantDto dto);

    }
    public class RestaurantServies : IRestaurantServies
    {
        private readonly RestaurantDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<RestaurantServies> _logger;
        private readonly IAuthorizationService _authorizationService;
        private readonly IUserContextServives _userContextServives;
        public RestaurantServies(RestaurantDbContext dbContext, IMapper mapper, ILogger<RestaurantServies> logger, IAuthorizationService authorizationService, IUserContextServives userContextServives)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
            _authorizationService = authorizationService;
            _userContextServives = userContextServives;
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
        public PageResult<RestaurantDto> GetAll(RestaurantQuery query)
        {
            var baseQuary = _dbContext
                 .Restaurants
                 .Include(r => r.Address)
                 .Include(r => r.Dishes)
                 .Where(r => query.SearchPhrase == null || (r.Name.ToLower().Contains(query.SearchPhrase.ToLower())
                             || r.Description.ToLower().Contains(query.SearchPhrase.ToLower())));

            if (!string.IsNullOrEmpty(query.SortBy)) 
            {
                var columnsSelectors = new Dictionary<string, Expression<Func<Restaurant, object>>>
                {
                    {nameof(Restaurant.Name), r => r.Name},
                    {nameof(Restaurant.Description), r => r.Description},
                    {nameof(Restaurant.Category), r => r.Category},
                };

                var selectedColum = columnsSelectors[query.SortBy];


                baseQuary = query.SortDirection== SortDirection.ASC
                    ? baseQuary.OrderBy(selectedColum)
                    : baseQuary.OrderByDescending(selectedColum);
            }

            var restaurants = baseQuary
                .Skip(query.PageSize *(query.PageNumber -1))
                .Take(query.PageSize)
                .ToList();

            var count = baseQuary.Count();

            var restaurantsDtos = _mapper.Map<List<RestaurantDto>>(restaurants);
            
            var result = new PageResult<RestaurantDto>(restaurantsDtos, count, query.PageSize,query.PageNumber);

            return result;
        }

       //Twozenie nowej Restauracji 
        public int Create(CreateRestaurantDto dto)
        {
            var restaurant = _mapper.Map<Restaurant>(dto);
            restaurant.CreatedById = _userContextServives.GetUserId;
            _dbContext.Restaurants.Add(restaurant);
            _dbContext.SaveChanges();

            return restaurant.Id;
        }

        //Usuwanie Restauracji z Adressem 
        public void Delete(int id)
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
            var wynik = _authorizationService.AuthorizeAsync(_userContextServives.User, restaurant, new ResourceOperationRequirement(ResourceOperation.Delete)).Result;
            if (!wynik.Succeeded)
            {
                throw new ForbidException();
            }
            _dbContext.Restaurants.Remove(restaurant);
            _dbContext.Adresses.Remove(address);
            _dbContext.SaveChanges();
        }
    
        public void Modify(int id, ModifyRestaurantDto dto)
        {
            var restaurant = _dbContext
                .Restaurants
                .FirstOrDefault(x => x.Id == id);

            if (restaurant is null)
            {
                throw new NotFoundException("Restaurant not found");
            }

            var wynik = _authorizationService.AuthorizeAsync(_userContextServives.User, restaurant, new ResourceOperationRequirement(ResourceOperation.Update)).Result;
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
