#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GGroupp.Infra;

namespace GGroupp.Internal.Support
{
    internal sealed partial class IncidentCreateFunc : IAsyncValueFunc<IncidentCreateIn, Result<IncidentCreateOut, Failure<IncidentCreateFailureCode>>>
    {
        public static IncidentCreateFunc Create(IDataverseEntityCreateSupplier entityCreateSupplier, IDataverseEntityGetSupplier entityGetSupplier)
            =>
            new(
                entityCreateSupplier ?? throw new ArgumentNullException(nameof(entityCreateSupplier)),
                entityGetSupplier ?? throw new ArgumentNullException(nameof(entityGetSupplier)));

        private readonly IDataverseEntityCreateSupplier entityCreateSupplier;

        private readonly IDataverseEntityGetSupplier entityGetSupplier;

        private IncidentCreateFunc(IDataverseEntityCreateSupplier entityCreateSupplier, IDataverseEntityGetSupplier entityGetSupplier)
        {
            this.entityCreateSupplier = entityCreateSupplier;
            this.entityGetSupplier = entityGetSupplier;
        }

         public partial ValueTask<Result<IncidentCreateOut, Failure<IncidentCreateFailureCode>>> InvokeAsync(
            IncidentCreateIn input, CancellationToken cancellationToken = default);
    }
}
