using System;
using System.Collections.Generic;

namespace DocuStore.Core.Interfaces
{
    public interface IDirectory : IFileSystemItem
    {
        ICollection<IDirectory> Directories { get; }
        ICollection<IFile> Files { get; }
    }
}