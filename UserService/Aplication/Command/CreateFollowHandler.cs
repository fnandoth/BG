using MassTransit;
using UserService.Domain.Interfaces;
using UserService.Domain.ValueObjects;
using Shared.Contracts.Events;

namespace UserService.Aplication.Command
{
    public class CreateFollowHandler
    {
        private readonly IPublishEndpoint _bus;
        private readonly IUserRepository _user;

        public CreateFollowHandler(IPublishEndpoint bus, IUserRepository user)
        {
            _bus = bus;
            _user = user;
        }

        public async Task HandleAsync(CreateFollowCommand cmd, CancellationToken ct)
        {

            // No notificar si alguien se hace follow a sí mismo, aunque no deberia pasar por la logica de dominio, pero por si acaso
            if (cmd.FollowedUserId == cmd.FollowerUserId) return;


            await _bus.Publish(new UserFollowedEvent(
                FollowedUserId: cmd.FollowedUserId,     // recipient
                FollowerUserId: cmd.FollowerUserId, // sender
                FollowerUsername: cmd.FollowerUsername,
                FollowerDisplayName: cmd.FollowerDisplayName,
                FollowerAvatarUrl: cmd.FollowerAvatarUrl,
                OccurredAt: DateTimeOffset.UtcNow
            ), ct);
        }
    }
}


