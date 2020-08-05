using System;

namespace DocuStore.Core.Interfaces
{
    public interface ITag
    {
        public Guid Id { get; set; }
        public Guid ItemId { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
}