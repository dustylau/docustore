using System.ComponentModel.DataAnnotations;

namespace DocuStore.Api.Models
{
    public class GetFileRequest : ServiceRequest
    {
        [Required]
        public string Path { get; set; }
        public bool? ExcludeTags { get; set; }
        public bool? IncludeContent { get; set; }
    }
}