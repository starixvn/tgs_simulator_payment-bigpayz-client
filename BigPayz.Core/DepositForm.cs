using Tgs.Payment.Clients;
using Payment.BigPayz.Client;

namespace Payment.BigPayz;
public record DepositForm(
    Guid Id,
    string PlayerCode,
    Guid BrandId,
    Guid BrandPaymentId,
    decimal Amount,
    PaymentFlowState Status,
    BigPayzDepositApplyInfo? ApplyInfo,
    string? PaymentReturn = null);
