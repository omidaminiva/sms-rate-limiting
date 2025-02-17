using PhoneNumbers;
using SMS_Rate_Limiting.Repositories;
using SMS_Rate_Limiting.Settings;

namespace SMS_Rate_Limiting.Services;

public class SmsRateLimitService(
    ISettingRepositories settingRepositories,
    ILogger<SmsRateLimitService> logger,
    ISmsRateLimitRepository smsRateLimitRepository)
    : ISmsRateLimitService
{
    private readonly ILogger<SmsRateLimitService> _logger = logger;

    public bool IsValidPhoneNumber(string phoneNumber)
    {
        var phoneNumberUtil = PhoneNumberUtil.GetInstance();
        PhoneNumber number;
        try
        {
            number = phoneNumberUtil.Parse(phoneNumber, null);
        }
        catch (NumberParseException  e)
        {
            _logger.LogError(e, "Failed to parse phone number");
            return false;
        }
        return phoneNumberUtil.IsValidNumber(number);
    }

    private String StandardizeThePhoneNumber(String phoneNumber)
    {
        var phoneNumberUtil = PhoneNumberUtil.GetInstance();
        var number = phoneNumberUtil.Parse(phoneNumber, null);
        return phoneNumberUtil.Format(number, PhoneNumberFormat.E164);
    }

    public async Task<int> GetSmsRateLimitCountAsync(String phoneNumber)
    {
        phoneNumber = StandardizeThePhoneNumber(phoneNumber);
        var interval = settingRepositories.PhoneNumberActiveDurationInHours();
        var numberOfSmsSentPerPhoneNumber = await smsRateLimitRepository.GetNumberOfSmsSent(phoneNumber, interval);
        var numberOfSmsSentPerPhoneNumberLimit = settingRepositories.GetBusinessPhoneNumberSmsLimit();
        return numberOfSmsSentPerPhoneNumberLimit - numberOfSmsSentPerPhoneNumber;
    }

    public async Task<int> GetSmsRateLimitAsync()
    {
        var interval = settingRepositories.PhoneNumberActiveDurationInHours();
        var numberOfSmsSent =  await smsRateLimitRepository.GetNumberOfSmsSent(interval);
        var numberOfSmsLimit = settingRepositories.GetAccountSmsLimit();
        return numberOfSmsLimit - numberOfSmsSent;
    }

    public async Task SmsSent(string phoneNumber)
    {
        phoneNumber = StandardizeThePhoneNumber(phoneNumber);
        await smsRateLimitRepository.SmsSent(phoneNumber);
    }
}