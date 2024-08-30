namespace Payment.BigPayz.Client;
public record BigPayzWithdrawResponse(
    Guid FormId,
    string ThirdPartyOrderNo,
    decimal MerchantBalance);
