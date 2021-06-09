using System;

namespace Tgs.Payment.PrimePay.Services
{
    public record ApproveDepositFormCommand(Guid FormId, Guid BrandId);
}
