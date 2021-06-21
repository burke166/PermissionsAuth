using System;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using PermissionsAuth.Constants;

namespace PermissionsAuth.Data
{
    public class Db : DbContext
    {
        public DbSet<UserAccount> UserAccounts { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<AccessToken> Tokens { get; set; }
        public DbSet<Log> Logs { get; set; }

        public Db(DbContextOptions opt) : base(opt)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<AccessToken>()
                .ToTable("Tokens");

            builder.Entity<UserRole>()
                .Property(e => e._Permissions)
                .HasColumnName("Permissions");

            builder.Entity<UserAccountUserRole>()
                .HasKey(e => new { e.UserAccountId, e.UserRoleId });

            builder.Entity<UserRole>().HasData(
                new UserRole
                {
                    Id = 1,
                    Name = Roles.SuperAdmin.ToString(),
                    _Permissions = JsonSerializer.Serialize(new List<string>
                    {
                        Permissions.All
                    })
                },
                new UserRole
                {
                    Id = 2,
                    Name = Roles.Admin.ToString(),
                    _Permissions = JsonSerializer.Serialize(new List<string>
                    {
                        Permissions.Users.View
                    })
                },
                new UserRole
                {
                    Id = 3,
                    Name = Roles.Basic.ToString(),
                    _Permissions = JsonSerializer.Serialize(new List<string>())
                });
        }
    }
}
