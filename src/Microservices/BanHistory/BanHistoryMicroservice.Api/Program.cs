using BanHistoryMicroservice.Api.Database;
using BanHistoryMicroservice.Api.MessageBus.MessageBusConsumers;
using BanHistoryMicroservice.Api.Models;
using BanHistoryMicroservice.Api.Services;
using BanHistoryMicroservice.Api.Services.Pagination;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(x => x.UseNpgsql(
    builder.Configuration["Database:ConnectionString"]));

builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();
    x.AddConsumer<UserNameChangedBanHistoryConsumer>();
    x.AddConsumer<UserBannedByUserNameBanHistoryConsumer>();
    x.AddConsumer<UserBannedByUserIdBanHistoryConsumer>();
    x.UsingRabbitMq((context, config) =>
    {
        //config.Host(
        //    $"amqp://{builder.Configuration["RabbitMQ:User"]}:{builder.Configuration["RabbitMQ:Password"]}@{builder.Configuration["RabbitMQ:HostName"]}");
        config.Host(builder.Configuration["RabbitMQ:HostName"], builder.Configuration["RabbitMQ:VirtualHost"], h =>
        {
            h.Username(builder.Configuration["RabbitMQ:User"]!);
            h.Password(builder.Configuration["RabbitMQ:Password"]!);
        });
        config.ConfigureEndpoints(context);
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<IBanService<Ban>, BanService>();
builder.Services.AddTransient<IPaginationService, PaginationService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMetricServer();
app.UseHttpMetrics();

app.UseAuthorization();

app.MapControllers();

app.Run();
