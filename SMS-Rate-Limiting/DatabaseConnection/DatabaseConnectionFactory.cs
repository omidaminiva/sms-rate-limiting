using MySqlConnector;
using SMS_Rate_Limiting.Settings;

namespace SMS_Rate_Limiting.DatabaseConnection;

public class DatabaseConnectionFactory : IDatabaseConnectionFactory
{
    private readonly string _connectionString;

    public DatabaseConnectionFactory(ISettingRepositories settingRepositories)
    {
        _connectionString = settingRepositories.GetDatabaseConnectionString();
    }

    public async Task<T> QueryAsync<T>(string query, IEnumerable<KeyValuePair<string, object>> parameters)
    {
        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync().ConfigureAwait(false);

        await using var cmd = new MySqlCommand(query, connection);

        foreach (var parameter in parameters)
        {
            cmd.Parameters.AddWithValue(parameter.Key, parameter.Value);
        }

        await using var reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);
        if (await reader.ReadAsync().ConfigureAwait(false))
        {
            return MapTo<T>(reader) ?? throw new InvalidOperationException("Mapping function returned null.");
        }

        throw new InvalidOperationException("Query returned no results.");
    }

    public async Task ExecuteAsync(string query, IEnumerable<KeyValuePair<string, object>> parameters)
    {
        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync().ConfigureAwait(false);

        await using var cmd = new MySqlCommand(query, connection);

        foreach (var parameter in parameters)
        {
            cmd.Parameters.AddWithValue(parameter.Key, parameter.Value);
        }

        await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
    }

    private T? MapTo<T>(MySqlDataReader reader)
    {
        // Add mappings for specific types
        if (typeof(T) == typeof(int))
        {
            return (T)(object)reader.GetInt32(0); // Assuming the query returns a single int
        }

        if (typeof(T) == typeof(string))
        {
            return (T)(object)reader.GetString(0); // Assuming the query returns a single string
        }

        // Handle complex types
        var result = Activator.CreateInstance<T>();
        foreach (var property in typeof(T).GetProperties())
        {
            if (!reader.HasColumn(property.Name) || reader[property.Name] == DBNull.Value)
                continue;

            property.SetValue(result, Convert.ChangeType(reader[property.Name], property.PropertyType));
        }
        return result;
    }
}

public static class MySqlDataReaderExtensions
{
    public static bool HasColumn(this MySqlDataReader reader, string columnName)
    {
        for (int i = 0; i < reader.FieldCount; i++)
        {
            if (reader.GetName(i).Equals(columnName, StringComparison.OrdinalIgnoreCase))
                return true;
        }
        return false;
    }
}
