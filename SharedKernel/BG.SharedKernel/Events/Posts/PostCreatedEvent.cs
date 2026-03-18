using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel.BG.SharedKernel.Events.Posts
{
    public record PostCreatedEvent : IntegrationEvent
    {
        public Guid PostId { get; init; }
        public Guid AuthorId { get; init; }
        public string Content { get; init; } = default!;
        public IReadOnlyList<string> Topics { get; init; } = [];
        public DateTime CreatedAt { get; init; }
    }
}
