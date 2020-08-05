using System.ComponentModel.DataAnnotations;

namespace DocuStore.Api.Models
{
    public class CreateTagRequest : ServiceRequest
    {
        [Required]
        public string Path { get; set; }

        [Required]
        public string Name { get; set; }

        public string Value { get; set; }
    }
}