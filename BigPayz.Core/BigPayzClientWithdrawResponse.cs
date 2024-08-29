namespace Tgs.Payment.U2Pay;
public record BigPayzClientWithdrawResponse(
    Guid FormId,
    string ThirdPartyOrderNo,
    decimal Amount);
