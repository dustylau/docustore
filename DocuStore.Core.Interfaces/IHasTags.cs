using System.Collections.Generic;

namespace DocuStore.Core.Interfaces
{
    public interface IHasTags
    {
        public ICollection<ITag> Tags { get; }
    }
}