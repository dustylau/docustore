using System;
using System.IO;
using System.Linq;
using System.Text;
using CommonServiceLocator;
using DocStore.Repository.Sql;
using DocuStore.Core.Interfaces;
using DocuStore.IOC;
using Lamar;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace DocuStore.Tests.Repository
{
    [TestFixture]
    public class DirectFileSystemRepositoryTests
    {
        private FileSystem _repository;
        private FileSystem.Directory _rootDirectory;
        private FileSystem.Directory _subDirectory;

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
            _repository = ServiceLocator.Current.GetInstance<FileSystem>();

            Assert.NotNull(_repository);
        }

        [Test]
        [Order(3)]
        public void CanCreateRootDirectory()
        {
            _repository.Directories.Add(new FileSystem.Directory()
            {
                Name = "",
                Path = "/"
            });

            _repository.SaveChanges(true);
        }

        [Test]
        [Order(4)]
        public void CanGetRootDirectory()
        {
            _rootDirectory = _repository.Directories.SingleOrDefault(d => d.Path == "/");

            Assert.NotNull(_rootDirectory);
        }

        [Test]
        [Order(5)]
        public void CanTagRootDirectory()
        {
            _rootDirectory.Tags.Add(new FileSystem.Tag(_rootDirectory, "SystemDirectory", "Root"));
            _repository.SaveChanges(true);
        }

        [Test]
        [Order(6)]
        public void CanCreateSubDirectory()
        {
            try
            {
                var subDirectory = new FileSystem.Directory(_rootDirectory, "Files");
                _rootDirectory.Directories.Add(subDirectory);
                _repository.SaveChanges(true);
            }
            catch (Exception exception)
            {
                var message = GetMessage(exception);
                Assert.Fail(message);
            }
        }

        [Test]
        [Order(7)]
        public void CanGetSubDirectory()
        {
            _subDirectory = _repository.Directories.SingleOrDefault(d => d.Path == "/Files");
            Assert.NotNull(_subDirectory);
        }

        [Test]
        [Order(8)]
        public void CanCreateFile()
        {
            try
            {
                var file = new FileSystem.File(_subDirectory, "Test.txt");

                byte[] data = System.Text.Encoding.UTF8.GetBytes("A test file.");
                file.Content = new FileSystem.Content(data);

                _subDirectory.Files.Add(file);
                _repository.SaveChanges(true);
            }
            catch (Exception exception)
            {
                var message = GetMessage(exception);
                Assert.Fail(message);
            }
        }

        [Test]
        [Order(9)]
        public void CanGetFile()
        {
            var file = _repository.Files.AsNoTracking().Include(f => f.Content).SingleOrDefault(f => f.Path == "/Files/Test.txt");

            AssertValidTextFile(file, "/Files/Test.txt", "Test.txt", "A test file.");
        }

        [Test]
        [Order(10)]
        public void CanGetDirectoryWithContents()
        {
            var directory = _repository.Directories.AsNoTracking()
                .Include(f => f.Files)
                .ThenInclude(f => f.Content)
                .SingleOrDefault(f => f.Path == "/Files");

            AssertValidDirectory(directory, "/Files", "Files");

            var file = directory.Files.Single();

            AssertValidTextFile(file, "/Files/Test.txt", "Test.txt", "A test file.");
        }

        private static void AssertValidDirectory(FileSystem.Directory directory, string path, string name)
        {
            Assert.NotNull(directory);
            Assert.AreEqual(path, directory.Path);
            Assert.AreEqual(name, directory.Name);
        }

        private static void AssertValidTextFile(FileSystem.File file, string path, string name, string content)
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