using Dapper;
using System.Data;

namespace PMS.Persistence.DataAccessLayer
{
    public class DataAccess
    {
        private IDbConnection _dbConnect;
        private ReturnDapper _returnDapper;

        public DataAccess(IDbConnection dbConnect, ReturnDapper returnDapper)
        {
            _dbConnect = dbConnect;
            _returnDapper = returnDapper;
        }
        
        public void EnsureConnectionOpen()
        {
            if (_dbConnect.State == ConnectionState.Closed)
            {
                _dbConnect.Open();
            }
        }

        public void EnsureConnectionClosed()
        {
            if (_dbConnect.State == ConnectionState.Open)
            {
                _dbConnect.Close();
            }
        }

        public ReturnDapper Query(String SpName, DynamicParameters parameters)
        {
            EnsureConnectionOpen();
            try
            {
                // Add output parameters
                parameters.Add("@ReturnStatus", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);
                parameters.Add("@ErrorCode", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);

                var result = _dbConnect.Query(SpName, parameters, commandType: CommandType.StoredProcedure).ToList();

                // Read the output parameters
                _returnDapper.ReturnStatus = parameters.Get<string>("@ReturnStatus");
                _returnDapper.ErrorCode = parameters.Get<string>("@ErrorCode");
                _returnDapper.ListResultset = result;

                return _returnDapper;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                _returnDapper.ReturnStatus = "error"; ;
                _returnDapper.ErrorCode = "ERR400";
                return _returnDapper;
            }
            finally
            {
                EnsureConnectionClosed();
            }
        }

        public ReturnDapper QueryTrans(String SpName, DynamicParameters parameters)
        {
            EnsureConnectionOpen();    
            using (var transaction = _dbConnect.BeginTransaction())
            {
                try
                {
                    // Add output parameters
                    parameters.Add("@ReturnStatus", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);
                    parameters.Add("@ErrorCode", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);

                    var result = _dbConnect.Query(SpName, parameters, transaction: transaction, commandType: CommandType.StoredProcedure).ToList();

                    // Read the output parameters
                    _returnDapper.ReturnStatus = parameters.Get<string>("@ReturnStatus");
                    _returnDapper.ErrorCode = parameters.Get<string>("@ErrorCode");
                    _returnDapper.ListResultset = result;

                    if (_returnDapper.ReturnStatus == "success")
                    {
                        transaction.Commit();
                    }
                    else
                    {
                        transaction.Rollback();
                    }

                    return _returnDapper;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _returnDapper.ReturnStatus = "error";
                    _returnDapper.ErrorCode = ex.Message.ToString();
                    return _returnDapper;
                }
                finally
                {
                    EnsureConnectionClosed();
                }
            }
        }

        public ReturnDapper QueryFirst(String SpName, DynamicParameters parameters)
        {
            EnsureConnectionOpen();
            try
            {
                // Add output parameters
                parameters.Add("@ReturnStatus", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);
                parameters.Add("@ErrorCode", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);

                var result = _dbConnect.QueryFirst(SpName, parameters, commandType: CommandType.StoredProcedure).ToList();

                // Read the output parameters
                _returnDapper.ReturnStatus = parameters.Get<string>("@ReturnStatus");
                _returnDapper.ErrorCode = parameters.Get<string>("@ErrorCode");
                _returnDapper.ListResultset = result;

                return _returnDapper;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                _returnDapper.ReturnStatus = "error";
                _returnDapper.ErrorCode = "ERR400";
                return _returnDapper;
            }
            finally
            {
                EnsureConnectionClosed();
            }
        }

        public ReturnDapper QueryFirstOrDefault(String SpName, DynamicParameters parameters)
        {
            EnsureConnectionOpen();
            try
            {
                // Add output parameters
                parameters.Add("@ReturnStatus", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);
                parameters.Add("@ErrorCode", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);

                var result = _dbConnect.QueryFirstOrDefault(SpName, parameters, commandType: CommandType.StoredProcedure).ToList();

                // Read the output parameters
                _returnDapper.ReturnStatus = parameters.Get<string>("@ReturnStatus");
                _returnDapper.ErrorCode = parameters.Get<string>("@ErrorCode");
                _returnDapper.ListResultset = result;

                return _returnDapper;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                _returnDapper.ReturnStatus = "error";
                _returnDapper.ErrorCode = "ERR400";
                return _returnDapper;
            }
            finally
            {
                EnsureConnectionClosed();
            }
        }

