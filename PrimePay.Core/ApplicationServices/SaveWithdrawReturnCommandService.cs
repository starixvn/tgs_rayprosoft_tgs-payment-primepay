using Tgs.Payment.Clients;
using Tgs.ServiceUtility;

namespace Tgs.Payment.PrimePay.Services;

internal class SaveWithdrawReturnCommandService : ICommandService<SaveWithdrawReturnCommand>
{
    private readonly IPaymentServiceClient _paymentServiceClient;
    private readonly PrimePaySerializer _primePaySerializer;

    public SaveWithdrawReturnCommandService(
        IPaymentServiceClient paymentServiceClient,
        PrimePaySerializer primePaySerializer)
    {
        _paymentServiceClient = paymentServiceClient ?? throw new ArgumentNullException(nameof(paymentServiceClient));
        _primePaySerializer = primePaySerializer ?? throw new ArgumentNullException(nameof(primePaySerializer));
    }

    public ValueTask ExecuteAsync(SaveWithdrawReturnCommand command, CancellationToken cancellationToken = default)
        => _paymentServiceClient.SubmitWithdrawFormStatusAsync(
            command.FormId,
            null,
            _primePaySerializer.Serialize(command.WithdrawReturn),
            command.ActualAmount);
}
