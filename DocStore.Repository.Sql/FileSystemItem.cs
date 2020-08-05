using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using DocuStore.Core.Interfaces;
using DocuStore.Core.Shared;
using MassTransit;
using Toolbelt.ComponentModel.DataAnnotations.Schema;

namespace DocStore.Repository.Sql
{
    public partial class FileSystem
    {
        public partial class FileSystemItem : IFileSystemItem
        {
            public FileSystemItem()
            {
                // Id = NewId.NextGuid();
                CreateDate = DateTime.Now;
                Tags = new List<Tag>();
            }

            public FileSystemItem(FileSystemItemType itemType) : this()
            {
                ItemType = itemType;
            }

            #region IFileSystemItem

            [Key] 
            [Required] 
            public virtual Guid Id { get; set; }

            [ForeignKey("FileSystemItem")]
            public virtual Guid? ParentId { get; set; }

            [Required]
            public virtual FileSystemItemType ItemType { get; set; }

            [Required(AllowEmptyStrings = false)]
            [StringLength(256)]
            [Index]
            public virtual string Path { get; set; }

            [Required(AllowEmptyStrings = false)]
            [StringLength(128)]
            [Index]
            public virtual string Name { get; set; }

            [Required] 
            public virtual DateTime CreateDate { get; set; }

            public virtual DateTime? ModifiedDate { get; set; }

            #endregion

            #region IHasTags
            [InverseProperty("Item")]
            public virtual ICollection<Tag> Tags { get; set; }
            ICollection<ITag> IHasTags.Tags => Tags.Cast<ITag>().ToList();

            #endregion

            #region ICopyable<IFileSystemItem>

            public virtual void CopyFrom(IFileSystemItem @from)
            {
                Id = @from.Id;
                ParentId = @from.ParentId;
                ItemType = @from.ItemType;
                Path = @from.Path;
                Name = @from.Name;
                CreateDate = @from.CreateDate;
                ModifiedDate = @from.ModifiedDate;
            }

            public virtual void CopyTo(IFileSystemItem @to)
            {
                @to.Id = Id;
                @to.ParentId = ParentId;
                @to.ItemType = ItemType;
                @to.Path = Path;
                @to.Name = Name;
                @to.CreateDate = CreateDate;
                @to.ModifiedDate = ModifiedDate;
            }

            #endregion
        }
    }
}