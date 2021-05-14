using System;
using System.Text;

namespace Tgs.Payment.PrimePay
{
    internal static class GuidHelper
    {
        public static string GuidToString(Guid id)
        {
            var byteArray = id.ToByteArray();
            var hex = new StringBuilder(byteArray.Length * 2);

            foreach (byte b in byteArray)
                hex.AppendFormat("{0:x2}", b);

            return hex.ToString();
        }

        public static Guid StringToGuid(string hex)
        {
            var numberChars = hex.Length;
            var bytes = new byte[numberChars / 2];

            for (int i = 0; i < numberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);

            return new Guid(bytes);
        }

        internal static string GuidToString(object transactionId) => throw new NotImplementedException();
    }
}
