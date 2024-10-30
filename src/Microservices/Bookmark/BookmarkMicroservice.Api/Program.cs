using BookmarkMicroservice.Api.Database;
using BookmarkMicroservice.Api.MessageBus.Consumers;
using BookmarkMicroservice.Api.Services;
using BookmarkMicroservice.Api.Services.Pagination;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(x =>
    x.UseNpgsql(builder.Configuration["Database:ConnectionString"]));
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();
    x.AddConsumer<UserNameChangedBookmarkConsumer>();
    x.UsingRabbitMq((context, config) =>
    {
        config.Host(
            $"amqp://{builder.Configuration["RabbitMQ:User"]}:{builder.Configuration["RabbitMQ:Password"]}@{builder.Configuration["RabbitMQ:HostName"]}");
        config.ConfigureEndpoints(context);
    });
});

builder.Services.AddTransient<IBookmarkService, BookmarkService>();
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
