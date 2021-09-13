using GGroupp.Infra;
using System.Collections.ObjectModel;

namespace GGroupp.Internal.Support;

using IIncidentCreateFunc = IAsyncValueFunc<IncidentCreateIn, Result<IncidentCreateOut, Failure<IncidentCreateFailureCode>>>;

internal sealed partial class IncidentCreateFunc : IIncidentCreateFunc
{
    private static readonly ReadOnlyCollection<string> selectedFields;

    static IncidentCreateFunc()
        =>
        selectedFields = new(new[] { ApiNames.IncidentId, ApiNames.Title });

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