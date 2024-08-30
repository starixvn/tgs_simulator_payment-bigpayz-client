namespace Payment.BigPayz;
public record BigPayzClientWithdrawResponse(
    Guid FormId,
    string ThirdPartyOrderNo,
    decimal MerchantBalance);
