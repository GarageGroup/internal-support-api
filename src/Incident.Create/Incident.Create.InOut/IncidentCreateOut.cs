using System;
using System.Diagnostics.CodeAnalysis;

namespace GGroupp.Internal.Support;

public readonly record struct IncidentCreateOut
{
    private readonly string? title;

    public IncidentCreateOut(Guid id, [AllowNull] string title)
    {
        Id = id;
        this.title = title.OrNullIfEmpty();
    }

    public Guid Id { get; }

    public string Title => title.OrEmpty();
}