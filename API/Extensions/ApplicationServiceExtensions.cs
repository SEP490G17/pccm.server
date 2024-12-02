using System.Threading.Channels;
using API.DTOs;
using API.Services;
using API.SocketSignalR;
using Application.Core;
using Application.DTOs;
using Application.Handler.Categories;
using Application.Interfaces;
using FluentValidation;
using FluentValidation.AspNetCore;
using Infrastructure.Payment;
using Infrastructure.Photos;
using Infrastructure.Repository;
using Infrastructure.Security;
using Infrastructure.SendMessage;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Persistence.Repository;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<DataContext>(
                opt =>
                {
                    opt.UseMySql(configuration.GetConnectionString("DefaultConnection"), new MySqlServerVersion(new Version(10, 11, 9)));
                },
                ServiceLifetime.Transient
            );

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(List.Handler).Assembly));
            services.AddAutoMapper(typeof(MappingProfile).Assembly);
            services.AddSignalR();

            services.AddSingleton<BookingRealTimeService>();
            services.AddSingleton<NotificationService>();

            services.AddSingleton(Channel.CreateUnbounded<(BookingDtoV1 booking, string groupId)>());
            services.AddSingleton(Channel.CreateUnbounded<(BookingDtoV2 booking, string groupId)>());
            services.AddSingleton(Channel.CreateUnbounded<(NotificationDto booking, string groupId)>());


            services.AddHostedService<BookingBackGroundService>();
            services.AddHostedService<NotificationBackgroundService>();


            
            services.AddFluentValidationAutoValidation();
            services.AddValidatorsFromAssemblyContaining<Create>();
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
