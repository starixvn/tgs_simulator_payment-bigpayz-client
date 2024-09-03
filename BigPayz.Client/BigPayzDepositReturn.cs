namespace Payment.BigPayz.Client;
public record BigPayzDepositReturn
{
    public string Transaction { get; set; } = default!;

    public decimal OrderAmount { get; set; }

    public decimal RealAmount { get; set; }

    public Guid FormId { get; set; }

    public string Status { get; set; } = default!;

    public string Sign { get; set; } = default!;

    public string ExtendData { get; set; } = default!;
}
