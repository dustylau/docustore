using System;

namespace DocuStore.Core.Interfaces
{
    public interface IFile : IFileSystemItem
    {
        Guid ContentId { get; }
        IContent Content { get; }
    }
}