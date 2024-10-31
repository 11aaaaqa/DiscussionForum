using MassTransit;
using Microsoft.EntityFrameworkCore;
using Prometheus;
using ReportMicroservice.Api.Database;
using ReportMicroservice.Api.MessageBus.Consumers;
using ReportMicroservice.Api.Models;
using ReportMicroservice.Api.Services;
using ReportMicroservice.Api.Services.MessageBusConsumers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(x => x.UseNpgsql(
    builder.Configuration["Database:ConnectionString"]));
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();
    x.AddConsumer<UserNameChangedReportConsumer>();
    x.AddConsumer<DiscussionDeletedReportConsumer>();
    x.AddConsumer<CommentDeletedReportConsumer>();
    x.UsingRabbitMq((context, config) =>
    {
        config.Host(
            $"amqp://{builder.Configuration["RabbitMQ:User"]}:{builder.Configuration["RabbitMQ:Password"]}@{builder.Configuration["RabbitMQ:HostName"]}");
        config.ConfigureEndpoints(context);
    });
});

builder.Services.AddTransient<IReportService<Report>, ReportService>();
builder.Services.AddTransient<IPaginationService, ReportService>();

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
