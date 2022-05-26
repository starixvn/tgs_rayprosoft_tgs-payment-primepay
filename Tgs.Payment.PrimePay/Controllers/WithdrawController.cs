using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stateless;
using Tgs.Payment.Clients;
using Tgs.Payment.PrimePay.ApplicationServices;
using Tgs.Payment.PrimePay.Services;
using Tgs.Payment.PrimePay.ViewModels;
using Tgs.ServiceUtility;

namespace Tgs.Payment.PrimePay.Controllers;

[Route("[controller]")]
[ApiController]
public class WithdrawController : ControllerBase
{
    private readonly ILogger<WithdrawController> _logger;

    public WithdrawController(ILogger<WithdrawController> logger) => _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    [HttpPost("callback")]
    public async Task<CallbackConfirmViewModel> WithdrawCallbackAsync(
        [FromServices] IOptions<JsonOptions> jsonOptionsAccessor,
        [FromServices] IGetService<GetWithdrawFormQuery, WithdrawFormInfo?> getDepositFormService,
        [FromServices] ICommandService<SaveWithdrawReturnCommand> saveReturnService,
        [FromServices] PrimePayCryptography cryptography,
        [FromServices] IGetService<GetWithdrawStateMachineQuery, StateMachine<PaymentFlowState, PaymentTrigger>> getStateMachineService,
        [FromServices] IPaymentClient paymentClient,
        [FromBody] CallbackViewModel request)
    {
        var jsonSerializerOptions = jsonOptionsAccessor.Value.JsonSerializerOptions;

        var withdrawForm = await getDepositFormService
            .GetAsync(new GetWithdrawFormQuery(request.TransactionId))
            .ConfigureAwait(false);

        if (withdrawForm == null)
        {
            return new CallbackConfirmViewModel
            {
                Status = "fail",
                Message = "Can't find withdraw form."
            };
        }

        var callbackMessage = cryptography.Decrypt<PrimePayReturn>(
            withdrawForm.ApplyInfo!.Settings.Key,
            withdrawForm.ApplyInfo!.Settings.IV,
            request.Message)!;

        var fsm = await getStateMachineService
            .GetAsync(new GetWithdrawStateMachineQuery(withdrawForm.Status))
            .ConfigureAwait(false);

        switch (callbackMessage.Status)
        {
            case TransactionStatus.Completed:

                if (fsm.CanFire(PaymentTrigger.PaymentWithdrawCompleted))
                    await fsm.FireAsync(PaymentTrigger.PaymentWithdrawCompleted).ConfigureAwait(false);

                await saveReturnService
                    .ExecuteAsync(new SaveWithdrawReturnCommand(
                        withdrawForm.Id,
                        callbackMessage,
                        ((PrimePayWithdrawCompletedReturn)callbackMessage).TransactionAmount))
                    .ConfigureAwait(false);

                await paymentClient
                    .PaidWithdrawAsync(
                        withdrawForm.Id,
                        withdrawForm.BrandId,
                        HttpContext.RequestAborted)
                    .ConfigureAwait(false);
                break;

            case TransactionStatus.Rejected:
                await saveReturnService
                    .ExecuteAsync(new SaveWithdrawReturnCommand(
                        withdrawForm.Id,
                        callbackMessage,
                        null))
                    .ConfigureAwait(false);

                if (fsm.CanFire(PaymentTrigger.Reject))
                    await paymentClient
                        .RejectWithdrawAsync(withdrawForm.Id, withdrawForm.BrandId)
                        .ConfigureAwait(false);
                break;

            default:
                await saveReturnService
                    .ExecuteAsync(new SaveWithdrawReturnCommand(
                        withdrawForm.Id,
                        callbackMessage,
                        null))
                    .ConfigureAwait(false);
                break;
        }

        _logger.LogInformation(JsonSerializer.Serialize(callbackMessage, jsonSerializerOptions));

        return new CallbackConfirmViewModel
        {
            Status = "success"
        };
    }
}
