namespace SMS_Rate_Limiting.Settings;

public class SettingRepositories: ISettingRepositories
{

    public const String BusinessPhoneNumberSmsLimitKey = "BUSINESS_PHONE_NUMBER_SMS_LIMIT";
    public const String AccountSmsLimitKey = "ACCOUNT_SMS_LIMIT";
    public const String PhoneNumberActiveDurationInHoursKey = "PHONE_NUMBER_ACTIVE_DURATION_IN_HOURS";
    public const String GetDatabaseConnectionStringKey = "DB_CONNECTION_String";
    
    public int GetBusinessPhoneNumberSmsLimit()
    {
        return int.TryParse(Environment.GetEnvironmentVariable(BusinessPhoneNumberSmsLimitKey), out var businessPhoneNumberSmsLimit) ? businessPhoneNumberSmsLimit : 0;
    }

    public int GetAccountSmsLimit()
    {
        return int.TryParse(Environment.GetEnvironmentVariable(AccountSmsLimitKey), out var accountSmsLimit) ? accountSmsLimit : 0;
    }

    public int PhoneNumberActiveDurationInHours()
    {
        return int.TryParse(Environment.GetEnvironmentVariable(PhoneNumberActiveDurationInHoursKey), out var phoneNumberActiveDurationInHours) ? phoneNumberActiveDurationInHours : 0;
    }

    public String GetDatabaseConnectionString()
    {
        return Environment.GetEnvironmentVariable(GetDatabaseConnectionStringKey) ?? "";
    }
}