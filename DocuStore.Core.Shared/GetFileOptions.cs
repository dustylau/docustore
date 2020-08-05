using System;

namespace DocuStore.Core.Shared
{
    [Flags]
    public enum GetFileOptions
    {
        None = 0,
        IncludeFileContents = 1,
        IgnoreFileContents = 2,
        ExcludeTags = 4
    }
}