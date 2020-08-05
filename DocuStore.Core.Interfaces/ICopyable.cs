namespace DocuStore.Core.Interfaces
{
    public interface ICopyable<T>
    {
        void CopyFrom(T from);
        void CopyTo(T to);
    }
}