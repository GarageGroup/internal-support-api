namespace GGroupp.Internal.Support;

public sealed record CustomerItemSearchOut
{
    public CustomerItemSearchOut(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; }
}