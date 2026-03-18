using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel.BG.SharedKernel.ValueObjects
{
    public record UserId(Guid Value)
    {
        public static UserId New() => new(Guid.NewGuid());
        public static UserId From(Guid id) => new(id);
        public override string ToString() => Value.ToString();
    }
}
