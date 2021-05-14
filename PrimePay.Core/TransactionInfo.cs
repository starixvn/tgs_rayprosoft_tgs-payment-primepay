using System;

namespace Tgs.Payment.PrimePay
{
    internal abstract class TransactionInfo
    {
        public string Currency { get; init; } = default!;

        public string ReferenceId { get; init; } = default!;

        public string Remark { get; init; } = default!;

        public DateTimeOffset RequestDate { get; init; }

        public TransactionStatus Status { get; init; }

        public string StatusDesc { get; init; } = default!;

        public DateTimeOffset TransactionDate { get; init; }

        public string TransactionId { get; init; } = default!;

        public string TransactionType { get; init; } = default!;

        public string TransactionTypeDesc { get; init; } = default!;
    }
}
