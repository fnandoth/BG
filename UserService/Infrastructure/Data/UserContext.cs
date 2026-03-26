using System.Data;
using Microsoft.EntityFrameworkCore;
using UserService.Domain.Entities;

namespace UserService.Infrastructure.Data
{
    public class UserContext :  DbContext
    {
        public UserContext (DbContextOptions<UserContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Follow> Follows { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UserName).IsRequired().HasMaxLength(40);
                entity.Property(e => e.Email).IsRequired();
                entity.Property(e => e.DisplayName).IsRequired().HasMaxLength(32);
            });
        }


    }
}
