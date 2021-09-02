using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RestaurantApi.Services;
using RestaurantAPI.Middleware;
using RestaurantApii.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();
            //£aczenie z baz¹ danych 
            services.AddDbContext<RestaurantDbContext>();
            //Akywowanie seedera 
            services.AddScoped<RestaurantSeeder>();
            //Dodanie Mappera
            services.AddAutoMapper(this.GetType().Assembly);
            //Us³uga dodwania, pobierania restauraciji
            services.AddScoped<IRestaurantServies, RestaurantServies>();
            //Us³uga dodowania, pobieranie dañ
            services.AddScoped<IDishService, DishService>();
            //Logger do pliku txt 
            services.AddScoped<ErrorHandlingMiddleware>();
            //Loger 2 czas wykonania wiêkszy ni¿ 4 sekundy
            services.AddScoped<RequestTimeMiddleware>();
            //Swagger 
            services.AddSwaggerGen();


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, RestaurantSeeder restaurantSeeder)
        {
            //Dodawanie Wartoœci Podstawoych przy starcie programu 
            restaurantSeeder.Seed();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMiddleware<ErrorHandlingMiddleware>();
            app.UseMiddleware<RequestTimeMiddleware>();

            app.UseHttpsRedirection();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Restaurant API");
            });

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
