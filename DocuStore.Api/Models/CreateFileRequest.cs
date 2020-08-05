using System.ComponentModel.DataAnnotations;

namespace DocuStore.Api.Models
{
    public class CreateFileRequest : ServiceRequest
    {
        [Required]
        public string Path { get; set; }
    }
}