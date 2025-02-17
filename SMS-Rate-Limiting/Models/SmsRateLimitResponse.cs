namespace SMS_Rate_Limiting.Models;

public class SmsRateLimitResponse
{
    public int RemainingSmsMessagesPerPhoneNumber { get; set; }
    
    public int RemainingSmsMessagesForAccount { get; set; }
    
}