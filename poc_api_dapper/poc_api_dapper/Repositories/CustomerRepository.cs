using Dapper;
using Microsoft.Extensions.Logging;
using poc_api_dapper.DataAccessLayer;
using poc_api_dapper.Models;
using System.Data;

namespace poc_api_dapper.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly DataAccess _dataAccess;
        private readonly ILogger<CustomerRepository> _logger;

        public CustomerRepository(DataAccess dataAccess, ILogger<CustomerRepository> logger)
        {
            _dataAccess = dataAccess;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves customer details by ID.
        /// </summary>
        public async Task<List<CustomerModelOutput>> GetCustomerDetailsAsync(string customerId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@CustomerID", customerId, DbType.String);

            var result = await _dataAccess.QueryAsync<CustomerModelOutput>(
                StoredProcedures.GetCustomerDetails, parameters);

            if (!result.IsSuccess)
                throw new DataException($"Failed to fetch customer details: {result.ReturnStatus}");

            return result.ResultSet;
        }

        /// <summary>
        /// Updates a customer’s name.
        /// </summary>
        public async Task<int> UpdateCustomerNameAsync(string customerId, string newName)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@CustomerID", customerId, DbType.String);
            parameters.Add("@NewName", newName, DbType.String);

            var result = await _dataAccess.ExecuteAsync(StoredProcedures.UpdateCustomerName, parameters);

            if (!result.IsSuccess)
                throw new DataException($"Failed to update customer: {result.ReturnStatus}");

            return result.ResultSet.FirstOrDefault(); // Rows affected
        }

        /// <summary>
        /// Retrieves customer count for a given country.
        /// </summary>
        public async Task<int> GetCustomerCountByCountryAsync(string country)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Country", country, DbType.String);

            var result = await _dataAccess.ExecuteScalarAsync<int>(
                StoredProcedures.GetCustomerCountByCountry, parameters);

            if (!result.IsSuccess)
                throw new DataException($"Failed to get customer count: {result.ReturnStatus}");

            return result.ResultSet.FirstOrDefault(); // Scalar result
        }

        /// <summary>
        /// Fetches multiple result sets (customers and their orders).
        /// </summary>
        public async Task<(List<CustomerModelOutput>, List<OrderModel>)> GetCustomerWithOrdersAsync(string customerId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@CustomerID", customerId, DbType.String);

            var result = await _dataAccess.QueryMultipleAsync(StoredProcedures.GetCustomerWithOrders, parameters);

            if (!result.IsSuccess)
                throw new DataException($"Failed to fetch data: {result.ReturnStatus}");

            var customers = MapToModel<CustomerModelOutput>(result.CombinedResultSets[0]);
            var orders = MapToModel<OrderModel>(result.CombinedResultSets[1]);

            return (customers, orders);
        }

        /// <summary>
        /// Fetches multiple result sets (customers and their orders and employees).
        /// </summary>
        public async Task<(List<CustomerModelOutput>, List<OrderModel>, List<EmployeeModel>)> GetCustomerOrdersAndEmployeesAsync(string customerId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@CustomerID", customerId, DbType.String);

            var result = await _dataAccess.QueryMultipleAsync(StoredProcedures.GetCustomerOrdersAndEmployees, parameters);

            if (!result.IsSuccess)
                throw new DataException($"Failed to fetch data: {result.ReturnStatus}");

            // Read 3 result sets using CombinedResultSets
            var customers = MapToModel<CustomerModelOutput>(result.CombinedResultSets[0]);
            var orders = MapToModel<OrderModel>(result.CombinedResultSets[1]);
            var employees = MapToModel<EmployeeModel>(result.CombinedResultSets[2]);

            return (customers, orders, employees);
        }

        private static List<T> MapToModel<T>(IEnumerable<dynamic> resultSet)
        {
            return resultSet.Select(row =>
            {
                var model = Activator.CreateInstance<T>();
                var properties = typeof(T).GetProperties();
                var rowDictionary = (IDictionary<string, object>)row;

                properties
                    .Where(property => rowDictionary.ContainsKey(property.Name))
                    .ToList()
                    .ForEach(property =>
                    {
                        var value = rowDictionary[property.Name];
                        var targetType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

                        // Set value considering nullable and non-nullable types
                        property.SetValue(model, value == null ? null : Convert.ChangeType(value, targetType));
                    });

                return model;
            }).ToList();
        }
    }

    public static class StoredProcedures
    {
        public const string GetCustomerDetails = "[dbo].GetCustomerDetails";
        public const string UpdateCustomerName = "[dbo].UpdateCustomerName";
        public const string GetCustomerCountByCountry = "[dbo].GetCustomerCountByCountry";
        public const string GetCustomerWithOrders = "[dbo].GetCustomerWithOrders";
        public const string GetCustomerOrdersAndEmployees = "[dbo].[GetCustomerOrdersAndEmployees]";
    }
}
