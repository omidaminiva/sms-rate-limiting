using SMS_Rate_Limiting.DatabaseConnection;

namespace SMS_Rate_Limiting.Repositories;

public class SmsRateLimitRepository(IDatabaseConnectionFactory connectionFactory) : ISmsRateLimitRepository
{
    public async Task SmsSent(string phoneNumber)
    {
        var id = Guid.NewGuid().ToString();
        var sqlQuery = @"INSERT INTO SmsMessages (Id, PhoneNumber) VALUES (@Id, @phoneNumber)";
        await connectionFactory.ExecuteAsync(sqlQuery,
            [ 
                new KeyValuePair<string, object>("Id", id),
                new KeyValuePair<string, object>("PhoneNumber", phoneNumber)
            ]);
    }

    public async Task<int> GetNumberOfSmsSent(string phoneNumber, int interval)
    {
        var sqlQuery = @"SELECT COUNT(*)
        FROM SmsMessages
        WHERE
            PhoneNumber = @phoneNumber AND
            SentAt >= DATE_SUB(NOW(), INTERVAL @interval HOUR)";
        return await connectionFactory.QueryAsync<int>(sqlQuery, [
            new KeyValuePair<string, object>("PhoneNumber", phoneNumber),
            new KeyValuePair<string, object>("interval", interval)
        ]);
    }

    public async Task<int> GetNumberOfSmsSent(int interval)
    {
        var sqlQuery = @"SELECT COUNT(*)
        FROM SmsMessages
        WHERE SentAt >= DATE_SUB(NOW(), INTERVAL @interval HOUR)";
        return await connectionFactory.QueryAsync<int>(sqlQuery, [
            new KeyValuePair<string, object>("interval", interval)
        ]);
    }
}