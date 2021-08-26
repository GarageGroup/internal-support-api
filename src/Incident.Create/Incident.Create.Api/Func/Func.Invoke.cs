using GGroupp.Infra;
using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace GGroupp.Internal.Support;

partial class IncidentCreateFunc
{
    public partial ValueTask<Result<IncidentCreateOut, Failure<IncidentCreateFailureCode>>> InvokeAsync(
        IncidentCreateIn input, CancellationToken cancellationToken)
        =>
        AsyncPipeline.Start(
            input ?? throw new ArgumentNullException(nameof(input)),
            cancellationToken)
        .Pipe(
            @in => new DataverseEntityCreateIn<CreateIncidentJsonIn>(
                    entityPluralName: "incidents",
                    selectFields: new[]
                        {
                            ApiJsonFieldName.incidentId,
                            ApiJsonFieldName.title
                        },
                    entityData: new(
                        ownerId: $"/systemusers({input.OwnerId.ToString("D", CultureInfo.InvariantCulture)})",
                        customerId: $"/accounts({input.CustomerId.ToString("D", CultureInfo.InvariantCulture)})",
                        title: input.Title,
                        description: input.Description)))
        .PipeValue(
            entityCreateSupplier.CreateEntityAsync<CreateIncidentJsonIn, CreateIncidentJsonOut>)
        .MapFailure(failure => failure.MapFailureCode(_ => IncidentCreateFailureCode.Unknown))
        .MapSuccess(
            entityCreateOut =>
                new IncidentCreateOut(
                    id: entityCreateOut?.Value?.IncidentId, // Can't convert Guid? -> Guid
                    title: entityCreateOut?.Value?.Title));
}