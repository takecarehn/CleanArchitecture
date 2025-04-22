using System.Data;
using Dapper;
using static Dapper.SqlMapper;

namespace CleanArchitecture.Application.Common.Interfaces;
public interface IDapperService : IDisposable
{
    DynamicParameters CreateDynamicParameters<T>(T data) where T : class;

    DynamicParameters CreateDynamicParameters(Dictionary<string, object> data);

    int Execute(string sql, object parameters, CommandType commandType = CommandType.StoredProcedure);

    Task<int> ExecuteAsync(string sql, object parameters, CommandType commandType = CommandType.StoredProcedure);

    T? ExecuteScalar<T>(string sql, object parameters, CommandType commandType = CommandType.StoredProcedure);

    Task<T?> ExecuteScalarAsync<T>(string sql, object parameters, CommandType commandType = CommandType.StoredProcedure);

    T? QueryFirstOrDefault<T>(string sql, DynamicParameters parameters, CommandType commandType = CommandType.StoredProcedure);

    Task<T?> QueryFirstOrDefaultAsync<T>(string sql, DynamicParameters parameters, CommandType commandType = CommandType.StoredProcedure);

    IEnumerable<T> Query<T>(string sql, DynamicParameters parameters, CommandType commandType = CommandType.StoredProcedure);

    Task<IEnumerable<T>> QueryAsync<T>(string sql, DynamicParameters parameters, CommandType commandType = CommandType.StoredProcedure);

    GridReader QueryMultiple(string sql, DynamicParameters parameters, CommandType commandType = CommandType.StoredProcedure);

    Task<GridReader> QueryMultipleAsync(string sql, DynamicParameters parameters, CommandType commandType = CommandType.StoredProcedure);
}
