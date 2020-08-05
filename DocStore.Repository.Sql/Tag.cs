using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DocuStore.Core.Interfaces;
using MassTransit;
using Toolbelt.ComponentModel.DataAnnotations.Schema;

namespace DocStore.Repository.Sql
{
    public partial class FileSystem
    {
        public partial class Tag : ITag
        {
            public Tag()
            {
                // Id = NewId.NextGuid();
            }

            public Tag(Guid itemId) : this()
            {
                ItemId = itemId;
            }

            public Tag(IFileSystemItem item) : this(item.Id)
            {
                ItemId = item.Id;
            }

            public Tag(Guid itemId, string name) : this(itemId)
            {
                Name = name;
            }

            public Tag(IFileSystemItem item, string name) : this(item.Id, name)
            {
            }

            public Tag(Guid itemId, string name, string value) : this(itemId, name)
            {
                Value = value;
            }

            public Tag(IFileSystemItem item, string name, string value) : this(item.Id, name, value)
            {
            }

            [Key] 
            [Required] 
            public virtual Guid Id { get; set; }

            [Required]
            public Guid ItemId { get; set; }

            [Required(AllowEmptyStrings = false, ErrorMessage = "Name is required.")]
            [StringLength(50)]
            [Index]
            public virtual string Name { get; set; }

            [StringLength(int.MaxValue)] 
            public virtual string Value { get; set; }

            [ForeignKey("ItemId")]
            public virtual FileSystemItem Item { get; set; }
        }
    }
}