        public ReturnDapper QuerySingle(String SpName, DynamicParameters parameters)
        {
            EnsureConnectionOpen();
            try
            {
                // Add output parameters
                parameters.Add("@ReturnStatus", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);
                parameters.Add("@ErrorCode", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);

                var result = _dbConnect.QuerySingle(SpName, parameters, commandType: CommandType.StoredProcedure).ToList();

                // Read the output parameters
                _returnDapper.ReturnStatus = parameters.Get<string>("@ReturnStatus");
                _returnDapper.ErrorCode = parameters.Get<string>("@ErrorCode");
                _returnDapper.ListResultset = result;

                return _returnDapper;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                _returnDapper.ReturnStatus = "error";
                _returnDapper.ErrorCode = "ERR400";
                return _returnDapper;
            }
            finally
            {
                EnsureConnectionClosed();
            }
        }

        public ReturnDapper QuerySingleOrDefault(String SpName, DynamicParameters parameters)
        {
            EnsureConnectionOpen();
            try
            {
                // Add output parameters
                parameters.Add("@ReturnStatus", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);
                parameters.Add("@ErrorCode", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);

                var result = _dbConnect.QuerySingleOrDefault(SpName, parameters, commandType: CommandType.StoredProcedure).ToList();

                // Read the output parameters
                _returnDapper.ReturnStatus = parameters.Get<string>("@ReturnStatus");
                _returnDapper.ErrorCode = parameters.Get<string>("@ErrorCode");
                _returnDapper.ListResultset = result;

                return _returnDapper;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                _returnDapper.ReturnStatus = "error";
                _returnDapper.ErrorCode = "ERR400";
                return _returnDapper;
            }
            finally
            {
                EnsureConnectionClosed();
            }
        }

        public async Task<ReturnDapper> QueryAsync(String SpName, DynamicParameters parameters)
        {
            EnsureConnectionOpen();
            try
            {
                // Add output parameters
                parameters.Add("@ReturnStatus", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);
                parameters.Add("@ErrorCode", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);

                var result = await _dbConnect.QueryAsync(SpName, parameters, commandType: CommandType.StoredProcedure);

                // Read the output parameters
                _returnDapper.ReturnStatus = parameters.Get<string>("@ReturnStatus");
                _returnDapper.ErrorCode = parameters.Get<string>("@ErrorCode");
                _returnDapper.ListResultset = result.ToList();

                return _returnDapper;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                _returnDapper.ReturnStatus = "error";
                _returnDapper.ErrorCode = "ERR400";
                return _returnDapper;
            }
            finally
            {
                EnsureConnectionClosed();
            }
        }

        public async Task<ReturnDapper<T>> QueryAsync<T>(String SpName, DynamicParameters parameters)
        {
            EnsureConnectionOpen();
            var _returnDapper = new ReturnDapper<T>();
            try
            {
                // Add output parameters
                parameters.Add("@ReturnStatus", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);
                parameters.Add("@ErrorCode", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);


                var result = await _dbConnect.QueryAsync<T>(SpName, parameters, commandType: CommandType.StoredProcedure);

                //await Task.Delay(100);
                // Read the output parameters
                _returnDapper.ReturnStatus = parameters.Get<string>("@ReturnStatus");
                _returnDapper.ErrorCode = parameters.Get<string>("@ErrorCode");
                _returnDapper.ListResultset = result.ToList();

                return _returnDapper;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                _returnDapper.ReturnStatus = ex.Message.ToString();// "error";
                _returnDapper.ErrorCode = "ERR400";
                return _returnDapper;
            }
            finally
            {
                EnsureConnectionClosed(); 
            }
        }

