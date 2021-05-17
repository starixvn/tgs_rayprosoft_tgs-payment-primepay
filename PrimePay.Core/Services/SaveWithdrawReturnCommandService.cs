using System;
using System.Threading;
using System.Threading.Tasks;
using TGS.ServiceUtility;

namespace Tgs.Payment.PrimePay.Services
{
    internal class SaveWithdrawReturnCommandService : ICommandService<SaveWithdrawReturnCommand>
    {
        private readonly ITgsRepository _tgsRepository;

        public SaveWithdrawReturnCommandService(ITgsRepository tgsRepository)
        {
            _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));
        }

        public ValueTask ExecuteAsync(SaveWithdrawReturnCommand command, CancellationToken cancellationToken = default)
            => _tgsRepository.SaveWithdrawReturnAsync(command.FormId, command.WithdrawReturn);
    }
}
