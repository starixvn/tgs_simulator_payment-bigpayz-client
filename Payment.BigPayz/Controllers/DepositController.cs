using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Tgs.Payment.Clients;
using Payment.BigPayz.ApplicationServices;
using Payment.BigPayz.Client;
using Payment.BigPayz.ViewModels;
using Tgs.ServiceUtility;

namespace Payment.BigPayz.Controllers;

[Route("[controller]")]
[ApiController]
public class DepositController : ControllerBase
{
    private readonly ILogger<DepositController> _logger;

    public DepositController(ILogger<DepositController> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpPost("callback", Name = "NotifyDeposit")]
    [Consumes("application/x-www-form-urlencoded")]
    public async Task<string> DepositCallbackAsync(
    [FromServices] BigPayzHasher hasher,
        [FromServices] IGetService<GetDepositFormQuery, DepositForm?> getService,
        [FromServices] IPaymentServiceClient paymentServiceClient,
        [FromServices] IPaymentClient paymentClient,
        [FromForm] CallbackDepositViewModel source)
    {
        var formId = Guid.Parse(source.ItemId);

        var depositForm = await getService
            .GetAsync(new(formId), HttpContext.RequestAborted)
            .ConfigureAwait(false);

        if (depositForm == null)
        {
            _logger.LogWarning("Can't found deposit form({id}).", formId);
            return "fail";
        }

        var sign = source.Signature;
        var hash = hasher.ComputeHash(
            depositForm.ApplyInfo!.Settings.ApiKey,
            [source.Transaction, source.Status.ToString(), source.Amount.ToString(), source.CreatedAt]);

        _logger.LogInformation("Compute hash: {hash}, sign: {sign}", hash, sign);

        if (sign != hash)
        {
            _logger.LogWarning("Sign value invalid. input: {sign}, compute: {hash}", sign, hash);
            return "fail";
        }

        var paymentReturn = new BigPayzDepositReturn()
        {
            Status = source.StatusMessage,
            FormId = formId,
            Transaction = source.Transaction,
            RealAmount = source.Amount,
            OrderAmount = depositForm.Amount,
            Sign = sign,
            ClientName = source.ClientName,
            ItemDescription = source.ItemDescription
        };

        var fsm = PaymentStateMachineFactory.CreateDepositStateMachine(depositForm.Status);

        switch (paymentReturn.Status)
        {
            case "Success":
                if (fsm.CanFire(PaymentTrigger.PaymentDepositCompleted))
                {
                    await fsm.FireAsync(PaymentTrigger.PaymentDepositCompleted).ConfigureAwait(false);

                    await paymentServiceClient.SubmitDepositFormStatusAsync(
                        depositForm.Id,
                        paymentReturn: JsonSerializer.Serialize(
                            paymentReturn,
                            BigPayzUtility.SerializerOptions),
                        actualAmount: source.Amount,
                        handlingFee: depositForm.Amount - source.Amount, 
                        cancellationToken: HttpContext.RequestAborted).ConfigureAwait(false);

                    await paymentClient.ApproveDepositAsync(
                        depositForm.Id,
                        depositForm.BrandId,
                        paymentReturn.RealAmount,
                        HttpContext.RequestAborted).ConfigureAwait(false);
                }

                break;

            default:
                _logger.LogWarning("FormId:{formId}, ThirdPartyOrderNo:{ThirdPartyOrderNo} , payment Status:{status}",
                    formId,
                    paymentReturn.Transaction,
                    paymentReturn.Status);
                return "fail";
        }

        return "success";
    }
}
