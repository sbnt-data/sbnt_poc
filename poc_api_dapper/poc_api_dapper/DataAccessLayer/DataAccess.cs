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

        public async Task<DbExecutionResult<int>> ExecuteAsync(string spName, DynamicParameters parameters)
        {
            using var connection = _context.CreateConnection();

            var result = new DbExecutionResult<int>();

            try
            {
                // Add standard output parameters
                AddStandardOutputParameters(parameters);

                _logger.LogInformation("Executing stored procedure {StoredProcedure}.", spName);

                // Execute the stored procedure and get affected rows count
                var affectedRows = await connection.ExecuteAsync(
                    spName, parameters, commandType: CommandType.StoredProcedure);

                // Populate the ExecutionResult
                result.ResultSet.Add(affectedRows); // Store affected rows as a single integer result
                result.ReturnStatus = parameters.Get<string>(StoredProcedureParameters.ReturnStatus);
                result.ErrorCode = parameters.Get<string>(StoredProcedureParameters.ErrorCode);

                // Log success or error
                if (result.IsSuccess)
                {
                    _logger.LogInformation("Stored procedure {StoredProcedure} executed successfully. Affected Rows: {AffectedRows}, ReturnStatus: {ReturnStatus}, ErrorCode: {ErrorCode}",
                        spName, affectedRows, result.ReturnStatus, result.ErrorCode);
                }
                else
                {
                    _logger.LogWarning("Stored procedure {StoredProcedure} returned an error. ReturnStatus: {ReturnStatus}, ErrorCode: {ErrorCode}",
                        spName, result.ReturnStatus, result.ErrorCode);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing stored procedure {StoredProcedure}.", spName);
                result.ReturnStatus = "error";
                result.ErrorCode = ex.Message;
            }

            return result;
        }

        public async Task<DbExecutionResult<int>> ExecuteAsyncTrans(string spName, DynamicParameters parameters)
        {
            using var connection = _context.CreateConnection(); // Safely create a new connection
            using var transaction = connection.BeginTransaction(); // Begin a transaction

            var result = new DbExecutionResult<int>();

            try
            {
                // Add standard output parameters
                AddStandardOutputParameters(parameters);

                _logger.LogInformation("Executing stored procedure {StoredProcedure} within a transaction.", spName);

                // Execute the stored procedure within the transaction
                var affectedRows = await connection.ExecuteAsync(
                    spName, parameters, transaction: transaction, commandType: CommandType.StoredProcedure);

                // Extract output parameters
                result.ReturnStatus = parameters.Get<string>(StoredProcedureParameters.ReturnStatus);
                result.ErrorCode = parameters.Get<string>(StoredProcedureParameters.ErrorCode);
                result.ResultSet.Add(affectedRows);

                // Commit or rollback based on ReturnStatus
                if (result.ReturnStatus?.ToLower() == "success")
                {
                    transaction.Commit();
                    _logger.LogInformation("Transaction committed successfully for {StoredProcedure}. Affected Rows: {AffectedRows}", spName, affectedRows);
                }
                else
                {
                    transaction.Rollback();
                    _logger.LogWarning("Transaction rolled back due to error in {StoredProcedure}. ReturnStatus: {ReturnStatus}, ErrorCode: {ErrorCode}",
                        spName, result.ReturnStatus, result.ErrorCode);
                }
            }
            catch (Exception ex)
            {
                // Rollback the transaction in case of exception
                transaction.Rollback();
                _logger.LogError(ex, "Exception occurred while executing {StoredProcedure} within a transaction. Transaction rolled back.", spName);

                result.ReturnStatus = "error";
                result.ErrorCode = ex.Message;
            }

            return result;
        }

        public async Task<DbExecutionResult<T>> ExecuteScalarAsync<T>(string spName, DynamicParameters parameters)
        {
            using var connection = _context.CreateConnection();
            var result = new DbExecutionResult<T>();

            try
            {
                AddStandardOutputParameters(parameters);

                _logger.LogInformation("Executing stored procedure {StoredProcedure} for scalar result", spName);

                var scalarValue = await connection.ExecuteScalarAsync<T>(
                    spName, parameters, commandType: CommandType.StoredProcedure);

                result.ResultSet.Add(scalarValue);
                result.ReturnStatus = parameters.Get<string>(StoredProcedureParameters.ReturnStatus);
                result.ErrorCode = parameters.Get<string>(StoredProcedureParameters.ErrorCode);

                _logger.LogInformation("Scalar stored procedure executed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing scalar stored procedure {StoredProcedure}", spName);
                throw;
            }

            return result;
        }

        public async Task<DbExecutionResult<T>> QueryAsync<T>(string spName, DynamicParameters parameters)
        {
            using var connection = _context.CreateConnection();
            var result = new DbExecutionResult<T>();

            try
            {
                AddStandardOutputParameters(parameters);

                _logger.LogInformation("Executing stored procedure {StoredProcedure}", spName);

                result.ResultSet = (await connection.QueryAsync<T>(
                    spName, parameters, commandType: CommandType.StoredProcedure)).ToList();

                result.ReturnStatus = parameters.Get<string>(StoredProcedureParameters.ReturnStatus);
                result.ErrorCode = parameters.Get<string>(StoredProcedureParameters.ErrorCode);

                _logger.LogInformation("Stored procedure executed successfully. ReturnStatus: {ReturnStatus}, ErrorCode: {ErrorCode}",
                    result.ReturnStatus, result.ErrorCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing stored procedure {StoredProcedure}", spName);
                throw;
            }

            return result;
        }

        public async Task<DbExecutionResult<dynamic>> QueryMultipleAsync(string spName, DynamicParameters parameters)
        {
            using var connection = _context.CreateConnection();
            var result = new DbExecutionResult<dynamic>();

            try
            {
                AddStandardOutputParameters(parameters);

                _logger.LogInformation("Executing stored procedure {StoredProcedure}", spName);

                using var gridReader = await connection.QueryMultipleAsync(spName, parameters, commandType: CommandType.StoredProcedure);

                while (!gridReader.IsConsumed)
                {
                    var resultSet = await gridReader.ReadAsync<dynamic>();
                    result.CombinedResultSets.Add(resultSet);
                }

                result.ReturnStatus = parameters.Get<string>(StoredProcedureParameters.ReturnStatus);
                result.ErrorCode = parameters.Get<string>(StoredProcedureParameters.ErrorCode);

                _logger.LogInformation("Stored procedure executed successfully. ReturnStatus: {ReturnStatus}, ErrorCode: {ErrorCode}",
                    result.ReturnStatus, result.ErrorCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing stored procedure {StoredProcedure}", spName);
                throw;
            }

            return result;
        }

        public async Task<DbExecutionResult<T>> QueryAsyncTrans<T>(string spName, DynamicParameters parameters)
        {
            using var connection = _context.CreateConnection();
            using var transaction = connection.BeginTransaction(); // Transaction begins here

            var result = new DbExecutionResult<T>();

            try
            {
                // Add standard output parameters
                AddStandardOutputParameters(parameters);

                _logger.LogInformation("Executing stored procedure {StoredProcedure} within a transaction.", spName);

                // Execute the stored procedure within the transaction
                var queryResult = await connection.QueryAsync<T>(
                    spName, parameters, transaction: transaction, commandType: CommandType.StoredProcedure);

                // Populate the ExecutionResult
                result.ResultSet = queryResult.ToList();
                result.ReturnStatus = parameters.Get<string>(StoredProcedureParameters.ReturnStatus);
                result.ErrorCode = parameters.Get<string>(StoredProcedureParameters.ErrorCode);

                // Check ReturnStatus and commit/rollback the transaction
                if (result.ReturnStatus?.ToLower() == "error")
                {
                    _logger.LogWarning("Stored procedure returned an error. Rolling back transaction. ErrorCode: {ErrorCode}", result.ErrorCode);
                    transaction.Rollback();
                }
                else
                {
                    transaction.Commit();
                    result.ReturnStatus = "success";
                    result.ErrorCode = "ERR200"; // Success code
                    _logger.LogInformation("Transaction committed successfully for {StoredProcedure}.", spName);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing stored procedure {StoredProcedure} within a transaction. Rolling back transaction.", spName);
                transaction.Rollback();

                result.ReturnStatus = "error";
                result.ErrorCode = ex.Message;
            }

            return result;
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

    public class DbExecutionResult<T>
    {
        public string? ReturnStatus { get; set; }
        public string? ErrorCode { get; set; }
        public List<T> ResultSet { get; set; } = new List<T>(); // Single result set
        public List<IEnumerable<dynamic>> CombinedResultSets { get; set; } = new List<IEnumerable<dynamic>>(); // Multiple result sets
        public bool IsSuccess => ErrorCode == "ERR200";
        public DbExecutionResult() { }
        public DbExecutionResult(string returnStatus, string errorCode)
        {
            ReturnStatus = returnStatus;
            ErrorCode = errorCode;
        }
    }
}
