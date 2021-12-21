using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tgs.Payment.PrimePay.Services;
using Tgs.Payment.PrimePay.ViewModels;
using Tgs.ServiceUtility;

namespace Tgs.Payment.PrimePay.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class WithdrawController : ControllerBase
    {
        private readonly ILogger<WithdrawController> _logger;

        public WithdrawController(ILogger<WithdrawController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("callback")]
        public async Task<CallbackConfirmViewModel> WithdrawCallbackAsync(
            [FromServices] IOptions<JsonOptions> jsonOptionsAccessor,
            [FromServices] IGetService<GetWithdrawFormQuery, WithdrawFormInfo?> getDepositFormService,
            [FromServices] ICommandService<SaveWithdrawReturnCommand> saveReturnService,
            [FromServices] PrimePayCryptography cryptography,
            [FromBody] CallbackViewModel request)
        {
            var jsonSerializerOptions = jsonOptionsAccessor.Value.JsonSerializerOptions;

            var withdrawForm = await getDepositFormService
                .GetAsync(new GetWithdrawFormQuery(request.TransactionId))
                .ConfigureAwait(false);

            if (withdrawForm == null)
                return new CallbackConfirmViewModel
                {
                    Status = "fail",
                    Message = "Can't find withdraw form."
                };

            var callbackMessage = cryptography.Decrypt<PrimePayReturn>(
                withdrawForm.ApplyInfo!.Settings.Key,
                withdrawForm.ApplyInfo!.Settings.IV,
                request.Message)!;

            if (callbackMessage.Status == TransactionStatus.Completed)
            {
                var formStatus = withdrawForm.Status;
                if (withdrawForm.Status == Clients.PaymentFlowState.Approved)
                    formStatus = Clients.PaymentFlowState.Paid;

                await saveReturnService
                    .ExecuteAsync(new SaveWithdrawReturnCommand(
                        withdrawForm.Id,
                        formStatus,
                        callbackMessage,
                        ((PrimePayWithdrawCompletedReturn)callbackMessage).TransactionAmount))
                    .ConfigureAwait(false);
            }
            else
            {
                await saveReturnService
                    .ExecuteAsync(new SaveWithdrawReturnCommand(
                        withdrawForm.Id,
                        withdrawForm.Status,
                        callbackMessage,
                        null))
                    .ConfigureAwait(false);
            }

            _logger.LogInformation(JsonSerializer.Serialize(callbackMessage, jsonSerializerOptions));

            return new CallbackConfirmViewModel
            {
                Status = "success"
            };
        }
    }
}
