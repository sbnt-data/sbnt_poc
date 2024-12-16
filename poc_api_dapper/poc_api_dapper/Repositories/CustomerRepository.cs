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

        public async Task<ReturnDapper<CustomerModelOutput>> GetCustomerDetails(string customerId)
        {
            _logger.LogInformation("Fetching customer details for CustomerID: {CustomerID}", customerId);

            try
            {
                string spName = StoredProcedures.GetCustomerDetails;

                var parameters = new DynamicParameters();
                parameters.Add("@CustomerID", customerId, DbType.String, ParameterDirection.Input);

                var result = await _dataAccess.QueryAsync<CustomerModelOutput>(spName, parameters);

                _logger.LogInformation("Stored procedure executed successfully. ReturnStatus: {ReturnStatus}, ErrorCode: {ErrorCode}",
                    result.ReturnStatus, result.ErrorCode);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching customer details for CustomerID: {CustomerID}", customerId);
                throw;
            }
        }
    }

    public static class StoredProcedures
    {
        public const string GetCustomerDetails = "[dbo].GetCustomerDetails";
    }
}
