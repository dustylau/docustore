using System;

namespace DocuStore.Core.Shared
{
    [Flags]
    public enum GetDirectoryOptions
    {
        None = 0,
        Directories = 1,
        Files = 2,
        FileContents = 4,
        ExcludeTags = 8,
        Recurse = 16
    }
}