using System;

namespace DocuStore.Core.Interfaces
{
    public interface IContent
    {
        public Guid Id { get; }
        public DateTime CreateDate { get; set; }
        int Size { get; set; }
        public string Hash { get; set; }
        public byte[] Data { get; set; }
    }
}