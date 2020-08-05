using System.ComponentModel.DataAnnotations;

namespace DocuStore.Api.Models
{
    public class DeleteDirectoryRequest : ServiceRequest
    {
        [Required]
        public string Path { get; set; }
    }
}