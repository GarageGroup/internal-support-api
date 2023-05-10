using System;
using System.Threading;
using System.Threading.Tasks;

namespace GarageGroup.Internal.Support;

public interface IIncidentCreateSupplier
{
    ValueTask<Result<IncidentCreateOut, Failure<IncidentCreateFailureCode>>> CreateIncidentAsync(
        IncidentCreateIn input, CancellationToken cancellationToken);
}