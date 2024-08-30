namespace Payment.BigPayz.Client;
public record BigPayzDepositResponse(
    decimal Amount,
    Guid FormId,
    Uri RedirectUrl,
    string Transaction,
    string Token,
    string Currency);
