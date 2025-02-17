namespace SMS_Rate_Limiting.Settings;

public interface ISettingRepositories
{
    public int GetBusinessPhoneNumberSmsLimit();

    public int GetAccountSmsLimit();

    public int PhoneNumberActiveDurationInHours();
    
    public String GetDatabaseConnectionString();
}