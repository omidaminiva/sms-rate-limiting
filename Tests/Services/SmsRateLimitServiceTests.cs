using Microsoft.Extensions.Logging;
using NSubstitute;
using SMS_Rate_Limiting.Repositories;
using SMS_Rate_Limiting.Services;
using SMS_Rate_Limiting.Settings;

namespace Tests.Services;

[TestFixture]
public class SmsRateLimitServiceTests
{
    
    private ISmsRateLimitService _service;
    private List<string> _smsSent;
    private ISmsRateLimitRepository _smsRateLimitRepository;
    
    [SetUp]
    public void Setup()
    {
        _smsSent = new List<string>();
        ISettingRepositories settingRepositories = Substitute.For<ISettingRepositories>();
        settingRepositories.GetAccountSmsLimit().Returns(5);
        settingRepositories.GetBusinessPhoneNumberSmsLimit().Returns(3);
        settingRepositories.PhoneNumberActiveDurationInHours().Returns(1);
        ILogger<SmsRateLimitService> logger = Substitute.For<ILogger<SmsRateLimitService>>();
        _smsRateLimitRepository = Substitute.For<ISmsRateLimitRepository>();
        _smsRateLimitRepository.When(x => x.SmsSent(Arg.Any<string>()))
            .Do(arg =>
            {
                var phoneNumber = arg.Args()[0].ToString();
                if (phoneNumber != null) _smsSent.Add(phoneNumber);
            });
        _smsRateLimitRepository.GetNumberOfSmsSent(Arg.Any<string>(), Arg.Any<int>())
            .Returns(x => Task.FromResult(_smsSent.FindAll(s => s==x.Args()[0].ToString()).Count));

       _smsRateLimitRepository.GetNumberOfSmsSent(Arg.Any<int>()).Returns(x => Task.FromResult(_smsSent.Count));
        _service = new SmsRateLimitService(settingRepositories, logger, _smsRateLimitRepository);
    }

    [Test]
    public void ValidPhoneNumberTest()
    {
        Assert.True(_service.IsValidPhoneNumber("+12505550199"));
    }
    
    [Test]
    public void InValidPhoneNumberTest()
    {
        Assert.False(_service.IsValidPhoneNumber("0012505550199"));
    }
    
    [Test]
    public void InValidPhoneNumberTestMissingAreaCod()
    {
        Assert.False(_service.IsValidPhoneNumber("2505550199"));
    }
    
    [Test]
    public void InValidPhoneNumberTestLetters()
    {
        Assert.False(_service.IsValidPhoneNumber("abc"));
    }

    [Test]
    public async Task SmsSentTest()
    {
        var phoneNumber = "+12505550199";
        await _service.SmsSent(phoneNumber);
        await _smsRateLimitRepository.Received(1).SmsSent(Arg.Any<String>());
    }
    
    [Test]
    public async Task SmsSentTwiceTest()
    {
        var phoneNumber = "+12505550199";
        await _service.SmsSent(phoneNumber);
        await _service.SmsSent(phoneNumber);
        await _smsRateLimitRepository.Received(2).SmsSent(Arg.Any<String>());
    }

    [Test]
    public async Task GetSmsRateLimitCountAsyncTest()
    {
        var phoneNumber = "+12505550199";
        await _service.SmsSent(phoneNumber);
        var result = await _service.GetSmsRateLimitCountAsync(phoneNumber);
        await _smsRateLimitRepository.Received(1).GetNumberOfSmsSent(Arg.Any<String>(), Arg.Any<int>());
        Assert.That(result, Is.EqualTo(2));
    }
    
    [Test]
    public async Task GetSmsRateLimitCountFirstTimeAsyncTest()
    {
        var phoneNumber = "+12505550199";
        var result = await _service.GetSmsRateLimitCountAsync(phoneNumber);
        await _smsRateLimitRepository.Received(1).GetNumberOfSmsSent(Arg.Any<String>(), Arg.Any<int>());
        Assert.That(result, Is.EqualTo(3));
    }
    
    [Test]
    public async Task GetSmsRateLimitAsyncAsyncTest()
    {
        var phoneNumber = "+12505550199";
        await _service.SmsSent(phoneNumber);
        var result = await _service.GetSmsRateLimitAsync();
        await _smsRateLimitRepository.Received(1).GetNumberOfSmsSent(Arg.Any<int>());
        Assert.That(result, Is.EqualTo(4));
    }
}