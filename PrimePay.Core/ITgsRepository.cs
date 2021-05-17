using System;
using System.Threading.Tasks;

namespace Tgs.Payment.PrimePay
{
    public interface ITgsRepository
    {
        ValueTask<DepositFormInfo> GetDepositFormByIdAsync(Guid id);

        ValueTask<WithdrawFormInfo> GetWithdrawFormByIdAsync(Guid id);

        ValueTask SaveDepositReturnAsync(Guid id, PrimePayReturn depositReturn);

        ValueTask SaveWithdrawReturnAsync(Guid id, PrimePayReturn withdrawReturn);
    }
}
