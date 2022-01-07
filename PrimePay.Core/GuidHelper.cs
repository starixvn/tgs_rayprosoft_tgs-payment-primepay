using System.Numerics;

namespace Tgs.Payment.PrimePay;

internal static class GuidHelper
{
    public static string GuidToString(Guid id)
    {
        var byteArray = id.ToByteArray();

        var bitInt = new BigInteger(byteArray, isBigEndian: true);

        return bitInt.ToString($"x{byteArray.Length * 2}");
    }

    public static Guid StringToGuid(string hex)
    {
        var numberChars = hex.Length;

        var bigInt = BigInteger.Parse($"0{hex}", System.Globalization.NumberStyles.HexNumber);

        Span<byte> bytes = stackalloc byte[numberChars / 2 + 1];

        bigInt.TryWriteBytes(bytes, out _, isBigEndian: true);

        return new Guid(bytes.Slice(1));
    }
}
