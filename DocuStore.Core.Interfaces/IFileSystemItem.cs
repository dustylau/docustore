using System;
using System.Collections.Generic;
using DocuStore.Core.Shared;

namespace DocuStore.Core.Interfaces
{
    public interface IFileSystemItem : IHasTags, ICopyable<IFileSystemItem>
    {
        public Guid Id { get; set; }
        public Guid? ParentId { get; set; }
        public FileSystemItemType ItemType { get; set; }
        public string Path { get; set; }
        public string Name { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}