using System.Diagnostics.CodeAnalysis;

namespace GGroupp.Internal.Support
{
    public sealed record CustomerSetFindIn
    {
        public CustomerSetFindIn([AllowNull] string searchText)
            =>
            SearchText = searchText ?? string.Empty;

        public string SearchText { get; }
    }
}