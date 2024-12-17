using Dapper;
using poc_api_dapper.Context;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace poc_api_dapper.DataAccessLayer
{
    public class DataAccess
    {
        private readonly DapperContext _context;
        private readonly ILogger<DataAccess> _logger;

        public DataAccess(DapperContext context, ILogger<DataAccess> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<(List<T>, string ReturnStatus, string ErrorCode)> QueryAsync<T>(string spName, DynamicParameters parameters)
        {
            using var connection = _context.CreateConnection();

            try
            {
                // Add standard output parameters
                AddStandardOutputParameters(parameters);

                _logger.LogInformation("Executing stored procedure {StoredProcedure}", spName);

                // Execute the stored procedure and fetch data
                var resultList = (await connection.QueryAsync<T>(
                    spName,
                    parameters,
                    commandType: CommandType.StoredProcedure)).ToList();

                // Extract output parameters
                var returnStatus = parameters.Get<string>(StoredProcedureParameters.ReturnStatus);
                var errorCode = parameters.Get<string>(StoredProcedureParameters.ErrorCode);

                _logger.LogInformation("Stored procedure {StoredProcedure} executed successfully. ReturnStatus: {ReturnStatus}, ErrorCode: {ErrorCode}",
                    spName, returnStatus, errorCode);

                // Return the result list and output parameters
                return (resultList, returnStatus, errorCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing stored procedure {StoredProcedure}", spName);
                var parameterDetails = string.Join(", ", parameters.ParameterNames.Select(name => $"{name}={parameters.Get<object>(name)}"));
                throw new InvalidOperationException($"Error executing stored procedure '{spName}'. Parameters: {parameterDetails}", ex);
            }
        }

        public async Task<(List<T1>, List<T2>, string ReturnStatus, string ErrorCode)> QueryMultipleAsync<T1, T2>(
    string spName, DynamicParameters parameters)
        {
            using var connection = _context.CreateConnection();

            List<T1> resultSet1;
            List<T2> resultSet2;
            string returnStatus = string.Empty;
            string errorCode = string.Empty;

            try
            {
                // Add standard output parameters
                AddStandardOutputParameters(parameters);

                _logger.LogInformation("Executing stored procedure {StoredProcedure}", spName);

                // Execute the stored procedure and fetch multiple result sets
                using var gridReader = await connection.QueryMultipleAsync(
                    spName,
                    parameters,
                    commandType: CommandType.StoredProcedure);

                // Read the first and second result sets
                resultSet1 = (await gridReader.ReadAsync<T1>()).ToList();
                resultSet2 = (await gridReader.ReadAsync<T2>()).ToList();

                // Extract output parameters
                returnStatus = parameters.Get<string>(StoredProcedureParameters.ReturnStatus);
                errorCode = parameters.Get<string>(StoredProcedureParameters.ErrorCode);

                _logger.LogInformation("Stored procedure {StoredProcedure} executed successfully. ReturnStatus: {ReturnStatus}, ErrorCode: {ErrorCode}",
                    spName, returnStatus, errorCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing stored procedure {StoredProcedure}", spName);
                var parameterDetails = string.Join(", ", parameters.ParameterNames.Select(name => $"{name}={parameters.Get<object>(name)}"));
                throw new InvalidOperationException($"Error executing stored procedure '{spName}'. Parameters: {parameterDetails}", ex);
            }

            return (resultSet1, resultSet2, returnStatus, errorCode);
        }


        private static void AddStandardOutputParameters(DynamicParameters parameters)
        {
            parameters.Add(StoredProcedureParameters.ReturnStatus, dbType: DbType.String, size: 50, direction: ParameterDirection.Output);
            parameters.Add(StoredProcedureParameters.ErrorCode, dbType: DbType.String, size: 10, direction: ParameterDirection.Output);
        }
    }

    public static class StoredProcedureParameters
    {
        public const string ReturnStatus = "@ReturnStatus";
        public const string ErrorCode = "@ErrorCode";
    }
}
