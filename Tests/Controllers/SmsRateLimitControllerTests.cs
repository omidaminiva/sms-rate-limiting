using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using SMS_Rate_Limiting.Controllers;
using SMS_Rate_Limiting.Models;
using SMS_Rate_Limiting.Services;

namespace Tests.Controllers;

[TestFixture]
public class SmsRateLimitControllerTests
{
    
    private SmsRateLimitController _controller;
    private ISmsRateLimitService _service;

    [SetUp]
    public void Setup()
    {
        ILogger<SmsRateLimitController> logger = Substitute.For<ILogger<SmsRateLimitController>>();
        _service = Substitute.For<ISmsRateLimitService>();
        _controller = new SmsRateLimitController(_service, logger);
    }

    [Test]
    public async Task SmsSentTest()
    {
        var phoneNumber = "+12505550199";
        _service.IsValidPhoneNumber(phoneNumber).Returns(true);
        _service.GetSmsRateLimitCountAsync(phoneNumber).Returns(Task.FromResult(1));
        _service.GetSmsRateLimitAsync().Returns(Task.FromResult(1));
        var result = await _controller.SmsSent(new SmsSentRequest {PhoneNumber = phoneNumber});
        Assert.That(result, Is.InstanceOf<OkResult>());
    }
    
    [Test]
    public async Task SmsSentTestBadRequest()
    {
        var phoneNumber = "+abcd";
        _service.IsValidPhoneNumber(phoneNumber).Returns(false);
        _service.GetSmsRateLimitCountAsync(phoneNumber).Returns(Task.FromResult(1));
        _service.GetSmsRateLimitAsync().Returns(Task.FromResult(1));
        var result = await _controller.SmsSent(new SmsSentRequest {PhoneNumber = phoneNumber});
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
    }
    
    [Test]
    public async Task SmsSentTestBadRequestPhoneNumberLimit()
    {
        var phoneNumber = "+12505550199";
        _service.IsValidPhoneNumber(phoneNumber).Returns(true);
        _service.GetSmsRateLimitCountAsync(phoneNumber).Returns(Task.FromResult(0));
        var result = await _controller.SmsSent(new SmsSentRequest {PhoneNumber = phoneNumber});
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
    }
    
    [Test]
    public async Task SmsSentTestBadRequestAccountLimit()
    {
        var phoneNumber = "+12505550199";
        _service.IsValidPhoneNumber(phoneNumber).Returns(true);
        _service.GetSmsRateLimitAsync().Returns(Task.FromResult(0));
        var result = await _controller.SmsSent(new SmsSentRequest {PhoneNumber = phoneNumber});
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
    }
}