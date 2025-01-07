using MassTransit;
using Microsoft.EntityFrameworkCore;
using Prometheus;
using TopicMicroservice.Api.Database;
using TopicMicroservice.Api.Models;
using TopicMicroservice.Api.Services;
using TopicMicroservice.Api.Services.MessageBus_Consumers;
using TopicMicroservice.Api.Services.Repository;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(x => x.UseNpgsql(
    builder.Configuration["Database:ConnectionString"]));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();
    x.AddConsumer<UserNameChangedTopicConsumer>();
    x.AddConsumer<DiscussionDeletedTopicConsumer>();
    x.AddConsumer<DiscussionAddedTopicConsumer>();
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

builder.Services.AddTransient<IRepository<Topic>, TopicRepository>();
builder.Services.AddTransient<ITopicService, TopicService>();
builder.Services.AddTransient<IGetTopicsService, TopicService>();

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
