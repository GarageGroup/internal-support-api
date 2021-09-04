using GGroupp.Infra;
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
            MapDataverseFailureCode)
        .MapSuccess(
            entityCreateOut =>
                new IncidentCreateOut(
                    id: entityCreateOut?.Value?.IncidentId ?? default,
                    title: entityCreateOut?.Value?.Title));

    public static Failure<IncidentCreateFailureCode> MapDataverseFailureCode(Failure<int> dataverseFailureCode)
        =>
        dataverseFailureCode switch
        {
            { FailureCode: ApiNames.NotFoundFailureCode} => dataverseFailureCode.MapFailureCode( _ => IncidentCreateFailureCode.NotFound),
            _ => dataverseFailureCode.MapFailureCode(_ => IncidentCreateFailureCode.Unknown)
        };
}