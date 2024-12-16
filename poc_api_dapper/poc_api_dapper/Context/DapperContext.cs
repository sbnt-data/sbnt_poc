using Microsoft.Data.SqlClient;
using System.Data;

namespace poc_api_dapper.Context
{
    public class DapperContext
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        private readonly ILogger<DapperContext> _logger;

        public DapperContext(IConfiguration configuration, ILogger<DapperContext> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _connectionString = _configuration.GetConnectionString("SqlConnection");

            if (string.IsNullOrEmpty(_connectionString))
            {
                _logger.LogError("Connection string for 'SqlConnection' is missing or empty.");
                throw new InvalidOperationException("Database connection string is not configured.");
            }

            _logger.LogInformation("Database connection string successfully loaded.");
        }

        public IDbConnection CreateConnection()
        {
            try
            {
                _logger.LogDebug("Attempting to create a new SQL connection.");
                return new SqlConnection(_connectionString);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL connection creation failed.");
                throw new InvalidOperationException("Error creating database connection", ex);
            }
        }
    }
}
