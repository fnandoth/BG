using Microsoft.EntityFrameworkCore;
using NotificationService.Domain.Entities;

namespace NotificationService.Infrastructure.Data
{
    public class NotificationsContext : DbContext
    {
        public NotificationsContext(DbContextOptions<NotificationsContext> options)
            : base(options) { }

        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Type)
                      .HasConversion<string>();   

                // ActorSnapshot → JSONB 
                entity.OwnsOne(e => e.ActorSnapshot, actor =>
                {
                    actor.ToJson();
                });

                // EntitySnapshot → JSONB 
                entity.OwnsOne(e => e.EntitySnapshot, snapshot =>
                {
                    snapshot.ToJson();
                });
            });
        }
    }
}