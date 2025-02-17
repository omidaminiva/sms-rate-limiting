namespace SMS_Rate_Limiting.DatabaseConnection;

public interface IDatabaseConnectionFactory
{
    public Task<T> QueryAsync<T>(String query, IEnumerable<KeyValuePair<String, Object>> parameters);
    
    public Task ExecuteAsync(String query, IEnumerable<KeyValuePair<String, Object>> parameters);
}