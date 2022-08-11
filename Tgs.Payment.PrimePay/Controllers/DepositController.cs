using Microsoft.AspNetCore.Mvc;
using Tgs.Payment.Clients;
using Tgs.Payment.PrimePay.Services;
using Tgs.Payment.PrimePay.ViewModels;
using Tgs.ServiceUtility;

namespace Tgs.Payment.PrimePay.Controllers;

[Route("[controller]")]
[ApiController]
public class DepositController : ControllerBase
{
    private readonly ILogger<DepositController> _logger;

    public DepositController(ILogger<DepositController> logger) => _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    [HttpPost("callback")]
    public async Task<CallbackConfirmViewModel> DepositCallbackAsync(
        [FromServices] IGetService<GetDepositFormQuery, DepositFormInfo?> getDepositFormService,
        [FromServices] ICommandService<SaveDepositReturnCommand> saveReturnService,
        [FromServices] ICommandService<ApproveDepositFormCommand> approveService,
        [FromServices] PrimePayCryptography cryptography,
        [FromBody] CallbackViewModel request)
    {
        var depositForm = await getDepositFormService
            .GetAsync(new GetDepositFormQuery(request.TransactionId))
            .ConfigureAwait(false);

        if (depositForm != null && depositForm.Status < PaymentFlowState.WaitApprove)
        {
            var callbackMessage = cryptography.Decrypt<PrimePayReturn>(
                depositForm.ApplyInfo!.Settings.Key,
                depositForm.ApplyInfo!.Settings.IV,
                request.Message)!;

            await saveReturnService
                .ExecuteAsync(new SaveDepositReturnCommand(depositForm.Id, callbackMessage))
                .ConfigureAwait(false);

            _logger.LogInformation("{callbackMessage}", callbackMessage);

            if (callbackMessage.Status == TransactionStatus.Completed)
            {
                var approveCommand = new ApproveDepositFormCommand(
                    depositForm.Id,
                    depositForm.BrandId,
                    ((PrimePayDepositCompletedReturn)callbackMessage).TransactionAmount);

                await approveService
                    .ExecuteAsync(approveCommand)
                    .ConfigureAwait(false);
            }
        }

        return new CallbackConfirmViewModel
        {
            Status = "success"
        };
    }
}
