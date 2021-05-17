using System.Collections.Generic;
using System.Text.Json.Serialization;
using Tgs.Payment.PrimePay.Converters;

namespace System.Text.Json
{
    public static class JsonConverterExtensions
    {
        public static void AddPrimePayConverters(this IList<JsonConverter> converters)
        {
            converters.Add(new DecimalJsonConverter());
            converters.Add(new NullableDateTimeOffsetJsonConverter());
            converters.Add(new DateTimeOffsetJsonConverter());
            converters.Add(new TransactionInfoJsonConverter());
            converters.Add(new PrimePayReturnJsonConverter());
            converters.Add(new PrimePayDepositResponseJsonConverter());
            converters.Add(new PrimePayWithdrawResponseJsonConverter());
        }
    }
}
