using poc_api_dapper.Models;

namespace poc_api_dapper.Repositories
{
    public interface ICustomerRepository
    {
        Task<List<CustomerModelOutput>> GetCustomerDetails(string customerId);
    }
}