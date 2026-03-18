using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel.BG.SharedKernel.Interfaces
{
    public interface ICurrentUser
    {
        Guid UserId { get; }
        string Username { get; }
        bool IsAuthenticated { get; }
    }
}
