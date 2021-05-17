using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tgs.Payment.PrimePay.Services
{
    public record SaveDepositReturnCommand(Guid FormId, PrimePayReturn DepositReturn);
}
