using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RegisterMicroservice.Api.Constants;
using RegisterMicroservice.Api.Models.UserModels;

namespace RegisterMicroservice.Api.Database
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<IdentityRole>().HasData(new IdentityRole
            {
                ConcurrencyStamp = Guid.NewGuid().ToString(), Id = Guid.NewGuid().ToString(),
                Name = UserRoleConstants.UserRole, NormalizedName = UserRoleConstants.UserRole.ToUpper()
            },
            new IdentityRole
            {
                ConcurrencyStamp = Guid.NewGuid().ToString(), Id = Guid.NewGuid().ToString(),
                Name = UserRoleConstants.ModeratorRole, NormalizedName = UserRoleConstants.ModeratorRole.ToUpper()
            },
            new IdentityRole
            {
                ConcurrencyStamp = Guid.NewGuid().ToString(), Id = Guid.NewGuid().ToString(),
                Name = UserRoleConstants.AdminRole, NormalizedName = UserRoleConstants.AdminRole.ToUpper()
            });

            builder.Entity<User>()
                .Ignore(x => x.PhoneNumber)
                .Ignore(x => x.PhoneNumberConfirmed)
                .Ignore(x => x.TwoFactorEnabled)
                .Ignore(x => x.LockoutEnabled)
                .Ignore(x => x.LockoutEnd)
                .Ignore(x => x.AccessFailedCount)
                .Ignore(x => x.NormalizedUserName)
                .Ignore(x => x.NormalizedEmail)
                .ToTable("Users");
        }
    }
}
