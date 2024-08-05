using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RegisterMicroservice.Api.Models.UserModels;

namespace RegisterMicroservice.Api.Database
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>()
                .Ignore(x => x.PhoneNumber)
                .Ignore(x => x.PhoneNumberConfirmed)
                .Ignore(x => x.TwoFactorEnabled)
                .Ignore(x => x.LockoutEnabled)
                .Ignore(x => x.LockoutEnd)
                .Ignore(x => x.AccessFailedCount)
                .ToTable("Users");
        }
    }
}
