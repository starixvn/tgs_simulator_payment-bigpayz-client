namespace Payment.BigPayz;
public record BigPayzClientDepositResponse(
    decimal Amount,
    Guid FormId,
    Uri RedirectUrl,
    string Token,
    string Currency,
    string Transaction);
