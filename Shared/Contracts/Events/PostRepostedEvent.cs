using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Contracts.Events
{
    public record PostRepostedEvent(
        Guid PostId,
        Guid AuthorId,
        Guid ReposterUserId,
        string ReposterUsername,
        string ReposterDisplayName,
        string? ReposterAvatarUrl,
        string ContentPreview,
        DateTimeOffset OccurredAt
    );
}
