using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RestaurantApi.Models;
using RestaurantApii.DataBase;
using RestaurantApii.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantApi.Services
{
    public interface IRestaurantServies
    {
        RestaurantDto GetById(int id);
        IEnumerable<RestaurantDto> GetAll();
        int Create(CreateRestaurantDto dto);
        bool Delete(int id);
        bool Modify(int id, ModifyRestaurantDto dto);

    }
    public class RestaurantServies : IRestaurantServies
    {
        private readonly RestaurantDbContext _dbContext;
        private readonly IMapper _mapper;
        public RestaurantServies(RestaurantDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
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
                return null;
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
        public int Create(CreateRestaurantDto dto)
        {
            var restaurant = _mapper.Map<Restaurant>(dto);
            _dbContext.Restaurants.Add(restaurant);
            _dbContext.SaveChanges();

            return restaurant.Id;
        }

        //Usuwanie Restauracji z Adressem 
        public bool Delete(int id)
        {
            var restaurant = _dbContext
                .Restaurants
                .FirstOrDefault(x => x.Id == id);
            var address = _dbContext
                .Adresses.
                FirstOrDefault(x => x.Id == id);
            if (restaurant is null && address is null)
            {
                return false;
            }
            _dbContext.Restaurants.Remove(restaurant);
            _dbContext.Adresses.Remove(address);
            _dbContext.SaveChanges();
            return true;
        }
    
        public bool Modify(int id, ModifyRestaurantDto dto)
        {
            var restaurant = _dbContext
                .Restaurants
                .FirstOrDefault(x => x.Id == id);
            if (restaurant is null)
            {
                return false;
            }
            restaurant.Name = dto.Name;
            restaurant.Description = dto.Description;
            restaurant.HasDelivery = dto.HasDelivery;

            _dbContext.SaveChanges();
            return true;
        }
    }
}
