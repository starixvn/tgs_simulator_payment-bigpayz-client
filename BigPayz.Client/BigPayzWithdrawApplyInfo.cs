namespace Tgs.Payment.U2Pay.Client;
public record BigPayzWithdrawApplyInfo(
    BigPayzWithdrawSettings Settings,
    U2PayWithdrawResponse Response);