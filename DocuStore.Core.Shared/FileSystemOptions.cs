namespace DocuStore.Core.Shared
{
    public class FileSystemOptions
    {
        public static string DirectorySeparator { get; set; } = "/";
        public static GetDirectoryOptions DefaultGetDirectoryOptions { get; set; } = GetDirectoryOptions.None;
        public static bool AutoCreateFileDirectories { get; set; } = true;
        public static GetFileOptions DefaultGetFileOptions { get; set; } = GetFileOptions.None;
    }
}