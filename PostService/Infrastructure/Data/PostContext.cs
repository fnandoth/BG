using Microsoft.EntityFrameworkCore;

namespace PostService.Infrastructure.Data
{
    public class PostContext : DbContext
    {
        public PostContext(DbContextOptions<PostContext> options) : base(options)
        {
        }

        public DbSet<Domain.Entities.Post> Posts { get; set; }
        public DbSet<Domain.Entities.Like> Likes { get; set; }


    }
}
