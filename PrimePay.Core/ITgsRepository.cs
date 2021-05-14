using System;
using System.Threading.Tasks;

namespace Tgs.Payment.PrimePay
{
    public interface ITgsRepository
    {
        ValueTask<DepositFormInfo> GetDepositFormByIdAsync(Guid id);

        //ValueTask<WithdrawFormInfo> GetWithdrawFormByIdAsync(Guid id);

        //ValueTask SaveDepositReturnAsync(Guid id, LeePayDepositReturn depositReturn);

        //ValueTask SaveWithdrawReturnAsync(Guid id, LeePayWithdrawReturn withdrawReturn);
    }
}
