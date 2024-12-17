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

        public async Task<List<CustomerModelOutput>> GetCustomerDetails(string customerId)
        {
            _logger.LogInformation("Fetching customer details for CustomerID: {CustomerID}", customerId);

            try
            {
                string spName = StoredProcedures.GetCustomerDetails;

                var parameters = new DynamicParameters();
                parameters.Add("@CustomerID", customerId, DbType.String, ParameterDirection.Input);

                // Call the updated QueryAsync method
                var (resultList, returnStatus, errorCode) = await _dataAccess.QueryAsync<CustomerModelOutput>(spName, parameters);

                _logger.LogInformation("Stored procedure executed successfully. ReturnStatus: {ReturnStatus}, ErrorCode: {ErrorCode}",
                    returnStatus, errorCode);

                // Handle error scenarios based on the output parameters
                if (errorCode != "ERR200")
                {
                    _logger.LogError("Stored procedure failed. ReturnStatus: {ReturnStatus}, ErrorCode: {ErrorCode}", returnStatus, errorCode);
                    throw new Exception($"Stored procedure failed: {returnStatus} (Code: {errorCode})");
                }

                // Return the result list
                return resultList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching customer details for CustomerID: {CustomerID}", customerId);
                throw;
            }
        }

        public async Task<List<JobOrderCbmModelOutput>> GetJobOrderCbmByJobOrderId(decimal jobOrderId, decimal clientId)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@JobOrderId", jobOrderId);
            parameters.Add("@ClientId", clientId);

            // Call QueryMultipleAsync
            var (joCbmList, jo2TList, returnStatus, errorCode) =
                await _dataAccess.QueryMultipleAsync<JobOrderCbmModelOutput, JobOrderCbmJob2triggerModel>(
                    "PM.usp_JobOrderCbmWithJobOrderIdPopulate", parameters);

            // Handle potential return status or errors
            if (errorCode != "ERR200")
            {
                _logger.LogError("Stored procedure failed with ErrorCode: {ErrorCode}, ReturnStatus: {ReturnStatus}", errorCode, returnStatus);
                throw new Exception($"Stored procedure failed: {returnStatus} (Code: {errorCode})");
            }

            // Map result sets
            if (joCbmList.Any() && jo2TList.Any())
            {
                var jo2TLookup = jo2TList.GroupBy(j => j.JobPlanCbmId).ToDictionary(g => g.Key, g => g.ToList());

                foreach (var cbm in joCbmList)
                {
                    cbm.JobOrderCbmJob2trigger = jo2TLookup.TryGetValue(cbm.JobPlanCbmId, out var triggers)
                        ? triggers
                        : new List<JobOrderCbmJob2triggerModel>();
                }
            }

            return joCbmList;
        }

    }

    public static class StoredProcedures
    {
        public const string GetCustomerDetails = "[dbo].GetCustomerDetails";
    }
}
