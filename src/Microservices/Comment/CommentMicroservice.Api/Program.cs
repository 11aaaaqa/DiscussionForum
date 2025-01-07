using CommentMicroservice.Api.Database;
using CommentMicroservice.Api.MessageBus.Consumers;
using CommentMicroservice.Api.Models;
using CommentMicroservice.Api.Services;
using CommentMicroservice.Api.Services.Repository;
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
    x.AddConsumer<CommentDeletedCommentConsumer>();
    x.AddConsumer<UserNameChangedCommentConsumer>();
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

builder.Services.AddTransient<IRepository<Comment>, CommentRepository>();
builder.Services.AddTransient<IRepository<SuggestedComment>, SuggestCommentRepository>();
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

public partial class Program;
