using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using DocuStore.Core.Interfaces;
using DocuStore.Core.Shared;
using Newtonsoft.Json;

namespace DocStore.Repository.Sql
{
    public partial class FileSystem
    {
        public partial class Directory : FileSystemItem, IDirectory
        {
            public Directory() : base(FileSystemItemType.Directory)
            {
                Directories = new List<Directory>();
                Files = new List<File>();
            }

            public Directory(Directory parent) : this()
            {
                ParentId = parent.Id;
                ParentDirectory = parent;
            }

            public Directory(Directory parent, string name) : this(parent)
            {
                Name = name;

                var parentPath = parent.Path.Equals(FileSystemOptions.DirectorySeparator) ? "" : parent.Path;

                Path = $"{parentPath}{FileSystemOptions.DirectorySeparator}{name}";
            }

            public Directory(IDirectory parent) : this(parent as Directory)
            {
            }

            public Directory(IDirectory parent, string name) : this(parent as Directory, name)
            {
            }

            [ForeignKey("ParentId")]
            [JsonIgnore]
            public virtual Directory ParentDirectory { get; set; }

            [InverseProperty("ParentDirectory")]
            public ICollection<Directory> Directories { get; set; }

            [InverseProperty("ParentDirectory")]
            public ICollection<File> Files { get; set; }

            ICollection<IDirectory> IDirectory.Directories => Directories.Cast<IDirectory>().ToList();
            ICollection<IFile> IDirectory.Files => Files.Cast<IFile>().ToList();
        }
    }
}