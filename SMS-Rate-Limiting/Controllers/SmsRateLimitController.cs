using Microsoft.AspNetCore.Mvc;
using SMS_Rate_Limiting.Models;
using SMS_Rate_Limiting.Services;

namespace SMS_Rate_Limiting.Controllers;

[ApiController]
[Route("v1/smsRateLimit")]
public class SmsRateLimitController(
    ISmsRateLimitService smsRateLimitService,
    ILogger<SmsRateLimitController> logger)
    : ControllerBase
{
    // to get number of sms messages left per phonenumber and per account 
    [HttpGet("phoneNumber/{phoneNumber}")]
    public async Task<ActionResult<SmsRateLimitResponse>> GetSmsLimits(String phoneNumber)
    {
        if (!smsRateLimitService.IsValidPhoneNumber(phoneNumber))
        {
            return BadRequest("Invalid phone number");
        }

        var numberOfMessagesLeftPerPhoneNumber
            = await smsRateLimitService.GetSmsRateLimitCountAsync(phoneNumber);
        var numberOfMessagesLeftPerAccount
            = await smsRateLimitService.GetSmsRateLimitAsync();
        logger.LogInformation($"Number: {phoneNumber}");
        return Ok(new SmsRateLimitResponse
        {
            RemainingSmsMessagesPerPhoneNumber = numberOfMessagesLeftPerPhoneNumber,
            RemainingSmsMessagesForAccount = numberOfMessagesLeftPerAccount
        });
    }

    [HttpPost]
    public async Task<ActionResult> SmsSent([FromBody] SmsSentRequest phoneNumber)
    {
        if (!smsRateLimitService.IsValidPhoneNumber(phoneNumber.PhoneNumber))
        {
            return BadRequest("Invalid phone number");
        }
        var numberOfMessagesLeftPerPhoneNumber
            = await smsRateLimitService.GetSmsRateLimitCountAsync(phoneNumber.PhoneNumber);
        var numberOfMessagesLeftPerAccount
            = await smsRateLimitService.GetSmsRateLimitAsync();
        if (numberOfMessagesLeftPerPhoneNumber < 1 || numberOfMessagesLeftPerAccount < 1)
        {
            return BadRequest("Limit has been reached");
        }

        await smsRateLimitService.SmsSent(phoneNumber.PhoneNumber);
        return Ok();
    }
}