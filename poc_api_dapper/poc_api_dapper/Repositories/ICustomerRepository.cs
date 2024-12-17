using poc_api_dapper.Models;

namespace poc_api_dapper.Repositories
{
    public interface ICustomerRepository
    {
        Task<int> GetCustomerCountByCountryAsync(string country);
        Task<List<CustomerModelOutput>> GetCustomerDetailsAsync(string customerId);
        Task<(List<CustomerModelOutput>, List<OrderModel>)> GetCustomerWithOrdersAsync(string customerId);
        Task<int> UpdateCustomerNameAsync(string customerId, string newName);
        Task<(List<CustomerModelOutput>, List<OrderModel>, List<EmployeeModel>)> GetCustomerOrdersAndEmployeesAsync(string customerId);
    }
}