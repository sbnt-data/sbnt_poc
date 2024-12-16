using poc_api_dapper.DataAccessLayer;
using poc_api_dapper.Models;

namespace poc_api_dapper.Repositories
{
    /// <summary>
    /// Interface for Customer repository operations.
    /// </summary>
    public interface ICustomerRepository
    {
        /// <summary>
        /// Fetches customer details based on the provided Customer ID.
        /// </summary>
        /// <param name="customerId">The unique identifier for the customer.</param>
        /// <returns>A <see cref="ReturnDapper{CustomerModelOutput}"/> containing the customer details and execution metadata.</returns>
        Task<ReturnDapper<CustomerModelOutput>> GetCustomerDetails(string customerId);
    }
}