using System.ComponentModel.DataAnnotations;

namespace DocuStore.Api.Models
{
    public class DeleteFileRequest : ServiceRequest
    {
        [Required]
        public string Path { get; set; }
    }
}