        public async Task<ReturnDapper> QueryAsyncTrans(String SpName, DynamicParameters parameters)
        {
            EnsureConnectionOpen();
            using (var transaction = _dbConnect.BeginTransaction())
            {
                try
                {
                    // Add output parameters
                    parameters.Add("@ReturnStatus", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);
                    parameters.Add("@ErrorCode", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);

                    var result = await _dbConnect.QueryAsync(SpName, parameters, transaction: transaction, commandType: CommandType.StoredProcedure);

                    // Read the output parameters
                    _returnDapper.ReturnStatus = parameters.Get<string>("@ReturnStatus");
                    _returnDapper.ErrorCode = parameters.Get<string>("@ErrorCode");
                    _returnDapper.ListResultset = result.ToList();

                    if (_returnDapper.ReturnStatus == "error")
                    {
                        transaction.Rollback();
                    }
                    else
                    {
                        transaction.Commit();
                        _returnDapper.ReturnStatus = "success";
                        _returnDapper.ErrorCode = "ERR200";

                    }

                    return _returnDapper;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _returnDapper.ReturnStatus = "error";
                    _returnDapper.ErrorCode = ex.Message.ToString();
                    return _returnDapper;
                }
                finally
                {
                    EnsureConnectionClosed();
                }
            }
        }

        public async Task<ReturnDapper> QueryFirstAsync(String SpName, DynamicParameters parameters)
        {
            EnsureConnectionOpen();
            try
            {
                // Add output parameters
                parameters.Add("@ReturnStatus", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);
                parameters.Add("@ErrorCode", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);

                var result = await _dbConnect.QueryFirstAsync(SpName, parameters, commandType: CommandType.StoredProcedure);

                // Read the output parameters
                _returnDapper.ReturnStatus = parameters.Get<string>("@ReturnStatus");
                _returnDapper.ErrorCode = parameters.Get<string>("@ErrorCode");
                _returnDapper.ListResultset = result;


                return _returnDapper;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                _returnDapper.ReturnStatus = "error";
                _returnDapper.ErrorCode = "ERR400";
                return _returnDapper;
            }
            finally
            {
                EnsureConnectionClosed();
            }
        }

        public async Task<ReturnDapper> QueryFirstOrDefaultAsync(String SpName, DynamicParameters parameters)
        {
            EnsureConnectionOpen();
            try
            {
                // Add output parameters
                parameters.Add("@ReturnStatus", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);
                parameters.Add("@ErrorCode", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);

                var result = await _dbConnect.QueryFirstOrDefaultAsync(SpName, parameters, commandType: CommandType.StoredProcedure);

                // Read the output parameters
                _returnDapper.ReturnStatus = parameters.Get<string>("@ReturnStatus");
                _returnDapper.ErrorCode = parameters.Get<string>("@ErrorCode");
                _returnDapper.ListResultset = result;


                return _returnDapper;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                _returnDapper.ReturnStatus = "error";
                _returnDapper.ErrorCode = "ERR400";
                return _returnDapper;
            }
            finally
            {
                EnsureConnectionClosed();
            }
        }

        public async Task<ReturnDapper> QuerySingleAsync(String SpName, DynamicParameters parameters)
        {
            EnsureConnectionOpen();
            try
            {
                // Add output parameters
                parameters.Add("@ReturnStatus", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);
                parameters.Add("@ErrorCode", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);

                var result = await _dbConnect.QuerySingleAsync(SpName, parameters, commandType: CommandType.StoredProcedure);

                // Read the output parameters
                _returnDapper.ReturnStatus = parameters.Get<string>("@ReturnStatus");
                _returnDapper.ErrorCode = parameters.Get<string>("@ErrorCode");
                _returnDapper.ListResultset = result;


                return _returnDapper;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                _returnDapper.ReturnStatus = "error";
                _returnDapper.ErrorCode = "ERR400";
                return _returnDapper;
            }
            finally
            {
                EnsureConnectionClosed();
            }
        }

        public async Task<ReturnDapper> QuerySingleOrDefaultAsync(String SpName, DynamicParameters parameters)
        {
            EnsureConnectionOpen();
            try
            {
                // Add output parameters
                parameters.Add("@ReturnStatus", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);
                parameters.Add("@ErrorCode", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);

                var result = await _dbConnect.QuerySingleOrDefaultAsync(SpName, parameters, commandType: CommandType.StoredProcedure);

                // Read the output parameters
                _returnDapper.ReturnStatus = parameters.Get<string>("@ReturnStatus");
                _returnDapper.ErrorCode = parameters.Get<string>("@ErrorCode");
                _returnDapper.ListResultset = result;


                return _returnDapper;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                _returnDapper.ReturnStatus = "error";
                _returnDapper.ErrorCode = "ERR400";
                return _returnDapper;
            }
            finally
            {
                EnsureConnectionClosed();
            }
        }

