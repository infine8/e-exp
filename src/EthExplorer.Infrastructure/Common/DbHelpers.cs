using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.Common;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace EthExplorer.Infrastructure.Common;

public static class DbHelpers
{
    internal static async Task<IReadOnlyList<T>> RawSqlQuery<T>(this DbContext dbContext, string query, Func<DbDataReader, T> map)
    {
        await using var command = dbContext.Database.GetDbConnection().CreateCommand();
        command.CommandText = query;
        command.CommandType = CommandType.Text;

        //await dbContext.Database.OpenConnectionAsync();

        await using var result = await command.ExecuteReaderAsync();
        var entities = new List<T>();

        while (await result.ReadAsync())
        {
            entities.Add(map(result));
        }

        return entities;
    }

    internal static TEntity MapTo<TEntity>(this DbDataReader reader) where TEntity : class, new()
    {
        var properties = typeof(TEntity)
            .GetProperties().Where(p => p.CanWrite && p.GetCustomAttributes(typeof(ColumnAttribute), true).Length > 0)
            .ToDictionary(p => p.GetCustomAttribute<ColumnAttribute>().Name, StringComparer.OrdinalIgnoreCase);

        var obj = new TEntity();

        for (var i = 0; i < reader.FieldCount; i++)
        {
            try
            {
                var columnName = reader.GetName(i);

                if (!properties.TryGetValue(columnName, out var prop)) continue;

                var value = reader.GetValue(i);
                
                var type = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                var safeValue = value is null ? null : Convert.ChangeType(value, type);
                
                prop.SetValue(obj, safeValue);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        return obj;
    }

    internal static async Task<T> RawSqlQueryFirst<T>(this DbContext dbContext, string query, Func<DbDataReader, T> map)
    {
        var items = await dbContext.RawSqlQuery(query, map);
        return items.First();
    }

    internal static async Task<T> RawSqlQueryFirst<T>(this DbContext dbContext, string query)
    {
        var items = await dbContext.RawSqlQuery(query, row => new { Value = row[0] });
        return (T)items.First().Value;
    }

    internal static async Task<object?> RawSqlQueryFirst(this DbContext dbContext, string query)
    {
        var items = await dbContext.RawSqlQuery(query, row => new { Value = row[0] });
        return items.First().Value;
    }

    internal static async Task<T?> RawSqlQueryFirstOfDefault<T>(this DbContext dbContext, string query, Func<DbDataReader, T> map)
    {
        var items = await dbContext.RawSqlQuery(query, map);
        return items.FirstOrDefault();
    }

    internal static async Task<T?> RawSqlQueryFirstOfDefault<T>(this DbContext dbContext, string query)
    {
        var items = await dbContext.RawSqlQuery(query, row => new { Value = row[0] });
        return (T?)items.FirstOrDefault()?.Value;
    }

    internal static async Task<object?> RawSqlQueryFirstOfDefault(this DbContext dbContext, string query)
    {
        var items = await dbContext.RawSqlQuery(query, row => new { Value = row[0] });
        return items.FirstOrDefault()?.Value;
    }

    internal static string GetTableName<TEntity>() where TEntity : class
    {
        return typeof(TEntity).GetCustomAttribute<TableAttribute>().Name;
    }

    internal static IReadOnlyDictionary<string, object> GetColumns<TEntity>(TEntity entity) where TEntity : class
    {
        var dict = new Dictionary<string, object>();

        foreach (var field in typeof(TEntity).GetProperties())
        {
            dict.Add(field.GetCustomAttribute<ColumnAttribute>().Name, field.GetValue(entity, null));
        }

        return dict;
    }
}