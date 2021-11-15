namespace GGroupp.Internal.Support;

internal static class IncidentCreateHelper
{
    internal static string? OrNullIfEmpty(this string? source)
        =>
        string.IsNullOrEmpty(source) ? null : source;

    internal static string OrEmpty(this string? source)
        =>
        source ?? string.Empty;
}