        public ReturnDapper QueryMultiple(String SpName, DynamicParameters parameters)
        {
            try
            {
                // Add output parameters
                parameters.Add("@ReturnStatus", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);
                parameters.Add("@ErrorCode", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);

                var result = _dbConnect.QueryMultiple(SpName, parameters, commandType: CommandType.StoredProcedure);
                // Read the output parameters
                _returnDapper.ReturnStatus = parameters.Get<string>("@ReturnStatus");
                _returnDapper.ErrorCode = parameters.Get<string>("@ErrorCode");
                _returnDapper.Gridreader = result;

                return _returnDapper;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                _returnDapper.ReturnStatus = "error";
                _returnDapper.ErrorCode = "ERR400";
                return _returnDapper;
            }
        }
        public ReturnDapper QueryMultipleTrans(String SpName, DynamicParameters parameters)
        {
            using (var transaction = _dbConnect.BeginTransaction())
            {
                try
                {
                    // Add output parameters
                    parameters.Add("@ReturnStatus", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);
                    parameters.Add("@ErrorCode", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);

                    var result = _dbConnect.QueryMultiple(SpName, parameters, transaction: transaction, commandType: CommandType.StoredProcedure);
                    // Read the output parameters
                    _returnDapper.ReturnStatus = parameters.Get<string>("@ReturnStatus");
                    _returnDapper.ErrorCode = parameters.Get<string>("@ErrorCode");
                    _returnDapper.Gridreader = result;

                    if (_returnDapper.ReturnStatus == "error")
                    {
                        transaction.Rollback();
                    }
                    else
                    {
                        transaction.Commit();
                        _returnDapper.ReturnStatus = "success";
                        _returnDapper.ErrorCode = "ERR200";

                    }

                    return _returnDapper;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _returnDapper.ReturnStatus = "error";
                    _returnDapper.ErrorCode = ex.Message.ToString();
                    return _returnDapper;
                }
            }
        }

        public async Task<ReturnDapper> QueryMultipleAsync(String SpName, DynamicParameters parameters)
        {
            try
            {
                // Add output parameters
                parameters.Add("@ReturnStatus", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);
                parameters.Add("@ErrorCode", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);

                var result = await _dbConnect.QueryMultipleAsync(SpName, parameters, commandType: CommandType.StoredProcedure);

                // Read the output parameters
                _returnDapper.ReturnStatus = parameters.Get<string>("@ReturnStatus");
                _returnDapper.ErrorCode = parameters.Get<string>("@ErrorCode");
                _returnDapper.Gridreader = result;


                return _returnDapper;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                _returnDapper.ReturnStatus = "error";
                _returnDapper.ErrorCode = "ERR400";
                return _returnDapper;
            }
        }
        public async Task<ReturnDapper> QueryMultipleAsyncTrans(string spName, DynamicParameters parameters)
        {
            using (var transaction = _dbConnect.BeginTransaction())
            {
                try
                {
                    // Add output parameters
                    parameters.Add("@ReturnStatus", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);
                    parameters.Add("@ErrorCode", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);

                    using (var result = await _dbConnect.QueryMultipleAsync(spName, parameters, transaction: transaction, commandType: CommandType.StoredProcedure))
                    {
                        var combinedResultSet = new List<IEnumerable<dynamic>>();

                        while (!result.IsConsumed)
                        {
                            var resultSet = (await result.ReadAsync()).ToArray();
                            combinedResultSet.Add(resultSet);
                        }

                        _returnDapper.ReturnStatus = parameters.Get<string>("@ReturnStatus");
                        _returnDapper.ErrorCode = parameters.Get<string>("@ErrorCode");

                        if (_returnDapper.ReturnStatus == "error")
                        {
                            transaction.Rollback();
                        }
                        else
                        {
                            transaction.Commit();
                        }

                        _returnDapper.CombinedResultSet = combinedResultSet;
                    }

                    return _returnDapper;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _returnDapper.ReturnStatus = "error";
                    _returnDapper.ErrorCode = ex.Message.ToString();
                    return _returnDapper;
                }
            }
        }

