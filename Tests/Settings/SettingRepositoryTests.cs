using SMS_Rate_Limiting.Settings;

namespace Tests.Settings;

[TestFixture]
public class SettingRepositoryTests
{
    ISettingRepositories _settingRepositories;

    [SetUp]
    public void Setup()
    {
        _settingRepositories = new SettingRepositories();
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
        Environment.SetEnvironmentVariable(SettingRepositories.AccountSmsLimitKey, "10");
        Environment.SetEnvironmentVariable(SettingRepositories.BusinessPhoneNumberSmsLimitKey, "5");
        Environment.SetEnvironmentVariable(SettingRepositories.PhoneNumberActiveDurationInHoursKey, "1");
        Environment.SetEnvironmentVariable(SettingRepositories.GetDatabaseConnectionStringKey, "master");
    }

    [Test]
    public void GetBusinessPhoneNumberSmsLimitTest()
    {
        var result = _settingRepositories.GetBusinessPhoneNumberSmsLimit();
        Assert.AreEqual(5, result);
    }

    [Test]
    public void GetAccountSmsLimitTest()
    {
        var result = _settingRepositories.GetAccountSmsLimit();
        Assert.AreEqual(10, result);
    }

    [Test]
    public void PhoneNumberActiveDurationInHoursTest()
    {
        var result = _settingRepositories.PhoneNumberActiveDurationInHours();
        Assert.AreEqual(1, result);
    }

    [Test]
    public void GetDatabaseConnectionStringTest()
    {
        var result = _settingRepositories.GetDatabaseConnectionString();
        Assert.AreEqual("master", result);
    }
}