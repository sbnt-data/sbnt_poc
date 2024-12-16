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

        public async Task<ReturnDapper<T>> QueryAsync<T>(string spName, DynamicParameters parameters)
        {
            using var connection = _context.CreateConnection();

            var result = new ReturnDapper<T>();
            try
            {
                AddStandardOutputParameters(parameters);

                _logger.LogInformation("Executing stored procedure {StoredProcedure}", spName);

                // Execute the stored procedure and fetch data
                result.ListResult = (await connection.QueryAsync<T>(
                    spName,
                    parameters,
                    commandType: CommandType.StoredProcedure)).ToList();

                // Extract output parameters
                result.ReturnStatus = parameters.Get<string>(StoredProcedureParameters.ReturnStatus);
                result.ErrorCode = parameters.Get<string>(StoredProcedureParameters.ErrorCode);

                _logger.LogInformation("Stored procedure {StoredProcedure} executed successfully. ReturnStatus: {ReturnStatus}, ErrorCode: {ErrorCode}",
                    spName, result.ReturnStatus, result.ErrorCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing stored procedure {StoredProcedure}", spName);
                var parameterDetails = string.Join(", ", parameters.ParameterNames.Select(name => $"{name}={parameters.Get<object>(name)}"));
                throw new InvalidOperationException($"Error executing stored procedure '{spName}'. Parameters: {parameterDetails}", ex);
            }

            return result;
        }

        private void AddStandardOutputParameters(DynamicParameters parameters)
        {
            parameters.Add(StoredProcedureParameters.ReturnStatus, dbType: DbType.String, size: 50, direction: ParameterDirection.Output);
            parameters.Add(StoredProcedureParameters.ErrorCode, dbType: DbType.String, size: 10, direction: ParameterDirection.Output);
        }
    }

    public class ReturnDapper<T>
    {
        public IEnumerable<T> ListResult { get; set; } = new List<T>();
        public string ReturnStatus { get; set; }
        public string ErrorCode { get; set; }

        public bool IsSuccess => ErrorCode == "ERR200";

        public override string ToString() => $"Status: {ReturnStatus}, ErrorCode: {ErrorCode}";
    }

    public static class StoredProcedureParameters
    {
        public const string ReturnStatus = "@ReturnStatus";
        public const string ErrorCode = "@ErrorCode";
    }
}
