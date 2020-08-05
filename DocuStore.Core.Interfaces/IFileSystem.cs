using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DocuStore.Core.Shared;

namespace DocuStore.Core.Interfaces
{
    public interface IFileSystem
    {
        IQueryable<IFileSystemItem> Items { get; }
        Task<IFileSystemItem> GetFileSystemItem(Guid id);
        Task<IFileSystemItem> GetFileSystemItem(Expression<Func<IFileSystemItem, bool>> filter);

        Task<IDirectory> CreateDirectory(string path);
        Task<IDirectory> CreateDirectory(string path, string name);
        Task<IDirectory> CreateDirectory(IDirectory parentDirectory, string name);
        Task InsertDirectory(IDirectory directory);
        Task<IDirectory> GetDirectory(Guid id);
        Task<IDirectory> GetDirectory(Guid id, GetDirectoryOptions options);
        Task<IDirectory> GetDirectory(string path);
        Task<IDirectory> GetDirectory(string path, GetDirectoryOptions options);
        Task UpdateDirectory(IDirectory directory);
        Task DeleteDirectory(Guid id);
        Task DeleteDirectory(IDirectory directory);

        Task<IFile> CreateFile(string path, byte[] data);
        Task<IFile> CreateFile(string path, string name, byte[] data);
        Task<IFile> CreateFile(string path, Stream data);
        Task<IFile> CreateFile(string path, string name, Stream data);
        Task<IFile> CreateFile(IDirectory directory, string name, byte[] data);
        Task<IFile> CreateFile(IDirectory directory, string name, Stream data);
        Task InsertFile(IFile file);
        Task<IFile> GetFile(Guid id);
        Task<IFile> GetFile(Guid id, GetFileOptions options);
        Task<IFile> GetFile(string path);
        Task<IFile> GetFile(string path, GetFileOptions options);
        Task UpdateFile(IFile file);
        Task DeleteFile(Guid id);
        Task DeleteFile(IFile file);

        Task<ITag> CreateTag(string path, string name);
        Task<ITag> CreateTag(string path, string name, string value);
        Task<ITag> CreateTag(IFileSystemItem item, string name);
        Task<ITag> CreateTag(IFileSystemItem item, string name, string value);
        Task<ITag> CreateTag(Guid id, string name);
        Task<ITag> CreateTag(Guid id, string name, string value);
        Task InsertTag(ITag tag);
        Task<ITag> GetTag(Guid id);
        Task<ITag> GetTag(string path, string name);
        Task<ITag> GetTag(Guid id, string name);
        Task<ITag> GetTag(IFileSystemItem item, string name);
        Task UpdateTag(ITag tag);
        Task DeleteTag(Guid id);
        Task DeleteTag(ITag tag);
        Task RunMigrations();
    }
}