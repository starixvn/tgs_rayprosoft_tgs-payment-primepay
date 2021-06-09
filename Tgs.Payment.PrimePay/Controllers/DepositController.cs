using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tgs.Payment.PrimePay.Services;
using Tgs.Payment.PrimePay.ViewModels;
using TGS.ServiceUtility;

namespace Tgs.Payment.PrimePay.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DepositController : ControllerBase
    {
        private readonly ILogger<DepositController> _logger;

        public DepositController(ILogger<DepositController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("callback")]
        public async Task<CallbackConfirmViewModel> DepositCallbackAsync(
            [FromServices] IOptions<JsonOptions> jsonOptionsAccessor,
            [FromServices] IGetService<GetDepositFormQuery, DepositFormInfo> getDepositFormService,
            [FromServices] ICommandService<SaveDepositReturnCommand> saveReturnService,
            [FromServices] ICommandService<ApproveDepositFormCommand> approveService,
            [FromServices] PrimePayCryptography cryptography,
            [FromBody] CallbackViewModel request)
        {
            var jsonSerializerOptions = jsonOptionsAccessor.Value.JsonSerializerOptions;

            var depositForm = await getDepositFormService
                .GetAsync(new GetDepositFormQuery(request.TransactionId))
                .ConfigureAwait(false);

            var callbackMessage = cryptography.Decrypt<PrimePayReturn>(
                depositForm.ApplyInfo!.Settings.Key,
                depositForm.ApplyInfo!.Settings.IV,
                request.Message)!;

            await saveReturnService
                .ExecuteAsync(new SaveDepositReturnCommand(depositForm.Id, callbackMessage))
                .ConfigureAwait(false);

            _logger.LogInformation(JsonSerializer.Serialize(callbackMessage, jsonSerializerOptions));

            var approveCommand = new ApproveDepositFormCommand(depositForm.Id, depositForm.BrandId);

            await approveService.ExecuteAsync(approveCommand).ConfigureAwait(false);

            return new CallbackConfirmViewModel
            {
                Status = "success"
            };
        }
    }
}
