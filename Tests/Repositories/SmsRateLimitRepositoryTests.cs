using NSubstitute;
using SMS_Rate_Limiting.DatabaseConnection;
using SMS_Rate_Limiting.Repositories;

namespace Tests.Repositories;

[TestFixture]
public class SmsRateLimitRepositoryTests
{
    private ISmsRateLimitRepository _repository;
    private IDatabaseConnectionFactory _databaseConnectionFactory;

    [SetUp]
    public void Setup()
    {
        _databaseConnectionFactory = Substitute.For<IDatabaseConnectionFactory>();
        
        _repository = new SmsRateLimitRepository(_databaseConnectionFactory);
    }

    [Test]
    public void IncrementNumberOfSmsSentTest()
    {
        var phoneNumber = "phoneNumber";
        _repository.SmsSent(phoneNumber);
        _databaseConnectionFactory.Received(1).ExecuteAsync("INSERT INTO SmsMessages (Id, PhoneNumber) VALUES (@Id, @phoneNumber)",
            Arg.Any<IEnumerable<KeyValuePair<String, Object>>>());

    }

    [Test]
    public async Task GetNumberOfSmsSentTest()
    {
        var phoneNumber = "phoneNumber";
        var sqlQuery = @"SELECT COUNT(*)
        FROM SmsMessages
        WHERE
            PhoneNumber = @phoneNumber AND
            SentAt >= DATE_SUB(NOW(), INTERVAL @interval HOUR)";
        await _repository.GetNumberOfSmsSent(phoneNumber, 1);
        _databaseConnectionFactory.Received(1).QueryAsync<int>(sqlQuery,  
            Arg.Any<IEnumerable<KeyValuePair<String, Object>>>());
    }

    [Test]
    public async Task GetNumberOfSmsSentAccountTest()
    {
        var sqlQuery = @"SELECT COUNT(*)
        FROM SmsMessages
        WHERE SentAt >= DATE_SUB(NOW(), INTERVAL @interval HOUR)";
        await _repository.GetNumberOfSmsSent(1);
        _databaseConnectionFactory.Received(1).QueryAsync<int>(sqlQuery,
            Arg.Any<IEnumerable<KeyValuePair<String, Object>>>());
    }

}