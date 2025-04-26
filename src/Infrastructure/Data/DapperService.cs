using System.Data;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Mappings;
using Dapper;
using Microsoft.Data.SqlClient;
using static Dapper.SqlMapper;

namespace CleanArchitecture.Infrastructure.Data;

public class DapperService : IDapperService
{
    private readonly DynamicParameterMap dynamicParameterMap = new DynamicParameterMap();
    private readonly IDbConnection _dbConnection;

    public DapperService(string connectionString)
    {
        _dbConnection = new SqlConnection(connectionString);
    }

    public void Dispose()
    {
        try
        {
            this._dbConnection.Dispose();
        }
        catch (Exception)
        {
        }
    }

    public DynamicParameters CreateDynamicParameters<T>(T data) where T : class
    {
        return this.dynamicParameterMap.Map<T>(data);
    }

    public DynamicParameters CreateDynamicParameters(Dictionary<string, object> data)
    {
        return this.dynamicParameterMap.Map(data);
    }

    public int Execute(string sql, object parameters, CommandType commandType = CommandType.StoredProcedure)
    {
        return _dbConnection.Execute(sql, parameters, null, null, commandType);
    }

    public async Task<int> ExecuteAsync(string sql, object parameters, CommandType commandType = CommandType.StoredProcedure)
    {
        return await _dbConnection.ExecuteAsync(sql, parameters, null, null, commandType);
    }

    public T? ExecuteScalar<T>(string sql, object parameters, CommandType commandType = CommandType.StoredProcedure)
    {
        return _dbConnection.ExecuteScalar<T>(sql, parameters, null, null, commandType);
    }

    public async Task<T?> ExecuteScalarAsync<T>(string sql, object parameters, CommandType commandType = CommandType.StoredProcedure)
    {
        return await _dbConnection.ExecuteScalarAsync<T>(sql, parameters, null, null, commandType);
    }

    public IEnumerable<T> Query<T>(string sql, DynamicParameters parameters, CommandType commandType = CommandType.StoredProcedure)
    {
        return _dbConnection.Query<T>(sql, parameters, null, true, null, commandType);
    }

    public async Task<IEnumerable<T>> QueryAsync<T>(string sql, DynamicParameters parameters, CommandType commandType = CommandType.StoredProcedure)
    {
        return await _dbConnection.QueryAsync<T>(sql, parameters, null, null, commandType);
    }

    public T? QueryFirstOrDefault<T>(string sql, DynamicParameters parameters, CommandType commandType = CommandType.StoredProcedure)
    {
        return _dbConnection.QueryFirstOrDefault<T>(sql, parameters, null, null, commandType);
    }

    public async Task<T?> QueryFirstOrDefaultAsync<T>(string sql, DynamicParameters parameters, CommandType commandType = CommandType.StoredProcedure)
    {
        return await _dbConnection.QueryFirstOrDefaultAsync<T>(sql, parameters, null, null, commandType);
    }

    public GridReader QueryMultiple(string sql, DynamicParameters parameters, CommandType commandType = CommandType.StoredProcedure)
    {
        return _dbConnection.QueryMultiple(sql, parameters, null, null, commandType);
    }

    public async Task<GridReader> QueryMultipleAsync(string sql, DynamicParameters parameters, CommandType commandType = CommandType.StoredProcedure)
    {
        return await _dbConnection.QueryMultipleAsync(sql, parameters, null, null, commandType);
    }
}
