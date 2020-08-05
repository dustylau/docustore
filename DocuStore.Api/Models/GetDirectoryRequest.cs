using System.ComponentModel.DataAnnotations;

namespace DocuStore.Api.Models
{
    public class GetDirectoryRequest : ServiceRequest
    {
        [Required]
        public string Path { get; set; }

        public bool IncludeDirectories { get; set; } = true;
        public bool IncludeFiles { get; set; }
        public bool IncludeFileContents { get; set; }
        public bool ExcludeTags { get; set; }
        public bool Recurse { get; set; }

    }
}