namespace SMS_Rate_Limiting.Repositories;

public interface ISmsRateLimitRepository
{
    public Task SmsSent(String phoneNumber);
    
    public Task<int> GetNumberOfSmsSent(String phoneNumber, int interval);

    public Task<int> GetNumberOfSmsSent(int interval);
}