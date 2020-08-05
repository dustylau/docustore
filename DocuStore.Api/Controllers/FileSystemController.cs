using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using DocuStore.Api.Models;
using DocuStore.Core.Interfaces;
using DocuStore.Core.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DocuStore.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FileSystemController : ControllerBase
    {

        private readonly ILogger<FileSystemController> _logger;
        private readonly IFileSystem _fileSystem;

        public FileSystemController(ILogger<FileSystemController> logger, IFileSystem fileSystem)
        {
            _logger = logger;
            _fileSystem = fileSystem;
        }


        [HttpGet("Items")]
        public async Task<ServiceResponse<IEnumerable<IFileSystemItem>>> GetItems([FromQuery] string filter, [FromQuery] string orderBy)
        {
            try
            {
                var items = _fileSystem.Items;

                if (!string.IsNullOrWhiteSpace(filter))
                {
                    filter = filter.Replace("'", "\"");
                    items = items.Where(filter);
                }

                if (!string.IsNullOrWhiteSpace(orderBy))
                    items = items.OrderBy(orderBy);

                return new ServiceResponse<IEnumerable<IFileSystemItem>>(items);
            }
            catch (Exception exception)
            {
                return new ServiceResponse<IEnumerable<IFileSystemItem>>(exception);
            }
        }


        [HttpPost("GetItems")]
        public async Task<ServiceResponse<IEnumerable<IFileSystemItem>>> GetItems(GetItemsRequest request)
        {
            try
            {
                var items = _fileSystem.Items;

                if (!string.IsNullOrWhiteSpace(request.Filter))
                {
                    request.Filter = request.Filter.Replace("'", "\"");
                    items = items.Where(request.Filter);
                }

                if (!string.IsNullOrWhiteSpace(request.OrderBy))
                    items = items.OrderBy(request.OrderBy);


                return new ServiceResponse<IEnumerable<IFileSystemItem>>(items);
            }
            catch (Exception exception)
            {
                return new ServiceResponse<IEnumerable<IFileSystemItem>>(exception);
            }
        }

        [HttpPost("GetDirectory")]
        public async Task<ServiceResponse<IDirectory>> GetDirectory(GetDirectoryRequest request)
        {
            try
            {
                var options = GetDirectoryOptions.None;

                if (request.ExcludeTags)
                    options |= GetDirectoryOptions.ExcludeTags;

                if (request.IncludeDirectories)
                    options |= GetDirectoryOptions.Directories;

                if (request.IncludeFiles)
                    options |= GetDirectoryOptions.Files;

                if (request.IncludeFileContents)
                    options |= GetDirectoryOptions.FileContents;

                if (request.Recurse)
                    options |= GetDirectoryOptions.Recurse;

                if (options > 0)
                    options &= ~GetDirectoryOptions.None;

                var directory = await _fileSystem.GetDirectory(request.Path, options);

                return new ServiceResponse<IDirectory>(directory);
            }
            catch (Exception exception)
            {
                _logger.LogWarning(exception, $"Could not get Directory: {request.Path}");
                return new ServiceResponse<IDirectory>(exception);
            }
        }

        [HttpPost("CreateDirectory")]
        public async Task<ServiceResponse<IDirectory>> CreateDirectory(CreateDirectoryRequest request)
        {
            try
            {
                var directory = await _fileSystem.CreateDirectory(request.Path);
                return new ServiceResponse<IDirectory>(directory);
            }
            catch (Exception exception)
            {
                _logger.LogWarning(exception, $"Could not create Directory: {request.Path}");
                return new ServiceResponse<IDirectory>(exception);
            }
        }

        [HttpPost("CreateFile/{path}")]
        public async Task<ServiceResponse<IFile>> CreateFile(IFormFile file, string path)
        {
            try
            {
                using (var fileStream = new MemoryStream())
                {
                    await file.CopyToAsync(fileStream);
                    var newFile = await _fileSystem.CreateFile(HttpUtility.UrlDecode(path), fileStream);
                    newFile.Content.Data = null;
                    return new ServiceResponse<IFile>(newFile);
                }
            }
            catch (Exception exception)
            {
                _logger.LogWarning(exception, $"Could not create File: {path}");
                return new ServiceResponse<IFile>(exception);
            }
        }

        [HttpPost("CreateTag")]
        public async Task<ServiceResponse<ITag>> CreateTag(CreateTagRequest request)
        {
            try
            {
                var item = await _fileSystem.GetFileSystemItem(i => i.Path == request.Path);

                if (item == null)
                    throw new Exception($"Path is not found: {request.Path}");

                var tag = await _fileSystem.CreateTag(request.Path, request.Name, request.Value);

                return new ServiceResponse<ITag>(tag);
            }
            catch (Exception exception)
            {
                _logger.LogWarning(exception, $"Could not create Tag: {request.Path} - {request.Name}");
                return new ServiceResponse<ITag>(exception);
            }
        }

        [HttpPost("GetFile")]
        public async Task<ServiceResponse<IFile>> GetFile(GetFileRequest request)
        {
            try
            {
                var options = (request.IncludeContent.HasValue && request.IncludeContent.Value)
                    ? GetFileOptions.IncludeFileContents
                    : GetFileOptions.IgnoreFileContents;

                if (request.ExcludeTags.HasValue && request.ExcludeTags.Value)
                    options |= GetFileOptions.ExcludeTags;

                var file = await _fileSystem.GetFile(request.Path, options);

                return new ServiceResponse<IFile>(file);
            }
            catch (Exception exception)
            {
                _logger.LogWarning(exception, $"Could not get file: {request.Path}");
                return new ServiceResponse<IFile>(exception);
            }
        }

        [HttpGet("File")]
        public async Task<IActionResult> Download(string path)
        {
            try
            {
                var file = await _fileSystem.GetFile(path, GetFileOptions.IncludeFileContents);

                var type = "text/plain";

                var extensionSeparatorIndex = file.Name.LastIndexOf(".");

                if (extensionSeparatorIndex >= 0)
                {
                    var extension = file.Name.Substring(extensionSeparatorIndex);
                    type = MimeTypeHelper.MimeTypes[extension];
                }

                var stream = new MemoryStream(file.Content.Data);

                stream.Seek(0, SeekOrigin.Begin);
                stream.Position = 0;

                return File(stream, type, file.Name);
            }
            catch (Exception exception)
            {
                _logger.LogWarning(exception, $"Could not get file: {path}");
                return Problem(exception.Message, null, (int) HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost("DeleteDirectory")]
        public async Task<ServiceResponse> DeleteDirectory(DeleteDirectoryRequest request)
        {
            try
            {
                var directory = await _fileSystem.GetDirectory(request.Path, GetDirectoryOptions.None);

                if (directory == null)
                    throw new Exception($"Path is not found: {request.Path}");

                await _fileSystem.DeleteDirectory(directory);

                return new ServiceResponse();
            }
            catch (Exception exception)
            {
                _logger.LogWarning(exception, $"Could not delete directory: {request.Path}");
                return new ServiceResponse<IFile>(exception);
            }
        }

        [HttpPost("DeleteFile")]
        public async Task<ServiceResponse> DeleteFile(DeleteFileRequest request)
        {
            try
            {
                var file = await _fileSystem.GetFile(request.Path, GetFileOptions.IgnoreFileContents);

                if (file == null)
                    throw new Exception($"Path is not found: {request.Path}");

                await _fileSystem.DeleteFile(file);

                return new ServiceResponse();
            }
            catch (Exception exception)
            {
                _logger.LogWarning(exception, $"Could not delete file: {request.Path}");
                return new ServiceResponse<IFile>(exception);
            }
        }

        [HttpPost("DeleteTag")]
        public async Task<ServiceResponse> DeleteTag(DeleteTagRequest request)
        {
            try
            {
                var tag = await _fileSystem.GetTag(request.Path, request.Name);

                if (tag == null)
                    throw new Exception($"Tag is not found: {request.Path} - {request.Name}");

                await _fileSystem.DeleteTag(tag);

                return new ServiceResponse();
            }
            catch (Exception exception)
            {
                _logger.LogWarning(exception, $"Could not delete tag: {request.Path} - {request.Name}");
                return new ServiceResponse<IFile>(exception);
            }
        }
    }
}

