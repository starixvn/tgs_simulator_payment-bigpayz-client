using System.Text.Json;
using Microsoft.Extensions.Options;
using Tgs.Payment.Clients;
using Tgs.Payment.Protos;
using Payment.BigPayz.Client;
using Valhalla.MessageQueue;

namespace Payment.BigPayz.MessageQueue;

internal class WithdrawProcessor(
    BigPayzClient bigPayzClient,
    IPaymentClient paymentClient,
    IOptions<BigPayzOptions> optionsAccessor) : IMessageProcessor<WithdrawRequest, WithdrawResponse>
{
    private readonly BigPayzOptions _options = optionsAccessor.Value;

    public async ValueTask<WithdrawResponse> HandleAsync(
        string subject,
        WithdrawRequest data,
        IEnumerable<MessageHeaderValue>? headerValues,
        CancellationToken cancellationToken = default)
    {
        var settings = JsonSerializer.Deserialize<BigPayzWithdrawSettings>(data.ApplyInfo, BigPayzUtility.SerializerOptions)!;

        var order = new WithdrawOrder(
            settings.FormId,
            decimal.Parse(data.Amount),
            new Uri(_options.CallbackBaseUrl,"withdraw/callback"),
            settings.ReceiverName,
            settings.BankName,
            settings.BankAccount,
            settings.BankCode,
            settings.PlayerIp,
            settings.CurrencyCode);

        var withdrawResponse = await bigPayzClient.WithdrawAsync(
            settings.MerchantCode,
            settings.ApiKey,
            order,
            cancellationToken).ConfigureAwait(false);

        var applyInfo = new BigPayzWithdrawApplyInfo(
            settings,
            new BigPayzWithdrawResponse(withdrawResponse.FormId, withdrawResponse.ThirdPartyOrderNo, withdrawResponse.MerchantBalance));

        await paymentClient.SaveWithdrawFormThirdPartyOrderNoAsync(
            settings.FormId,
            withdrawResponse.ThirdPartyOrderNo,
            cancellationToken);

        return new WithdrawResponse
        {
            Result = JsonSerializer.Serialize(applyInfo, BigPayzUtility.SerializerOptions)
        };
    }
}
