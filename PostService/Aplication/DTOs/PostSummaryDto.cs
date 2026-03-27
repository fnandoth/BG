using PostService.Domain.ValueObjects;

namespace PostService.Aplication.DTOs
{
    public class PostSummaryDto // post ligero para no sobrecargar la consulta 
    {
        public Guid Id { get; set; }
        public string Content { get; set; } = default!;
        public AuthorSnapshot Author { get; set; } = default!;
    }
}
