using System.Security.Claims;
using System.Text;
using API.Services;
using Domain;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Persistence;

namespace API.Extensions
{
    public static class IdentityServiceExtensions
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.CustomSchemaIds(type => type.FullName); // Sử dụng tên đầy đủ (namespace + class)
            });
            services.AddCors(opt =>
            {
                opt.AddPolicy("Policy", policy =>
                    {
                        policy.AllowAnyHeader().AllowAnyMethod().AllowCredentials()
                        .WithOrigins("http://localhost:3000", "http://localhost:3001", 
                        "https://admin.argonaut.asia", "https://argonaut.asia",
                        "https://trongnp-registry.site","https://admin.trongnp-registry.site",
                        "https://pickleball.name.vn","https://admin.pickleball.name.vn");
                    }
                );
            });

            // services.AddCors(opt =>
            // {
            //     opt.AddPolicy("Policy", policy => policy.AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin());
            // });
            services.AddIdentityCore<AppUser>(opt =>
            {
                opt.Password.RequireNonAlphanumeric = true;
                opt.Password.RequiredUniqueChars = 1;
                opt.Password.RequiredLength = 8;
                opt.Password.RequireLowercase = true;
                opt.Password.RequireDigit = true;
                opt.Password.RequireUppercase = true;
                opt.User.RequireUniqueEmail = true;
            })
            .AddRoles<IdentityRole>()
            .AddRoleManager<RoleManager<IdentityRole>>() // Đăng ký RoleManager
            .AddEntityFrameworkStores<DataContext>();


            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["TokenKey"]));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(opt =>
            {
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    RoleClaimType = ClaimTypes.Role
                };
            });
            services.AddScoped<TokenService>();
            services.AddHttpContextAccessor();
            return services;
        }
    }
}