using API.DTOs;
using API.Services;
using Application.Core;
using Application.DTOs;
using Application.Handler.Categories;
using Application.Handler.Payments;
using Application.Interfaces;
using Domain;
using FluentValidation;
using FluentValidation.AspNetCore;
using Infrastructure.Photos;
using Infrastructure.Repository;
using Infrastructure.Security;
using Infrastructure.SendMessage;
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

            // services.AddCors(opt =>
            // {
            //     opt.AddPolicy("Policy", policy => policy.AllowAnyMethod().AllowAnyHeader().WithOrigins("http://localhost:3000", "http://argonaut.asia", "https://argonaut.asia"));
            // });

            services.AddCors(opt =>
                      {
                          opt.AddPolicy("Policy", policy => policy.AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin());
                      });
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(List.Handler).Assembly));
            services.AddAutoMapper(typeof(MappingProfile).Assembly);

            services.AddFluentValidationAutoValidation();
            services.AddValidatorsFromAssemblyContaining<Create>();
            services.AddHttpContextAccessor();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IUserAccessor, UserAccessor>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IPhotoAccessor, PhotoAccessor>();
            services.AddScoped<ISendSmsService, SendSmsService>();
            services.AddScoped<IVnPayService, VnPayService>(); 
            services.AddOptions();
            services.Configure<CloudinarySettings>(configuration.GetSection("Cloudinary"));
            services.Configure<MailSettings>(configuration.GetSection("MailSettings"));
            services.Configure<VnPaySettings>(configuration.GetSection("VnPaySettings"));
            services.Configure<InfobipAPI>(configuration.GetSection("InfobipAPI"));
            services.AddScoped(typeof(IGenericRepository<>), (typeof(GenericRepository<>)));
            return services;
        }
    }
}
