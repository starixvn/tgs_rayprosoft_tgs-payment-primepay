using System;

namespace Tgs.Payment.PrimePay
{
    public abstract class PrimePayReturn
    {
        public decimal Amount { get; init; }

        public string Currency { get; init; } = default!;

        public string ReferenceId { get; init; } = default!;

        public string? Remark { get; init; }

        public DateTimeOffset RequestDate { get; init; }

        public TransactionStatus Status { get; init; }

        public string StatusDesc { get; init; } = default!;

        public DateTimeOffset TransactionDate { get; init; }

        public string TransactionType { get; init; } = default!;

        public string TransactionTypeDesc { get; init; } = default!;
    }
}
