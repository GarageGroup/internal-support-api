#nullable enable


using System;
using System.Threading;
using System.Threading.Tasks;

namespace GGroupp.Internal.Support
{
    partial class IncidentCreateFunc
    {
        public partial ValueTask<Result<IncidentCreateOut, Failure<IncidentCreateFailureCode>>> InvokeAsync(
            IncidentCreateIn input, CancellationToken cancellationToken)
            =>
            AsyncPipeline.Start(
                input ?? throw new ArgumentNullException(nameof(input)),
                cancellationToken)
            .Pipe(
                )
    }
}
