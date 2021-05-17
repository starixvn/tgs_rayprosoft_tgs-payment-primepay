using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TGS.ServiceUtility;

namespace Tgs.Payment.PrimePay.Services
{
    class SaveDepositReturnCommandService : ICommandService<SaveDepositReturnCommand>
    {
        private readonly ITgsRepository _tgsRepository;

        public SaveDepositReturnCommandService(ITgsRepository tgsRepository)
        {
            _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));
        }
        public ValueTask ExecuteAsync(
            SaveDepositReturnCommand command,
            CancellationToken cancellationToken = default)
            => _tgsRepository.SaveDepositReturnAsync(command.FormId, command.DepositReturn);
    }
}
