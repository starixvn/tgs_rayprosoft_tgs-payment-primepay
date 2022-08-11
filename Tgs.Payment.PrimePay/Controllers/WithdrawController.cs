using Microsoft.AspNetCore.Mvc;
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
        [FromServices] IGetService<GetWithdrawFormQuery, WithdrawFormInfo?> getWithdrawFormService,
        [FromServices] ICommandService<SaveWithdrawReturnCommand> saveReturnService,
        [FromServices] PrimePayCryptography cryptography,
        [FromServices] IGetService<GetWithdrawStateMachineQuery, StateMachine<PaymentFlowState, PaymentTrigger>> getStateMachineService,
        [FromServices] IPaymentClient paymentClient,
        [FromBody] CallbackViewModel request)
    {
        var withdrawForm = await getWithdrawFormService
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

        _logger.LogInformation("{callbackMessage}", callbackMessage);

        return new CallbackConfirmViewModel
        {
            Status = "success"
        };
    }
}