        public ReturnDapper Execute(String SpName, DynamicParameters parameters)
        {
            EnsureConnectionOpen();
            try
            {
                // Add output parameters
                parameters.Add("@ReturnStatus", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);
                parameters.Add("@ErrorCode", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);

                var result = _dbConnect.Execute(SpName, parameters, commandType: CommandType.StoredProcedure);

                // Read the output parameters
                _returnDapper.ReturnStatus = parameters.Get<string>("@ReturnStatus");
                _returnDapper.ErrorCode = parameters.Get<string>("@ErrorCode");
                _returnDapper.Result = result;


                return _returnDapper;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                _returnDapper.ReturnStatus = "error";
                _returnDapper.ErrorCode = "ERR400";
                return _returnDapper;
            }
            finally
            {
                EnsureConnectionClosed();
            }
        }
        public ReturnDapper ExecuteTrans(String SpName, DynamicParameters parameters)
        {
            EnsureConnectionOpen();
            using (var transaction = _dbConnect.BeginTransaction())
            {
                try
                {
                    // Add output parameters
                    parameters.Add("@ReturnStatus", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);
                    parameters.Add("@ErrorCode", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);

                    var result = _dbConnect.Execute(SpName, parameters, transaction: transaction, commandType: CommandType.StoredProcedure);

                    // Read the output parameters
                    _returnDapper.ReturnStatus = parameters.Get<string>("@ReturnStatus");
                    _returnDapper.ErrorCode = parameters.Get<string>("@ErrorCode");
                    _returnDapper.Result = result;

                    if (_returnDapper.ReturnStatus == "success")
                    {
                        transaction.Commit();
                    }
                    else
                    {
                        transaction.Rollback();
                    }

                    return _returnDapper;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _returnDapper.ReturnStatus = "error";
                    _returnDapper.ErrorCode = ex.Message.ToString();
                    return _returnDapper;

                }
                finally
                {
                    EnsureConnectionClosed();
                }
            }
        }
        public async Task<ReturnDapper> ExecuteAsync(String SpName, DynamicParameters parameters)
        {
            EnsureConnectionOpen();
            try
            {
                // Add output parameters
                parameters.Add("@ReturnStatus", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);
                parameters.Add("@ErrorCode", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);

                var result = await _dbConnect.ExecuteAsync(SpName, parameters, commandType: CommandType.StoredProcedure);

                // Read the output parameters
                _returnDapper.ReturnStatus = parameters.Get<string>("@ReturnStatus");
                _returnDapper.ErrorCode = parameters.Get<string>("@ErrorCode");
                _returnDapper.Result = result;


                return _returnDapper;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                _returnDapper.ReturnStatus = "error";
                _returnDapper.ErrorCode = "ERR400";
                return _returnDapper;
            }
            finally
            {
                EnsureConnectionClosed();
            }
        }
        public async Task<ReturnDapper> ExecuteAsyncTrans(String SpName, DynamicParameters parameters)
        {
            EnsureConnectionOpen();
            using (var transaction = _dbConnect.BeginTransaction())
            {
                try
                {
                    // Add output parameters
                    parameters.Add("@ReturnStatus", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);
                    parameters.Add("@ErrorCode", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);

                    var result = await _dbConnect.ExecuteAsync(SpName, parameters, transaction: transaction, commandType: CommandType.StoredProcedure);

                    // Read the output parameters
                    _returnDapper.ReturnStatus = parameters.Get<string>("@ReturnStatus");
                    _returnDapper.ErrorCode = parameters.Get<string>("@ErrorCode");
                    _returnDapper.Result = result;

                    if (_returnDapper.ReturnStatus == "success")
                    {
                        transaction.Commit();
                    }
                    else
                    {
                        transaction.Rollback();
                    }

                    return _returnDapper;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _returnDapper.ReturnStatus = "error";
                    _returnDapper.ErrorCode = ex.Message.ToString();
                    return _returnDapper;
                }
                finally
                {
                    EnsureConnectionClosed();
                }
            }
        }
        public ReturnDapper ExecuteScalar(String SpName, DynamicParameters parameters)
        {
            EnsureConnectionOpen();
            try
            {
                // Add output parameters
                parameters.Add("@ReturnStatus", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);
                parameters.Add("@ErrorCode", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);

                var result = _dbConnect.ExecuteScalar(SpName, parameters, commandType: CommandType.StoredProcedure);

                // Read the output parameters
                _returnDapper.ReturnStatus = parameters.Get<string>("@ReturnStatus");
                _returnDapper.ErrorCode = parameters.Get<string>("@ErrorCode");
                _returnDapper.Result = result;


                return _returnDapper;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                _returnDapper.ReturnStatus = "error";
                _returnDapper.ErrorCode = "ERR400";
                return _returnDapper;
            }
            finally
            {
                EnsureConnectionClosed();
            }
        }
        public ReturnDapper ExecuteReader(String SpName, DynamicParameters parameters)
        {
            EnsureConnectionOpen();
            try
            {
                // Add output parameters
                parameters.Add("@ReturnStatus", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);
                parameters.Add("@ErrorCode", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);

                var result = _dbConnect.ExecuteReader(SpName, parameters, commandType: CommandType.StoredProcedure);

                // Read the output parameters
                _returnDapper.ReturnStatus = parameters.Get<string>("@ReturnStatus");
                _returnDapper.ErrorCode = parameters.Get<string>("@ErrorCode");
                _returnDapper.DataReader = result;


                return _returnDapper;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                _returnDapper.ReturnStatus = "error";
                _returnDapper.ErrorCode = "ERR400";
                return _returnDapper;
            }
            finally
            {
                EnsureConnectionClosed();
            }
        }

