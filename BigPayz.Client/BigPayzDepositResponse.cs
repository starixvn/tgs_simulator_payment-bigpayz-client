namespace Tgs.Payment.U2Pay.Client;
public record BigPayzDepositResponse(
    decimal Amount,
    Guid FormId,
    Uri RedirectUrl,
    string ThirdPartyOrderNo);
