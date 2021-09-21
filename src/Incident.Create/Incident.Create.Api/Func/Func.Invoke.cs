using GGroupp.Infra;
using System;
using System.Threading;
using System.Threading.Tasks;
using static System.FormattableString;

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
                    selectFields: selectedFields,
                    entityData: new(
                        ownerId: Invariant($"/systemusers({input.OwnerId:D})"),
                        customerId: Invariant($"/accounts({input.CustomerId:D})"),
                        title: input.Title,
                        description: input.Description)))
        .PipeValue(
            entityCreateSupplier.CreateEntityAsync<CreateIncidentJsonIn, CreateIncidentJsonOut>)
        .MapFailure(
            failure => failure.MapFailureCode(MapDataverseFailureCode))
        .MapSuccess(
            entityCreateOut =>
                new IncidentCreateOut(
                    id: entityCreateOut?.Value?.IncidentId ?? default,
                    title: entityCreateOut?.Value?.Title));

    public static IncidentCreateFailureCode MapDataverseFailureCode(int dataverseFailureCode)
        =>
        dataverseFailureCode switch
        {
            ApiNames.NotFoundFailureCode => IncidentCreateFailureCode.NotFound,
            _ => IncidentCreateFailureCode.Unknown
        };
}