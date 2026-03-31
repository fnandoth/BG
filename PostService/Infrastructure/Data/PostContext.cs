using Microsoft.EntityFrameworkCore;
using PostService.Domain.Entities;

namespace PostService.Infrastructure.Data
{
    public class PostContext : DbContext
    {
        public PostContext(DbContextOptions<PostContext> options) : base(options)
        {
        }

        public DbSet<Post> Posts { get; set; }
        public DbSet<Like> Likes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Post>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.OwnsOne(e => e.AuthorSnapshot, snapshot =>
                {
                    snapshot.ToJson(); // mapea el AuthorSnapshot como JSONB en PostgreSQL
                });

                entity.Property(e => e.MediaUrls)
                      .HasColumnType("text[]");

                // Self-referencing: Reply → ParentPost
                entity.HasOne(e => e.ParentPost)
                      .WithMany()
                      .HasForeignKey(e => e.ParentPostId)
                      .OnDelete(DeleteBehavior.Restrict)
                      .IsRequired(false);

                // Self-referencing: Quote → QuotedPost
                entity.HasOne(e => e.QuotedPost)
                      .WithMany()
                      .HasForeignKey(e => e.QuotedPostId)
                      .OnDelete(DeleteBehavior.Restrict)
                      .IsRequired(false);
            });

            modelBuilder.Entity<Like>(entity =>
            {
                // Composite PK — no single Id field
                entity.HasKey(e => new { e.UserId, e.PostId });

                entity.HasOne(e => e.Post)
                      .WithMany()
                      .HasForeignKey(e => e.PostId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}