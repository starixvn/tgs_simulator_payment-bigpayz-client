namespace Tgs.Payment.U2Pay;
public record BigPayzClientDepositResponse(
    decimal Amount,
    Guid FormId,
    string ThirdPartyOrderNo,
    Uri RedirectUrl);
