namespace DocuStore.Core.Interfaces
{
    public interface ITaggable : IHasTags
    {
        public ITag GetTag(string name);
        public void AddTag(ITag tag);
        public void AddTag(string name);
        public void AddTag(string name, string value);
        public bool RemoveTag(ITag tag);
        public bool RemoveTag(string name);
        public bool HasTag(string name);
        public bool HasTag(string name, string value);
    }
}