using System.ComponentModel;

namespace Payment.BigPayz.Client;
public record BigPayzDepositSettings(
    string MerchantCode,
    string Currency,
    string ApiKey,
    Guid FormId,
    string PlayerId,
    string ItemDescription);