        public async Task<ReturnDapper> ExecuteScalarAsync(String SpName, DynamicParameters parameters)
        {
            EnsureConnectionOpen();
            try
            {
                // Add output parameters
                parameters.Add("@ReturnStatus", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);
                parameters.Add("@ErrorCode", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);

                var result = await _dbConnect.ExecuteScalarAsync(SpName, parameters, commandType: CommandType.StoredProcedure);

                // Read the output parameters
                _returnDapper.ReturnStatus = parameters.Get<string>("@ReturnStatus");
                _returnDapper.ErrorCode = parameters.Get<string>("@ErrorCode");
                _returnDapper.Result = result;


                return _returnDapper;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                _returnDapper.ReturnStatus = "error";
                _returnDapper.ErrorCode = "ERR400";
                return _returnDapper;
            }
            finally
            {
                EnsureConnectionClosed();
            }
        }
    }
    public class ReturnDapper
    {
        public String ReturnStatus { get; set; }
        public string ErrorCode { get; set; }
        public dynamic Result { get; set; }
        public SqlMapper.GridReader Gridreader { get; set; }
        public List<dynamic> ListResultset { get; set; }
        public List<IEnumerable<dynamic>> CombinedResultSet { get; set; }
        public IDataReader DataReader { get; set; }
    }
    public class ReturnDapper<T>
    {
        public String ReturnStatus { get; set; }
        public string ErrorCode { get; set; }
        public dynamic Result { get; set; }
        public SqlMapper.GridReader Gridreader { get; set; }
        public List<T> ListResultset { get; set; }
        public List<IEnumerable<dynamic>> CombinedResultSet { get; set; }
        public IDataReader DataReader { get; set; }
    }

}

// --------------------------------------------
// Queries

// Explicit Connection Management
// Why is explicit connection management (EnsureConnectionOpen/EnsureConnectionClosed) preferred over using 
// using statements, which automatically handle the connection lifecycle?

// General Practices
// Why are synchronous methods like Query and Execute implemented alongside their async counterparts? Are there specific 
// scenarios where synchronous methods are still necessary in the application?

// Error Handling
// Many methods return a generic "ERR400" error code. 
// Is there a plan to implement more specific error codes to better identify issues?

// Why are exceptions caught and logged using Console.WriteLine instead of using a 
// centralized logging mechanism (e.g., Serilog, NLog)?

// Is there a reason for not rethrowing exceptions or wrapping them in custom exceptions 
// for higher-level error tracking?

// Output Parameters
// Output parameters (e.g., @ReturnStatus, @ErrorCode) are added in every method. 
// Would it be better to centralize this logic in a helper method to reduce repetition?

// Transaction Management
// For transactional methods like QueryTrans and ExecuteAsyncTrans, 
// why is manual transaction handling used instead of letting higher-level services manage transactions?

// Is there a specific use case where transaction management needs to be tightly 
// coupled with database operations at this level?