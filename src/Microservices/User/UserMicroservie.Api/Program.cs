using MassTransit;
using Microsoft.EntityFrameworkCore;
using UserMicroservice.Api.Database;
using UserMicroservice.Api.MessageBus_Consumers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(x => x.UseNpgsql(
    builder.Configuration["Database:ConnectionString"]));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();
    x.AddConsumer<SuggestedDiscussionRejectedConsumer>();
    x.AddConsumer<UserSuggestedDiscussionConsumer>();
    x.AddConsumer<UserRegisteredConsumer>();
    x.UsingRabbitMq((context, config) =>
    {
        config.Host(
            $"amqp://{builder.Configuration["RabbitMQ:User"]}:{builder.Configuration["RabbitMQ:Password"]}@{builder.Configuration["RabbitMQ:HostName"]}");
        config.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
