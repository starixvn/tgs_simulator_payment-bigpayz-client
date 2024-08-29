using System.Text.Json;
using Microsoft.Extensions.Options;
using Tgs.Payment.Clients;
using Tgs.Payment.Protos;
using Tgs.Payment.U2Pay.Client;
using Valhalla.MessageQueue;

namespace Tgs.Payment.U2Pay.MessageQueue;

internal class WithdrawProcessor(
    BigPayzClient u2PayClient,
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
            decimal.Parse(data.Amount) * settings.ExchangeRate,
            new Uri(_options.CallbackBaseUrl,"/withdraw/callback"),
            settings.ReceiverName,
            settings.BankName,
            settings.BankAccount);

        var withdrawResponse = await u2PayClient.WithdrawAsync(
            settings.MerchantNo,
            settings.AppSecret,
            settings.PrivateKey,
            order,
            cancellationToken).ConfigureAwait(false);

        var applyInfo = new BigPayzWithdrawApplyInfo(
            settings,
            new U2PayWithdrawResponse(withdrawResponse.FormId, withdrawResponse.ThirdPartyOrderNo));

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
