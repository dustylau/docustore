using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Tracing;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DocuStore.Core.Interfaces;
using DocuStore.Core.Shared;
using DocuStore.Shared;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DocStore.Repository.Sql
{
    public partial class FileSystem : DbContext, IFileSystem
    {
        private readonly string _connectionString;
        private readonly ILogger _logger;
        private readonly ISettings _settings;

        public DbSet<FileSystemItem> FileSystemItems { get; set; }
        public DbSet<File> Files { get; set; }
        public DbSet<Directory> Directories { get; set; }
        public DbSet<Tag> Tags { get; set; }

        public FileSystem()
        {
            try
            {
                _connectionString = ConfigHelper.ConnectionString(
                    "FileSystemRepositoryConnectionString",
                    "Data Source = FileSystem.db"
                );
            }
            catch
            {
                _connectionString = "Data Source = FileSystem.db";
            }
        }

        public FileSystem(ILogger logger, ISettings settings)
        {
            _logger = logger;
            _settings = settings;
            _connectionString = _settings?.FileSystemRepositoryConnectionString ??
                                 ConfigHelper.ConnectionString(
                                     "FileSystemRepositoryConnectionString",
                                     "Data Source = FileSystem.db"
                                     );
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite(_connectionString);

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FileSystemItem>()
                .Property(i => i.ItemType)
                .HasConversion<int>();

            base.OnModelCreating(modelBuilder);
        }

        public virtual IQueryable<IFileSystemItem> Items => FileSystemItems;

        public async Task<IFileSystemItem> GetFileSystemItem(Guid id)
        {
            return await GetFileSystemItem(i => i.Id == id);
        }

        public async Task<IFileSystemItem> GetFileSystemItem(Expression<Func<IFileSystemItem, bool>> filter)
        {
            return await Task.Run(() => FileSystemItems.SingleOrDefault(filter));
        }

        public virtual bool GetParentPathAndName(string path, out string parentPath, out string name)
        {
            parentPath = null;
            name = null;

            if (string.IsNullOrWhiteSpace(path))
                throw new Exception("Path is invalid.");

            if (path == FileSystemOptions.DirectorySeparator)
                return false;

            var lastSeparatorIndex = path.LastIndexOf(FileSystemOptions.DirectorySeparator);

            if (lastSeparatorIndex <= -1)
                throw new Exception($"Path is missing directory separators: {path}");

            parentPath = lastSeparatorIndex == 0 
                ? FileSystemOptions.DirectorySeparator 
                : path.Substring(0, lastSeparatorIndex);

            name = path.Substring(lastSeparatorIndex + 1);

            return true;
        }

        public virtual async Task<IDirectory> CreateDirectory(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new Exception("Path is invalid.");

            if (path == FileSystemOptions.DirectorySeparator)
            {
                var rootDirectory = new Directory()
                {
                    Name = "",
                    Path = "/"
                };

                await Directories.AddAsync(rootDirectory);
                await SaveChangesAsync();
                return rootDirectory;
            }

            if (!GetParentPathAndName(path, out var parentPath, out var name))
                throw new Exception($"Could not get Parent Path and Name: {path}");

            return await CreateDirectory(parentPath, name);
        }

        public virtual async Task<IDirectory> CreateDirectory(string path, string name)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new Exception("Path is invalid.");

            if (string.IsNullOrWhiteSpace(name))
                throw new Exception("Name is invalid.");

            var lastSeparatorIndex = path.LastIndexOf(FileSystemOptions.DirectorySeparator);

            if (lastSeparatorIndex <= -1)
                throw new Exception($"Path is missing directory separators: {path}");

            var parentDirectory =
                await GetDirectoryTracked(path)
                ?? await CreateDirectory(path);

            return await CreateDirectory(parentDirectory, name);
        }

        private async Task<IDirectory> GetDirectoryTracked(string path)
        {
            return await Directories.SingleOrDefaultAsync(d => d.Path == path);
        }

        public async Task<IDirectory> CreateDirectory(IDirectory parentDirectory, string name)
        {
            var directory = new Directory(parentDirectory, name);

            await Directories.AddAsync(directory);

            await SaveChangesAsync(true);

            return directory;
        }

        public virtual async Task InsertDirectory(IDirectory directory)
        {
            var newDirectory = new Directory();

            newDirectory.CopyFrom(directory);

            await Directories.AddAsync(newDirectory);

            await SaveChangesAsync(true);
        }

        public virtual async Task<IDirectory> GetDirectory(Guid id)
        {
            return await GetDirectory(id, FileSystemOptions.DefaultGetDirectoryOptions);
        }

        public virtual async Task<IDirectory> GetDirectory(Guid id, GetDirectoryOptions options)
        {
            return await GetDirectory(directory => directory.Id == id, options);
        }

        public virtual async Task<IDirectory> GetDirectory(string path)
        {
            return await GetDirectory(path, FileSystemOptions.DefaultGetDirectoryOptions);
        }

        public virtual async Task<IDirectory> GetDirectory(string path, GetDirectoryOptions options)
        {
            return await GetDirectory(directory => directory.Path == path, options);
        }

        public virtual async Task<IDirectory> GetDirectory(Expression<Func<Directory, bool>> filter, GetDirectoryOptions options)
        {
            var query = Directories.AsNoTracking();

            if (options.HasFlag(GetDirectoryOptions.Directories))
                query = query.Include(d => d.Directories);

            if (options.HasFlag(GetDirectoryOptions.Files))
                if (options.HasFlag(GetDirectoryOptions.FileContents))
                    query = query.Include(d => d.Files).ThenInclude(f => f.Content);
                else
                    query = query.Include(d => d.Files);

            if (!options.HasFlag(GetDirectoryOptions.ExcludeTags))
                query = query.Include(d => d.Tags);

            var directory = await query.SingleOrDefaultAsync(filter);

            if (!options.HasFlag(GetDirectoryOptions.Recurse))
                return directory;

            var tasks = directory.Directories.Select(subDirectory => LoadDirectory(subDirectory, options)).ToList();

            await Task.WhenAll(tasks);

            return directory;
        }

        public virtual async Task LoadDirectory(Directory directory, GetDirectoryOptions options)
        {
            var temp = await GetDirectory(directory.Id, options);

            foreach (var file in temp.Files)
            {
                directory.Files.Add((File)file);
            }

            foreach (var subDirectory in temp.Directories)
            {
                directory.Directories.Add((Directory)subDirectory);
            }
        }

        public virtual async Task UpdateDirectory(IDirectory directory)
        {
            var temp = await Directories.SingleOrDefaultAsync(d => d.Id == directory.Id);

            if (temp == null)
                throw new Exception($"Directory: {directory.Id} not found.");

            temp.CopyFrom(directory);

            await SaveChangesAsync(true);
        }

        public virtual async Task DeleteDirectory(Guid id)
        {
            var temp = await Directories.SingleOrDefaultAsync(d => d.Id == id);

            if (temp == null)
                throw new Exception($"Directory: {id} not found.");

            Directories.Remove(temp);

            await SaveChangesAsync(true);
        }

        public virtual async Task DeleteDirectory(IDirectory directory)
        {
            await DeleteDirectory(directory.Id);
        }

        public virtual async Task<IFile> CreateFile(string path, byte[] data)
        {
            if (!GetParentPathAndName(path, out var parentPath, out var name))
                throw new Exception($"Could not get Parent Path and Name: {path}");

            return await CreateFile(parentPath, name, data);
        }

        public virtual async Task<IFile> CreateFile(string path, string name, byte[] data)
        {
            var directory = await GetDirectoryTracked(path);

            if (directory == null)
            {
                if (!FileSystemOptions.AutoCreateFileDirectories)
                    throw new Exception($"Path does not exist: {path}");

                directory = await CreateDirectory(path);
            }

            return await CreateFile(directory, name, data);
        }

        public virtual async Task<IFile> CreateFile(string path, Stream data)
        {
            if (!GetParentPathAndName(path, out var parentPath, out var name))
                throw new Exception($"Could not get Parent Path and Name: {path}");

            return await CreateFile(parentPath, name, data);
        }

        public virtual async Task<IFile> CreateFile(string path, string name, Stream data)
        {
            await using var memoryStream = new MemoryStream();
            data.Seek(0, SeekOrigin.Begin);
            data.Position = 0;
            await data.CopyToAsync(memoryStream);
            var bytes = memoryStream.ToArray();
            return await CreateFile(path, name, bytes);
        }

        public async Task<IFile> CreateFile(IDirectory directory, string name, byte[] data)
        {
            var file = new File(directory, name);

            file.Content = new Content(data);

            await Files.AddAsync(file);

            await SaveChangesAsync(true);

            return file;
        }

        public async Task<IFile> CreateFile(IDirectory directory, string name, Stream data)
        {
            await using var memoryStream = new MemoryStream();
            await data.CopyToAsync(memoryStream);
            var bytes = memoryStream.ToArray();
            return await CreateFile(directory, name, bytes);
        }

        public virtual async Task InsertFile(IFile file)
        {
            var temp = new File();
            temp.CopyFrom(file);
            await Files.AddAsync(temp);
            await SaveChangesAsync(true);
        }

        public virtual async Task<IFile> GetFile(Guid id)
        {
            return await GetFile(id, FileSystemOptions.DefaultGetFileOptions);
        }

        public virtual async Task<IFile> GetFile(Guid id, GetFileOptions options)
        {
            return await GetFile(file => file.Id == id, options);
        }

        public virtual async Task<IFile> GetFile(string path)
        {
            return await GetFile(path, FileSystemOptions.DefaultGetFileOptions);
        }

        public virtual async Task<IFile> GetFile(string path, GetFileOptions options)
        {
            return await GetFile(file => file.Path == path, options);
        }

        public virtual async Task<File> GetFile(Expression<Func<File, bool>> filter, GetFileOptions options)
        {
            var query = Files.AsNoTracking();

            if ((options.HasFlag(GetFileOptions.IncludeFileContents) || options.HasFlag(GetFileOptions.None)) && !options.HasFlag(GetFileOptions.IgnoreFileContents))
                query = query.Include(f => f.Content);

            if (!options.HasFlag(GetFileOptions.ExcludeTags))
                query = query.Include(d => d.Tags);

            return await query.SingleOrDefaultAsync(filter);
        }

        public virtual async Task UpdateFile(IFile file)
        {
            var temp = await Files.SingleOrDefaultAsync(f => f.Id == file.Id);

            if (temp == null)
                throw new Exception($"File: {file.Id} not found.");

            temp.CopyFrom(file);

            await SaveChangesAsync(true);
        }

        public virtual async Task DeleteFile(Guid id)
        {
            var temp = await Files.SingleOrDefaultAsync(d => d.Id == id);

            if (temp == null)
                throw new Exception($"File: {id} not found.");

            Files.Remove(temp);

            await SaveChangesAsync(true);
        }

        public virtual async Task DeleteFile(IFile file)
        {
            await DeleteFile(file.Id);
        }

        public virtual async Task<ITag> CreateTag(string path, string name)
        {
            var item = await FileSystemItems.AsNoTracking().SingleOrDefaultAsync(i => i.Path == path);

            if (item == null)
                throw new Exception($"Item could not be found: {path}");

            return await CreateTag(item, name, null);
        }

        public virtual async Task<ITag> CreateTag(string path, string name, string value)
        {
            var item = await FileSystemItems.AsNoTracking().SingleOrDefaultAsync(i => i.Path == path);

            if (item == null)
                throw new Exception($"Item could not be found: {path}");

            return await CreateTag(item, name, value);
        }

        public virtual async Task<ITag> CreateTag(IFileSystemItem item, string name)
        {
            return await CreateTag(item, name, null);
        }

        public virtual async Task<ITag> CreateTag(IFileSystemItem item, string name, string value)
        {
            var tag = new Tag(item, name, value);

            await Tags.AddAsync(tag);

            await SaveChangesAsync(true);

            return tag;
        }

        public virtual async Task<ITag> CreateTag(Guid id, string name)
        {
            var item = await FileSystemItems.AsNoTracking().SingleOrDefaultAsync(i => i.Id == id);

            if (item == null)
                throw new Exception($"Item could not be found: {id}");

            return await CreateTag(item, name, null);
        }

        public virtual async Task<ITag> CreateTag(Guid id, string name, string value)
        {
            var item = await FileSystemItems.AsNoTracking().SingleOrDefaultAsync(i => i.Id == id);

            if (item == null)
                throw new Exception($"Item could not be found: {id}");

            return await CreateTag(item, name, value);
        }

        public virtual async Task InsertTag(ITag tag)
        {
            var temp = new Tag();
            temp.ItemId = tag.ItemId;
            temp.Name = tag.Name;
            temp.Value = tag.Value;

            await Tags.AddAsync(temp);

            await SaveChangesAsync(true);
        }

        public virtual async Task<ITag> GetTag(Guid id)
        {
            return await Tags.AsNoTracking().SingleOrDefaultAsync(t => t.Id == id);
        }

        public virtual async Task<ITag> GetTag(string path, string name)
        {
            var item = await FileSystemItems.AsNoTracking().SingleOrDefaultAsync(i => i.Path == path);

            if (item == null)
                throw new Exception($"Item could not be found: {path}");

            return await GetTag(item.Id, name);
        }

        public virtual async Task<ITag> GetTag(Guid id, string name)
        {
            return await Tags.AsNoTracking().SingleOrDefaultAsync(t => t.ItemId == id && t.Name == name);
        }

        public virtual async Task<ITag> GetTag(IFileSystemItem item, string name)
        {
            return await GetTag(item.Id, name);
        }

        public virtual async Task UpdateTag(ITag tag)
        {
            var temp = await Tags.SingleOrDefaultAsync(t => t.Id == tag.Id);

            temp.ItemId = tag.ItemId;
            temp.Name = tag.Name;
            temp.Value = tag.Value;

            await SaveChangesAsync(true);
        }

        public virtual async Task DeleteTag(Guid id)
        {
            var temp = await Tags.SingleOrDefaultAsync(d => d.Id == id);

            if (temp == null)
                throw new Exception($"Tag: {id} not found.");

            Tags.Remove(temp);

            await SaveChangesAsync(true);
        }

        public virtual async Task DeleteTag(ITag tag)
        {
            await DeleteTag(tag.Id);
        }

        public virtual async Task RunMigrations()
        {
            await Database.MigrateAsync();
        }
    }
}
