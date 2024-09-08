using MassTransit;
using Microsoft.EntityFrameworkCore;
using UserMicroservice.Api.Database;
using UserMicroservice.Api.MessageBus_Consumers;
using UserMicroservice.Api.MessageBus_Consumers.CommentConsumers;
using UserMicroservice.Api.Models;
using UserMicroservice.Api.Services.Ban;
using UserMicroservice.Api.Services.User;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(x => x.UseNpgsql(
    builder.Configuration["Database:ConnectionString"]));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();
    x.AddConsumer<CommentDeletedConsumer>();
    x.AddConsumer<CommentCreatedConsumer>();
    x.AddConsumer<SuggestedCommentRejectedConsumer>();
    x.AddConsumer<SuggestedCommentAcceptedConsumer>();
    x.AddConsumer<UserSuggestedCommentConsumer>();
    x.AddConsumer<SuggestedDiscussionAcceptedConsumer>();
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

builder.Services.AddTransient<IUserService<User>, UserService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
