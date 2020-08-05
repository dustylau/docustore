using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using DocuStore.Core.Interfaces;
using MassTransit;
using Microsoft.Extensions.FileSystemGlobbing.Internal;

namespace DocStore.Repository.Sql
{
    public partial class FileSystem
    {
        public partial class Content : IContent
        {
            private byte[] _data;

            public Content()
            {
                // Id = NewId.NextGuid();
                CreateDate = DateTime.Now;
            }

            public Content(byte[] data) : this()
            {
                Data = data;
            }

            /*
            public Content(File file) : this()
            {
            }

            public Content(IFile file) : this(file as File)
            {
            }

            public Content(File file, byte[] data) : this(file)
            {
                Data = data;
            }

            public Content(IFile file, byte[] data) : this(file as File, data)
            {
            }
            */

            [Key] 
            [Required] 
            public virtual Guid Id { get; set; }

            [Required]
            public virtual DateTime CreateDate { get; set; }

            [Required] 
            public virtual int Size { get; set; }

            [Required] 
            [StringLength(128)] 
            public virtual string Hash { get; set; }

            [Required]
            [MaxLength(int.MaxValue)]
            public virtual byte[] Data
            {
                get => _data;
                set
                {
                    _data = value;

                    if (_data == null)
                    {
                        Size = 0;
                        Hash = null;
                        return;
                    }

                    Size = _data.Length;

                    var hash= SHA256.Create().ComputeHash(_data);

                    var stringBuilder = new StringBuilder();

                    foreach (var hashByte in hash) stringBuilder.Append(hashByte.ToString("X2"));

                    Hash = stringBuilder.ToString();
                }
            }
        }
    }
}