using System;

namespace Tgs.Payment.PrimePay
{
    public class AdjustmentInfo
    {
        public DateTimeOffset AdjustedDate { get; init; }

        public string AdjustedReason { get; init; } = default!;

        public decimal? ServiceFeeBeforeAdjusted { get; init; }

        public decimal? TransactionFeeBeforeAdjusted { get; init; }
    }
}
