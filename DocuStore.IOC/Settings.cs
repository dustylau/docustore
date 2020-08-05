using DocuStore.Core.Shared;
using DocuStore.Shared;

namespace DocuStore.IOC
{
    public class Settings : ISettings
    {
        public Settings()
        {
            FileSystemRepositoryConnectionString = ConfigHelper.ConnectionString("FileSystemRepositoryConnectionString", "Data Source = FileSystem.db");
        }

        public string FileSystemRepositoryConnectionString { get; }
    }
}