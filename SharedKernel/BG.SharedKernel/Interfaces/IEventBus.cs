using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedKernel.BG.SharedKernel.Events;

namespace SharedKernel.BG.SharedKernel.Interfaces
{
    public interface IEventBus
    {
        Task PublishAsync<T>(T integrationEvent, CancellationToken ct = default)
            where T : IntegrationEvent;
    }
}
