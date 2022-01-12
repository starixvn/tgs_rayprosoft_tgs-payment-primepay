using Stateless;
using Tgs.Payment.Clients;
using Tgs.ServiceUtility;

namespace Tgs.Payment.PrimePay.ApplicationServices;

internal class GetWithdrawStateMachineQueryService : IGetService<GetWithdrawStateMachineQuery, StateMachine<PaymentFlowState, PaymentTrigger>>
{
    private readonly IPaymentServiceClient _paymentServiceClient;

    public GetWithdrawStateMachineQueryService(IPaymentServiceClient paymentServiceClient)
    {
        _paymentServiceClient = paymentServiceClient ?? throw new ArgumentNullException(nameof(paymentServiceClient));
    }

    public ValueTask<StateMachine<PaymentFlowState, PaymentTrigger>> GetAsync(
        GetWithdrawStateMachineQuery query,
        CancellationToken cancellationToken = default)
    {
        return ValueTask.FromResult(_paymentServiceClient.CreateWithdrawStateMachine(query.InitialState));
    }
}
