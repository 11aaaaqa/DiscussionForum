using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RegisterMicroservice.Api.Models.UserModels;

namespace RegisterMicroservice.Api.Services.gRPC
{
    public class UserGrpcService : UserService.UserServiceBase
    {
        private readonly UserManager<User> userManager;

        public UserGrpcService(UserManager<User> userManager)
        {
            this.userManager = userManager;
        }

        public override async Task<GetAllUsersResponse> GetAllUsersAsync(Empty request, ServerCallContext context)
        {
            var users = new GetAllUsersResponse();
            
            var userList = await userManager.Users
                .Select(x => new UserResponse{
                    Id = x.Id, Email = x.Email, Answers = x.Answers,
                    UserName = x.UserName, RefreshToken = x.RefreshToken,
                    EmailConfirmed = x.EmailConfirmed, Posts = x.Posts, RegisteredAt = x.RegisteredAt.ToString()
                })
                .ToListAsync();
            users.Users.AddRange(userList);
            return users;
        }

        public override async Task<AddUserResponse> AddUserAsync(AddUserRequest request, ServerCallContext context)
        {
            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                Email = request.Email,
                UserName = request.UserName,
                Answers = 0,
                EmailConfirmed = false,
                Posts = 0,
                RefreshToken = null,
                RegisteredAt = DateOnly.FromDateTime(DateTime.Now)
            };

            var result = await userManager.CreateAsync(user, request.Password);
            if (result.Succeeded)
                return new AddUserResponse { IsAdded = true };
            
            return new AddUserResponse { IsAdded = false };
        }

        public override async Task<UpdateUserResponse> UpdateUserAsync(UpdateUserRequest request, ServerCallContext context)
        {
            var user = new User { UserName = request.UserName, Email = request.Email, Id = request.Id};

            var result = await userManager.UpdateAsync(user);

            if (result.Succeeded)
                return new UpdateUserResponse { Id = user.Id, Email = user.Email, UserName = user.UserName };
            return new UpdateUserResponse();
        }

        public override async Task<UserDeletedResponse> DeleteUserByEmailAsync(DeleteUserByEmailRequest request, ServerCallContext context)
        {
            var result = await userManager.DeleteAsync(new User { Email = request.Email });

            if (result.Succeeded)
                return new UserDeletedResponse { IsDeleted = true };
            return new UserDeletedResponse { IsDeleted = false };
        }

        public override async Task<UserDeletedResponse> DeleteUserByIdAsync(DeleteUserByIdRequest request, ServerCallContext context)
        {
            var result = await userManager.DeleteAsync(new User { Id = request.Id });

            if (result.Succeeded)
                return new UserDeletedResponse { IsDeleted = true };
            return new UserDeletedResponse { IsDeleted = false };
        }

        public override async Task<UserDeletedResponse> DeleteUserByUserNameAsync(DeleteUserByUserNameRequest request, ServerCallContext context)
        {
            var result = await userManager.DeleteAsync(new User { UserName = request.UserName });

            if (result.Succeeded)
                return new UserDeletedResponse { IsDeleted = true };
            return new UserDeletedResponse { IsDeleted = false };
        }

        public override async Task<UserResponse> GetUserByEmailAsync(GetUserByEmailRequest request, ServerCallContext context)
        {
            var user = await userManager.FindByEmailAsync(request.Email);
            if (user != null)
            {
                return new UserResponse {
                    Email = user.Email,
                    UserName = user.UserName,
                    RefreshToken = user.RefreshToken,
                    Answers = user.Answers,
                    EmailConfirmed = user.EmailConfirmed,
                    Id = user.Id,
                    Posts = user.Posts,
                    RegisteredAt = user.RegisteredAt.ToString()
                };
            }

            return new UserResponse();
        }

        public override async Task<UserResponse> GetUserByIdAsync(GetUserByIdRequest request, ServerCallContext context)
        {
            var user = await userManager.FindByIdAsync(request.Id);
            if (user != null)
            {
                return new UserResponse
                {
                    Email = user.Email,
                    UserName = user.UserName,
                    RefreshToken = user.RefreshToken,
                    Answers = user.Answers,
                    EmailConfirmed = user.EmailConfirmed,
                    Id = user.Id,
                    Posts = user.Posts,
                    RegisteredAt = user.RegisteredAt.ToString()
                };
            }

            return new UserResponse();
        }

        public override async Task<UserResponse> GetUserByUserNameAsync(GetUserByUserNameRequest request, ServerCallContext context)
        {
            var user = await userManager.FindByNameAsync(request.UserName.ToUpper());
            if (user != null)
            {
                return new UserResponse
                {
                    Email = user.Email,
                    UserName = user.UserName,
                    RefreshToken = user.RefreshToken,
                    Answers = user.Answers,
                    EmailConfirmed = user.EmailConfirmed,
                    Id = user.Id,
                    Posts = user.Posts,
                    RegisteredAt = user.RegisteredAt.ToString()
                };
            }

            return new UserResponse();
        }
    }
}
