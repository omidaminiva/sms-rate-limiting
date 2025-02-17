namespace SMS_Rate_Limiting.Services;

public interface ISmsRateLimitService
{
    public Task<int> GetSmsRateLimitCountAsync(String phoneNumber);
    
    public Task SmsSent(String phoneNumber);
    
    public Task<int> GetSmsRateLimitAsync();
    
    public bool IsValidPhoneNumber(String phoneNumber);
}