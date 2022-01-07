using System.Diagnostics;

namespace Tgs.Payment.PrimePay;

internal static class GuidHelper
{
    public static string GuidToString(Guid id)
    {
        var byteArray = id.ToByteArray();

        var spanId = ActivityTraceId.CreateFromBytes(byteArray);

        return spanId.ToHexString();
    }

    public static Guid StringToGuid(string hex)
    {
        var numberChars = hex.Length;

        Span<byte> bytes = stackalloc byte[numberChars / 2];

        ActivityTraceId.CreateFromString(hex).CopyTo(bytes);

        return new Guid(bytes);
    }
}
