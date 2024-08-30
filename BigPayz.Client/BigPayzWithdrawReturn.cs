namespace Payment.BigPayz.Client;
public record BigPayzWithdrawReturn
{
    public string ThirdPartyOrderNo { get; set; } = default!;

    public decimal ActualAmount { get; set; }

    public Guid FormId { get; set; }

    public string Status { get; set; } = default!;

    public string Sign { get; set; } = default!;
}
