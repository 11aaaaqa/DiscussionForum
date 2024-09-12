using MassTransit;
using Microsoft.EntityFrameworkCore;
using ReportMicroservice.Api.Database;
using ReportMicroservice.Api.Models;
using ReportMicroservice.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(x => x.UseNpgsql(
    builder.Configuration["Database:ConnectionString"]));
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();
    x.UsingRabbitMq((context, config) =>
    {
        config.Host(
            $"amqp://{builder.Configuration["RabbitMQ:User"]}:{builder.Configuration["RabbitMQ:Password"]}@{builder.Configuration["RabbitMQ:HostName"]}");
        config.ConfigureEndpoints(context);
    });
});

builder.Services.AddTransient<IReportService<Report>, ReportService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
