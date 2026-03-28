namespace NotificationService.Aplication.Queries
{
    public record GetNotificationsQuery(
        Guid RecipientId,
        int  Page       = 1,
        int  PageSize   = 20,
        bool OnlyUnread = false
    );

}
