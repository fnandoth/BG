using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Contracts.Events
{
    public record UserFollowedEvent(
        Guid FollowedUserId,     // recipient
        Guid FollowerUserId,
        string FollowerUsername,
        string FollowerDisplayName,
        string? FollowerAvatarUrl,
        DateTimeOffset OccurredAt
    );
}
