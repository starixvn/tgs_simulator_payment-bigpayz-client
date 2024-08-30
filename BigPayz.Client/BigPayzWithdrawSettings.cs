using System.ComponentModel;

namespace Payment.BigPayz.Client;
public record BigPayzWithdrawSettings(
    string MerchantCode,
    string ApiKey,
    Guid FormId,
    string ReceiverName,
    string BankName,
    string BankAccount,
    string BankCode,
    string PlayerIp,
    string CurrencyCode);
