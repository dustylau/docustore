using System.ComponentModel.DataAnnotations;

namespace DocuStore.Api.Models
{
    public class DeleteTagRequest : ServiceRequest
    {
        [Required]
        public string Path { get; set; }

        [Required]
        public string Name { get; set; }
    }
}