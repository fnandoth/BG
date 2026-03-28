using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Contracts.Events
{
    public record PostMentionedEvent(
        Guid PostId,
        Guid MentionedUserId,    // recipient
        Guid MentionerUserId,
        string MentionerUsername,
        string MentionerDisplayName,
        string? MentionerAvatarUrl,
        string ContentPreview,
        DateTimeOffset OccurredAt
    );
}
