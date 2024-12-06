using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

using System;

using WLS.Authentication.Constants;
using WLS.Authentication.Models;
using WLS.Authentication.Services;

using WLSUser.Services.Authentication;

namespace WLS.Authentication
{
    public static class ConfigureServices
    {
        public static void AddWLSAuthentication(this IServiceCollection services, string jwtValidIssuer, string jwtValidAudience, string jwtSecretKey)
        {
            services.AddSingleton<ICookiesService, CookiesService>();

            JwtOptions jwtOptions = new JwtOptions(jwtValidIssuer, jwtValidAudience, jwtSecretKey);
            services.AddSingleton(jwtOptions);

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtOptions.JwtValidIssuer,

                ValidateAudience = true,
                ValidAudience = jwtOptions.JwtValidAudience,

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = jwtOptions.JwtSigningCredentials.Key,

                RequireExpirationTime = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
            };

            services.AddAuthentication(options =>
           {
               options.DefaultScheme = JwtSchemes.Default;
           })
                .AddJwtBearer(configureOptions =>
            {
                configureOptions.TokenValidationParameters = tokenValidationParameters;
                configureOptions.SaveToken = true;
            })
                .AddScheme<JwtSchemeOptions, JwtAuthenticationHandler>(JwtSchemes.Default, op => { });
        }
    }
}
