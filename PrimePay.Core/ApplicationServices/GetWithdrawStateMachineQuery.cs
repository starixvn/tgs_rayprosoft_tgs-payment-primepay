using Tgs.Payment.Clients;

namespace Tgs.Payment.PrimePay.ApplicationServices;

public record GetWithdrawStateMachineQuery(PaymentFlowState InitialState);
