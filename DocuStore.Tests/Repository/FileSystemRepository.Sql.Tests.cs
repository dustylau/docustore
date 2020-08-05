using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using CommonServiceLocator;
using DocStore.Repository.Sql;
using DocuStore.Core.Interfaces;
using DocuStore.Core.Shared;
using DocuStore.IOC;
using Lamar;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace DocuStore.Tests.Repository
{
    [TestFixture]
    public class FileSystemRepositoryTests
    {
        private IFileSystem _repository;
        private IDirectory _rootDirectory;
        private IDirectory _subDirectory;

        [OneTimeSetUp]
        public void SetUp()
        {
            if (File.Exists("FileSystem.db"))
                File.Delete("FileSystem.db");

            Bootstrapper.BootstrapAndSetLocatorProvider(config =>
            {
                //config.For<IFileSystem>().Use<FileSystem>().Singleton();
            });
        }

        [Test]
        [Order(1)]
        public void CanCreateDatabase()
        {
            var repository = ServiceLocator.Current.GetInstance<FileSystem>();

            repository.Database.Migrate();
        }

        [Test]
        [Order(2)]
        public void CanCreateRepository()
        {
            _repository = ServiceLocator.Current.GetInstance<IFileSystem>();

            Assert.NotNull(_repository);
        }

        [Test]
        [Order(3)]
        public async Task CanCreateRootDirectory()
        {
            IDirectory directory = new FileSystem.Directory()
            {
                Name = "",
                Path = $"{FileSystemOptions.DirectorySeparator}"
            };

            await _repository.InsertDirectory(directory);
        }

        [Test]
        [Order(4)]
        public async Task CanGetRootDirectory()
        {
            _rootDirectory = await _repository.GetDirectory($"{FileSystemOptions.DirectorySeparator}");

            Assert.NotNull(_rootDirectory);
        }

        [Test]
        [Order(5)]
        public async Task CanTagRootDirectory()
        {
            var tag = await _repository.CreateTag(_rootDirectory, "SystemDirectory", "Root");
            Assert.NotNull(tag);
        }

        [Test]
        [Order(6)]
        public async Task RootDirectoryHasTag()
        {
            var directory = await _repository.GetDirectory($"{FileSystemOptions.DirectorySeparator}");
            Assert.True(directory.Tags.Any(tag => tag.Name == "SystemDirectory" && tag.Value == "Root"));
        }

        [Test]
        [Order(6)]
        public async Task CanCreateSubDirectory()
        {
            try
            {
                var subDirectory = await _repository.CreateDirectory("/Files");
                Assert.NotNull(subDirectory);
            }
            catch (Exception exception)
            {
                var message = GetMessage(exception);
                Assert.Fail(message);
            }
        }

        [Test]
        [Order(7)]
        public async Task CanGetSubDirectory()
        {
            _subDirectory = await _repository.GetDirectory("/Files");
            Assert.NotNull(_subDirectory);
        }

        [Test]
        [Order(8)]
        public async Task CanCreateFile()
        {
            try
            {
                byte[] data = System.Text.Encoding.UTF8.GetBytes("A test file.");
                var file = await _repository.CreateFile("/Files/Test.txt", data);
            }
            catch (Exception exception)
            {
                var message = GetMessage(exception);
                Assert.Fail(message);
            }
        }

        [Test]
        [Order(9)]
        public async Task CanGetFile()
        {
            var file = await _repository.GetFile("/Files/Test.txt");

            AssertValidTextFile(file, "/Files/Test.txt", "Test.txt", "A test file.");
        }

        [Test]
        [Order(10)]
        public async Task CanGetDirectoryWithContents()
        {
            var directory = await _repository.GetDirectory(
                "/Files", 
                GetDirectoryOptions.Directories 
                | GetDirectoryOptions.Files 
                | GetDirectoryOptions.FileContents 
                | GetDirectoryOptions.Recurse
                );

            AssertValidDirectory(directory, "/Files", "Files");

            var file = directory.Files.Single();

            AssertValidTextFile(file, "/Files/Test.txt", "Test.txt", "A test file.");
        }

        [Test]
        [Order(11)]
        public async Task CanCreateFileInNonExistingDirectory()
        {
            var path = "/NewDirectory/ANewSubDirectory/Test2.txt";
            var name = "Test2.txt";
            var content = "Another test file with data and stuff.\nAnd new lines.";

            byte[] data = System.Text.Encoding.UTF8.GetBytes(content);
            var file = await _repository.CreateFile(path, data);

            AssertValidTextFile(file, path, name, content);
        }

        [Test]
        [Order(12)]
        public async Task CanGetSubDirectoryWithContents()
        {
            var path = "/NewDirectory/ANewSubDirectory/Test2.txt";
            var name = "Test2.txt";
            var content = "Another test file with data and stuff.\nAnd new lines.";

            var directory = await _repository.GetDirectory(
                "/NewDirectory",
                GetDirectoryOptions.Directories
                | GetDirectoryOptions.Files
                | GetDirectoryOptions.FileContents
                | GetDirectoryOptions.Recurse
            );

            AssertValidDirectory(directory, "/NewDirectory", "NewDirectory");

            var subDirectory = directory.Directories.Single();

            AssertValidDirectory(subDirectory, "/NewDirectory/ANewSubDirectory", "ANewSubDirectory");

            var file = subDirectory.Files.Single();

            AssertValidTextFile(file, path, name, content);
        }

        private static void AssertValidDirectory(IDirectory directory, string path, string name)
        {
            Assert.NotNull(directory);
            Assert.AreEqual(path, directory.Path);
            Assert.AreEqual(name, directory.Name);
        }

        private static void AssertValidTextFile(IFile file, string path, string name, string content)
        {
            Assert.NotNull(file);
            Assert.AreEqual(path, file.Path);
            Assert.AreEqual(name, file.Name);
            Assert.NotNull(file.Content);
            Assert.NotNull(file.Content.Data);
            Assert.AreEqual(content, Encoding.UTF8.GetString(file.Content.Data));
        }

        private string GetMessage(Exception exception, string message = null, int level = 0)
        {
            var indent = string.Empty;

            for (var count = 0; count < level; count++) indent += " ";

            if (string.IsNullOrWhiteSpace(message))
                message = exception.Message;
            else
                message += $"{Environment.NewLine}{indent}{exception.Message}";

            return exception.InnerException == null
                ? message
                : GetMessage(exception.InnerException, message, level + 1);
        }
    }
}
