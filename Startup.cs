using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using RestaurantApi.Authorization;
using RestaurantApi.Models;
using RestaurantApi.Models.Validators;
using RestaurantApi.Services;
using RestaurantAPI.Middleware;
using RestaurantApii.DataBase;
using RestaurantApii.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            var authenticationSettings = new AuthenticationSettings();

            Configuration.GetSection("Authentication").Bind(authenticationSettings);

            services.AddSingleton(authenticationSettings);
            services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = "Bearer";
                option.DefaultScheme = "Bearer";
                option.DefaultChallengeScheme = "Bearer";
            }).AddJwtBearer(cfg =>
            {
                cfg.RequireHttpsMetadata = false;
                cfg.SaveToken = true;
                cfg.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = authenticationSettings.JwtIssuer,
                    ValidAudience = authenticationSettings.JwtIssuer,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationSettings.JwtKey)),
                };
            });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("HasNationality", builder => builder.RequireClaim("Nationality", "German", "Polish"));
                options.AddPolicy("Atleast20", builder => builder.AddRequirements(new MinimumAgeRequirement(20)));
                options.AddPolicy("CreatedAtleast2Restaurants",
                    builder => builder.AddRequirements(new MinimumTwoRestaurantRequirement(2)));
            });
            services.AddScoped<IAuthorizationHandler, MinimumAgeRequirementHandler>();
            services.AddScoped<IAuthorizationHandler, ResourceOperationRequirementHendler>();
            services.AddScoped<IAuthorizationHandler, MinimumTwoRestaurantRequirementHendler>();

            services.AddScoped<IUserContextServives, UserContextServives>();
            services.AddHttpContextAccessor();


            services.AddControllers().AddFluentValidation();
            //£aczenie z baz¹ danych 
            services.AddDbContext<RestaurantDbContext>();
            //Akywowanie seedera 
            services.AddScoped<RestaurantSeeder>();
            //Dodanie Mappera
            services.AddAutoMapper(this.GetType().Assembly);

            //Dodanie hashera hase³
            services.AddScoped<IPasswordHasher<User>,PasswordHasher<User>>();
            //Dodanie Valifdatora
            services.AddScoped<IValidator<UserRegisterDto>, RegisterUserDtoValidator>();
            services.AddScoped<IValidator<RestaurantQuery>, RestaurantQueryValidator>();

            //Us³uga dodwania, pobierania restauraciji
            services.AddScoped<IRestaurantServies, RestaurantServies>();
            //Us³uga dodowania, pobieranie dañ
            services.AddScoped<IDishService, DishService>();
            //Us³ugo dodwania i logowania urzytkowników
            services.AddScoped<IAccountServices, AccountServices>();
            
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
            app.UseAuthentication();

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
