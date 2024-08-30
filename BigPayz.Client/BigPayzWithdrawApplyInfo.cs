namespace Payment.BigPayz.Client;
public record BigPayzWithdrawApplyInfo(
    BigPayzWithdrawSettings Settings,
    BigPayzWithdrawResponse Response);