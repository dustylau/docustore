using System;
using System.ComponentModel.DataAnnotations.Schema;
using DocuStore.Core.Interfaces;
using DocuStore.Core.Shared;
using Newtonsoft.Json;

namespace DocStore.Repository.Sql
{
    public partial class FileSystem
    {
        public partial class File : FileSystemItem, IFile
        {
            public File() : base(FileSystemItemType.File)
            {
            }

            public File(Directory directory) : this()
            {
                ParentId = directory.Id;
                ParentDirectory = directory;
            }

            public File(Directory directory, string name) : this(directory)
            {
                ParentDirectory = directory;

                Name = name;

                var parentPath = directory.Path.Equals(FileSystemOptions.DirectorySeparator) ? "" : directory.Path;

                Path = $"{parentPath}{FileSystemOptions.DirectorySeparator}{name}";
            }

            public File(IDirectory directory) : this(directory as Directory)
            {
            }

            public File(IDirectory directory, string name) : this(directory as Directory, name)
            {
            }

            public override FileSystemItemType ItemType
            {
                get => FileSystemItemType.File;
                set{}
            }

            public Guid ContentId { get; set; }

            [ForeignKey("ContentId")]
            public Content Content { get; set; }

            [ForeignKey("ParentId")]
            [JsonIgnore]
            public Directory ParentDirectory { get; set; }

            IContent IFile.Content => Content;
        }
    }
}