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
    x.AddConsumer<UserDeletedUserConsumer>();
    x.AddConsumer<DiscussionDeletedUserConsumer>();
    x.AddConsumer<CommentDeletedUserConsumer>();
    x.AddConsumer<CommentCreatedUserConsumer>();
    x.AddConsumer<SuggestedCommentAcceptedUserConsumer>();
    x.AddConsumer<SuggestedDiscussionAcceptedUSerConsumer>();
    x.AddConsumer<UserRegisteredUserConsumer>();
    x.UsingRabbitMq((context, config) =>
    {
        config.Host(
            $"amqp://{builder.Configuration["RabbitMQ:User"]}:{builder.Configuration["RabbitMQ:Password"]}@{builder.Configuration["RabbitMQ:HostName"]}");
        config.ConfigureEndpoints(context);
    });
});

builder.Services.AddTransient<IUserService<User>, UserService>();
builder.Services.AddTransient<ICheckForNormalized, UserService>();
builder.Services.AddTransient<IBanService<User>, BanService>();
builder.Services.AddTransient<IChangeUserName, UserService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
