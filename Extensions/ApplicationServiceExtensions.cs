using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.Data;
using DatingApp.Interfaces;
using DatingApp.Services;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config){
        services.AddDbContext<DataContext>(opt=>{
        opt.UseSqlite(config.GetConnectionString("DefaultConnection"));
        });

        services.AddCors();

        services.AddScoped<ITokenService,TokenServices>();
        return services;
        }
    }
}