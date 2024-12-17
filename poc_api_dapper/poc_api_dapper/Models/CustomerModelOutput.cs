namespace poc_api_dapper.Models
{
    public class CustomerModelOutput
    {
        public string CustomerID { get; set; }      // Primary key
        public string CompanyName { get; set; }     // Company Name
        public string ContactName { get; set; }     // Contact Name
        public string Country { get; set; }         // Country of the customer
    }
}
