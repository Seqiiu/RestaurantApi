using RestaurantApii.DataBase;
using RestaurantApii.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantApi
{
    public class RestaurantSeeder
    {
        private readonly RestaurantDbContext _restaurantDb;
        public RestaurantSeeder( RestaurantDbContext restaurantDb)
        {
            _restaurantDb = restaurantDb;
        }
        public void Seed()
        {

            if (_restaurantDb.Database.CanConnect())
            {
                if (!_restaurantDb.Roles.Any())
                {
                    var Roles = GetRoles();
                    _restaurantDb.Roles.AddRange(Roles);
                    _restaurantDb.SaveChanges();

                }

                if (!_restaurantDb.Restaurants.Any())
                {
                    var restaurants = GetRestaurants();
                    _restaurantDb.Restaurants.AddRange(restaurants);
                    _restaurantDb.SaveChanges();
                }
            }
        }

        private IEnumerable<Restaurant> GetRestaurants()
        {
            var restaurants = new List<Restaurant>()
            {
                new Restaurant()
                {
                    Name = "KFC",
                    Category = "Fast Food",
                    Description =
                        "KFC (short for Kentucky Fried Chicken) is an American fast food restaurant chain headquartered in Louisville, Kentucky, that specializes in fried chicken.",
                    ContactEmail = "contact@kfc.com",
                    HasDelivery = true,
                    Dishes = new List<Dish>()
                    {
                        new Dish()
                        {
                            Name = "Nashville Hot Chicken",
                            Price = 10.30M,
                        },

                        new Dish()
                        {
                            Name = "Chicken Nuggets",
                            Price = 5.30M,
                        },
                    },
                    Address = new Address()
                    {
                        City = "Kraków",
                        Street = "Długa 5",
                        PostalCode = "30-001"
                    }
                },
                new Restaurant()
                {
                    Name = "McDonald Szewska",
                    Category = "Fast Food",
                    Description =
                        "McDonald's Corporation (McDonald's), incorporated on December 21, 1964, operates and franchises McDonald's restaurants.",
                    ContactEmail = "contact@mcdonald.com",
                    HasDelivery = true,
                    Address = new Address()
                    {
                        City = "Kraków",
                        Street = "Szewska 2",
                        PostalCode = "30-001"
                    }
                }
            };

            return restaurants;
        }
        private IEnumerable<Role> GetRoles()
        {
            var role = new List<Role>()
            {
                new Role()
                {
                Name = "User"
                },
                new Role()
                {
                Name = "Menager"
                },
                new Role()
                {
                Name = "Admin"
                },
            };

            return role;
        }
    }
}
