using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel.BG.SharedKernel.Events.Posts
{
    public record PostDeletedEvent : IntegrationEvent
    {
        public Guid PostId { get; init; }
        public Guid AuthorId { get; init; }
    }
}
