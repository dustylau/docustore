using System.ComponentModel.DataAnnotations;

namespace DocuStore.Api.Models
{
    public class CreateDirectoryRequest : ServiceRequest
    {
        [Required]
        public string Path { get; set; }
    }
}