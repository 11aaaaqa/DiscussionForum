using Hangfire;
using Hangfire.PostgreSql;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RegisterMicroservice.Api.Database;
using RegisterMicroservice.Api.MessageBus.Consumers;
using RegisterMicroservice.Api.Models.UserModels;
using RegisterMicroservice.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(x => x.UseNpgsql(
    builder.Configuration["Database:ConnectionString"]));

builder.Services.AddIdentity<User, IdentityRole>(x =>
    {
        x.Password.RequireLowercase = false;
        x.Password.RequireUppercase = false;
        x.Password.RequireDigit = false;
        x.Password.RequireNonAlphanumeric = false;
        x.Password.RequiredLength = 8;

        x.User.RequireUniqueEmail = true;

        x.SignIn.RequireConfirmedEmail = true;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
builder.Services.AddHangfire(x =>
{
    x.UsePostgreSqlStorage(c => c.UseNpgsqlConnection(builder.Configuration["Database:HangfireConnectionString"]));
});
builder.Services.AddHangfireServer();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();
    x.AddConsumer<UserNameChangedRegisterConsumer>();
    x.UsingRabbitMq((context, config) =>
    {
        config.Host(
            $"amqp://{builder.Configuration["RabbitMQ:User"]}:{builder.Configuration["RabbitMQ:Password"]}@{builder.Configuration["RabbitMQ:HostName"]}");
        config.ConfigureEndpoints(context);
    });
});

builder.Services.AddTransient<ITokenService, TokenService>();
builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.AddTransient<IUserService, UserService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHangfireDashboard();
app.MapControllers();

app.Run();

public partial class Program{}