using API.Services;
using Application.Core;
using Application.DTOs;
using Application.Handler.Categories;
using Application.Interfaces;
using Domain;
using FluentValidation;
using FluentValidation.AspNetCore;
using Infrastructure.Photos;
using Infrastructure.Repository;
using Infrastructure.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Persistence.Repository;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddDbContext<DataContext>(
                opt =>
                {
                    opt.UseMySql(configuration.GetConnectionString("DefaultConnection"), new MySqlServerVersion(new Version(10, 11, 9)));
                }
            );

            services.AddCors(opt =>
            {
                opt.AddPolicy("Policy", policy => policy.AllowAnyMethod().AllowAnyHeader().WithOrigins("http://localhost:3000"));
            });

            services.AddIdentity<AppUser, IdentityRole>(options =>
               {
                   options.SignIn.RequireConfirmedAccount = true;
                   options.Tokens.EmailConfirmationTokenProvider = "Default"; // Cung cấp token mặc định
                                                                              // Thêm các tùy chọn khác nếu cần
               })
               .AddEntityFrameworkStores<DataContext>()
               .AddDefaultTokenProviders(); // Đảm bảo đã thêm DefaultTokenProviders

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(List.Handler).Assembly));
            services.AddAutoMapper(typeof(MappingProfile).Assembly);

            services.AddFluentValidationAutoValidation();
            services.AddValidatorsFromAssemblyContaining<Create>();
            services.AddHttpContextAccessor();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IUserAccessor, UserAccessor>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IPhotoAccessor, PhotoAccessor>();
            services.AddOptions();
            services.Configure<CloudinarySettings>(configuration.GetSection("Cloudinary"));
            services.Configure<MailSettings>(configuration.GetSection("MailSettings"));
            services.AddScoped(typeof(IGenericRepository<>), (typeof(GenericRepository<>)));
            return services;
        }
    }
}