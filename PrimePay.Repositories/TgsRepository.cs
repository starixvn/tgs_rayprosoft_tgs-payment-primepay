using System;
using System.Threading.Tasks;
using Tgs.Payment.PrimePay.Repositories.Entities;

namespace Tgs.Payment.PrimePay.Repositories
{
    internal class TgsRepository : ITgsRepository
    {
        private readonly TgsContext _dbContext;
        private readonly PrimePaySerializer _serializer;

        public TgsRepository(TgsContext dbContext, PrimePaySerializer serializer)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        public async ValueTask<DepositFormInfo> GetDepositFormByIdAsync(Guid id)
        {
            var data = await _dbContext.DepositForms.FindAsync(id).ConfigureAwait(false);

            return new DepositFormInfo(
                data.Id,
                data.PlayerCode,
                data.BrandId,
                data.BrandPaymentId,
                data.Amount,
                data.Status,
                _serializer.Deserialize<PrimePayDepositApplyInfo>(data.ApplyInfo),
                data.PaymentReturn);
        }

        public async ValueTask<WithdrawFormInfo> GetWithdrawFormByIdAsync(Guid id)
        {
            var data = await _dbContext.WithdrawForms.FindAsync(id).ConfigureAwait(false);

            return new WithdrawFormInfo(
                data.Id,
                data.PlayerCode,
                data.BrandId,
                data.BrandPaymentId,
                data.Amount,
                data.Status,
                data.BankInfoId,
                data.BankInfo,
                _serializer.Deserialize<PrimePayWithdrawApplyInfo>(data.ApplyInfo),
                data.PaymentReturn);
        }

        public async ValueTask SaveDepositReturnAsync(Guid id, PrimePayReturn depositReturn)
        {
            var form = await _dbContext.DepositForms
                .FindAsync(id)
                .ConfigureAwait(false);

            form.Status = 1000;
            form.ActualAmount = depositReturn.Amount;
            form.PaymentReturn = _serializer.Serialize(depositReturn);

            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async ValueTask SaveWithdrawReturnAsync(Guid id, PrimePayReturn withdrawReturn)
        {
            var form = await _dbContext.WithdrawForms
                .FindAsync(id)
                .ConfigureAwait(false);

            form.PaymentReturn = _serializer.Serialize(withdrawReturn);

            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
