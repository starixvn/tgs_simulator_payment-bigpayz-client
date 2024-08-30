using System.Text.Json;
using Microsoft.Extensions.Options;
using Tgs.Payment.Clients;
using Tgs.Payment.Protos;
using Payment.BigPayz.Client;
using Valhalla.MessageQueue;

namespace Payment.BigPayz.MessageQueue;

internal class DepositProcessor(
    BigPayzClient BigPayzClient,
    IPaymentClient paymentClient,
    IOptions<BigPayzOptions> optionsAccessor)
    : IMessageProcessor<DepositRequest, DepositResponse>
{
    private readonly BigPayzOptions _options = optionsAccessor.Value;

    public async ValueTask<DepositResponse> HandleAsync(
        string subject,
        DepositRequest data,
        IEnumerable<MessageHeaderValue>? headerValues,
        CancellationToken cancellationToken = default)
    {
        var settings = JsonSerializer.Deserialize<BigPayzDepositSettings>(
            data.ApplyInfo,
            BigPayzUtility.SerializerOptions)!;

        var order = new DepositOrder(
            decimal.Parse(data.Amount),
            new Uri(_options.CallbackBaseUrl,"/deposit/callback"),
            settings.FormId,
            settings.PlayerId,
            settings.ItemDescription);

        var depositResponse = await BigPayzClient.DepositAsync(
            settings,
            order,
            cancellationToken).ConfigureAwait(false);

        var applyInfo = new BigPayzDepositApplyInfo(
            settings,
            new BigPayzDepositResponse(
                depositResponse.Amount,
                depositResponse.FormId,
                depositResponse.RedirectUrl,
                depositResponse.Transaction,
                depositResponse.Token,
                depositResponse.Currency));

        await paymentClient.ScheduleAsync(
            "tgs.payment.bigpayz.deposit.schedule.reject",
            settings.FormId,
            DateTime.UtcNow.AddMinutes(60D),
            cancellationToken).ConfigureAwait(false);

        return new DepositResponse
        {
            Result = JsonSerializer.Serialize(applyInfo, BigPayzUtility.SerializerOptions)
        };
    }
}
