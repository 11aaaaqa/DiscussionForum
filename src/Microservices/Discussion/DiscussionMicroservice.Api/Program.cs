using DiscussionMicroservice.Api.Database;
using DiscussionMicroservice.Api.MessageBus.Consumers;
using DiscussionMicroservice.Api.Services;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(x => x.UseNpgsql(
    builder.Configuration["Database:ConnectionString"]));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();
    x.AddConsumer<UserNameChangedDiscussionConsumer>();
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

builder.Services.AddTransient<IGetAllDiscussionsService, GetAllDiscussionsService>();
builder.Services.AddTransient<ICheckForNextDiscussionsPageExisting, CheckForNextDiscussionsPageExistingService>();

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

public partial class Program;