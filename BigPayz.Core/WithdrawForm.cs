using Tgs.Payment.Clients;
using Payment.BigPayz.Client;

namespace Payment.BigPayz;
public record WithdrawForm(
    Guid Id,
    string? UserId,
    UserType UserType,
    Guid BrandId,
    Guid? BrandPaymentId,
    decimal Amount,
    PaymentFlowState Status,
    Guid BankInfoId,
    BigPayzWithdrawApplyInfo? ApplyInfo,
    string? PaymentReturn = null);
