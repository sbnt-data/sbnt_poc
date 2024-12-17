namespace poc_api_dapper.Models
{
    public class OrderModel
    {
        public int OrderID { get; set; }           // Primary key of Orders table
        public DateTime? OrderDate { get; set; }   // Date when the order was placed
        public DateTime? RequiredDate { get; set; } // Date when the order is required
        public DateTime? ShippedDate { get; set; }  // Date when the order was shipped
        public decimal? Freight { get; set; }       // Freight cost
    }
}
