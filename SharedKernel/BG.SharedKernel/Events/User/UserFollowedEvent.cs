using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel.BG.SharedKernel.Events.User
{
    public record UserFollowedEvent : IntegrationEvent
    {
        public Guid FollowerId { get; init; }
        public Guid FollowedId { get; init; }
    }
}
