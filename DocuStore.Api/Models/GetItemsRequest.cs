namespace DocuStore.Api.Models
{
    public class GetItemsRequest : ServiceRequest
    {
        public string Filter { get; set; }
        public string OrderBy { get; set; }
    }
}