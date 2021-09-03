using GGroupp.Infra;
using System.Collections.ObjectModel;

namespace GGroupp.Internal.Support;

internal sealed partial class IncidentCreateFunc : IAsyncValueFunc<IncidentCreateIn, Result<IncidentCreateOut, Failure<IncidentCreateFailureCode>>>
{
    private static readonly ReadOnlyCollection<string> selectedFields;

    static IncidentCreateFunc()
        =>
        selectedFields = new(new[] { ApiJsonFieldName.IncidentId, ApiJsonFieldName.Title });

    private readonly IDataverseEntityCreateSupplier entityCreateSupplier;

    private IncidentCreateFunc(IDataverseEntityCreateSupplier entityCreateSupplier)
        =>
        this.entityCreateSupplier = entityCreateSupplier;

    public static IncidentCreateFunc Create(IDataverseEntityCreateSupplier entityCreateSupplier)
        =>
        new(
            entityCreateSupplier ?? throw new ArgumentNullException(nameof(entityCreateSupplier)));

    public partial ValueTask<Result<IncidentCreateOut, Failure<IncidentCreateFailureCode>>> InvokeAsync(
        IncidentCreateIn input, CancellationToken cancellationToken = default);
}