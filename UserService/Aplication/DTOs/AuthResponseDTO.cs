namespace UserService.Aplication.DTOs
{
    public class AuthResponseDTO
    {
        public required string AccessToken { get; set; }
        public required DateTime ExpiresAt { get; set; }
        public required UserResponseDTO User { get; set; }
    }
}
