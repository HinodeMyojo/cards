﻿using CardsServer.BLL.Abstractions;
using CardsServer.BLL.Infrastructure.Auth;
using CardsServer.BLL.Services.User;
using CardsServer.DAL;
using CardsServer.DAL.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace CardsServer.API
{
    public static class ProgramExtensions
    {
        public static IServiceCollection RegisterService(this IServiceCollection services)
        {
            services.AddDbContext<ApplicationContext>();

            services.AddTransient<IJwtGenerator, JwtGenerator>();


            services.AddTransient<ILoginService, LoginService>();
            services.AddTransient<IUserService, UserService>();

            services.AddTransient<ILoginRepository, LoginRepository>();
            services.AddTransient<IUserRepository, UserRepository>();

            return services;
        }

        public static IServiceCollection AuthService(this IServiceCollection services, IConfiguration configuration) 
        {
            JwtOptions jwtOptions = configuration.GetSection(nameof(JwtOptions)).Get<JwtOptions>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(opt =>
                {
                    opt.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtOptions.Issuer,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey)),
                        ValidateLifetime = true,
                    };
                });

            //services.AddAuthorization(opt =>
            //{
            //    opt.AddPolicy("Admin", policy =>
            //    {
            //        policy.AddRequirements.Add()
            //    })
            //})

            return services;
        }
    }
}
