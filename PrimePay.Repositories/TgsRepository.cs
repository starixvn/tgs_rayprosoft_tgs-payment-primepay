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
    }
}
