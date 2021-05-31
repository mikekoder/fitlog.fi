using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fitlog.Web.Models.Auth
{
    public class UserContext : IdentityDbContext<User, Role, Guid>
    {
        public UserContext(DbContextOptions<UserContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.HasDefaultSchema("auth");
            builder.Entity<User>().ToTable("User");
            builder.Entity<Role>().ToTable("Role");
            builder.Entity<IdentityUserLogin<Guid>>().ToTable("Login");
            builder.Entity<IdentityUserRole<Guid>>().ToTable("UserRole");
            builder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaim");  
            builder.Entity<IdentityUserToken<Guid>>().ToTable("UserToken");
            builder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaim");
        }
    }
}
