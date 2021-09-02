using System;
using System.Diagnostics.CodeAnalysis;

namespace GGroupp.Internal.Support;

public sealed record CustomerItemFindOut
{
    public CustomerItemFindOut(Guid id, [AllowNull] string title)
    {
        Id = id;
        Title = title ?? string.Empty;
    }

    public Guid Id { get; }

    public string Title { get; }
}