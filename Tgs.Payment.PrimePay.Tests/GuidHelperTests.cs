using NUnit.Framework;

namespace Tgs.Payment.PrimePay.Tests;

[TestFixture]
public class GuidHelperTests
{
    [Test]
    public void GuidToString_Test()
    {
        var guid = Guid.NewGuid();

        var expected = BitConverter.ToString(guid.ToByteArray())
            .Replace("-", string.Empty)
            .ToLower();

        var actual = GuidHelper.GuidToString(guid);

        Assert.AreEqual(expected, actual);
    }

    [Test]
    public void GuidToString_Negative_Test()
    {
        var guid = Guid.Parse("FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF");

        var expected = BitConverter.ToString(guid.ToByteArray())
            .Replace("-", string.Empty)
            .ToLower();

        var actual = GuidHelper.GuidToString(guid);

        Assert.AreEqual(expected, actual);
    }

    [Test]
    public void HexToGuid_Test()
    {
        var guid = Guid.NewGuid();

        var hex = BitConverter.ToString(guid.ToByteArray())
            .Replace("-", string.Empty)
            .ToLower();

        var actual = GuidHelper.StringToGuid(hex);

        Assert.AreEqual(guid, actual);
    }

    [Test]
    public void HexToGuid_Negative_Test()
    {
        var hex = "FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF".Replace("-", String.Empty).ToLower();

        var expected = Guid.Parse("FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF");

        var actual = GuidHelper.StringToGuid(hex);

        Assert.AreEqual(expected, actual);
    }
}