namespace UserService.Domain.Entities
{
    public class Follow
    {
        public Guid FollowerId { get; set; }
        public Guid FollowingId { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = default;

        // navegacion 
        public User? Following { get; set; }
        public User? Follower { get; set; }
    }

}
