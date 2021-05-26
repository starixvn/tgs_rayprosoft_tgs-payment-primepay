using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using Tgs.Payment.PrimePay.Converters;

namespace Tgs.Payment.PrimePay.Tests
{
    [TestFixture]
    public class PrimePayCryptographyTests
    {
        [Test]
        public void Data_Should_Fllow_PrimePay_Requirements_To_Encrypt()
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
                Converters = {
                    new DecimalJsonConverter(),
                    new NullableDateTimeOffsetJsonConverter(),
                    new DateTimeOffsetJsonConverter(),
                    new TransactionInfoJsonConverter()
                }
            };

            var sut = new PrimePayCryptography(
                new OptionsWrapper<PrimePayOptions>(new PrimePayOptions
                {
                    SystemSerializerOptions = options
                }),
                NullLogger<PrimePayCryptography>.Instance);

            var data = "TestData";

            var expected = "ta5yaIfpzfjYFbO7246HTQ==";

            var actual = sut.Encrypt("0123456789abcdef", "fedcba9876543210", data);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Deserialize_BalanceInfo_Data_Should_Success()
        {
            var json = "[{\"currency\":\"IDR\",\"availableBalance\":\"0.00\",\"givenCredit\":\"0.00\",\"balanceBroughtForward\":\"0.00\",\"totalMerchantTopUp\":\"0.00\",\"totalMemberDeposit\":\"0.00\",\"totalMerchantSettlement\":\"0.00\",\"totalMemberWithdraw\":\"0.00\",\"merchantBalance\":\"0.00\",\"lastTransDate\":\"\"},{\"currency\":\"MYR\",\"availableBalance\":\"0.00\",\"givenCredit\":\"0.00\",\"balanceBroughtForward\":\"0.00\",\"totalMerchantTopUp\":\"0.00\",\"totalMemberDeposit\":\"0.00\",\"totalMerchantSettlement\":\"0.00\",\"totalMemberWithdraw\":\"0.00\",\"merchantBalance\":\"0.00\",\"lastTransDate\":\"\"},{\"currency\":\"RMB\",\"availableBalance\":\"386.00\",\"givenCredit\":\"0.00\",\"balanceBroughtForward\":\"0.00\",\"totalMerchantTopUp\":\"0.00\",\"totalMemberDeposit\":\"500.00\",\"totalMerchantSettlement\":\"0.00\",\"totalMemberWithdraw\":\"100.00\",\"merchantBalance\":\"386.00\",\"lastTransDate\":\"2021-05-11 18:20:09\"},{\"currency\":\"THB\",\"availableBalance\":\"0.00\",\"givenCredit\":\"0.00\",\"balanceBroughtForward\":\"0.00\",\"totalMerchantTopUp\":\"0.00\",\"totalMemberDeposit\":\"0.00\",\"totalMerchantSettlement\":\"0.00\",\"totalMemberWithdraw\":\"0.00\",\"merchantBalance\":\"0.00\",\"lastTransDate\":\"\"},{\"currency\":\"VND\",\"availableBalance\":\"0.00\",\"givenCredit\":\"0.00\",\"balanceBroughtForward\":\"0.00\",\"totalMerchantTopUp\":\"0.00\",\"totalMemberDeposit\":\"0.00\",\"totalMerchantSettlement\":\"0.00\",\"totalMemberWithdraw\":\"0.00\",\"merchantBalance\":\"0.00\",\"lastTransDate\":\"\"}]";

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
                Converters = {
                    new DecimalJsonConverter(),
                    new NullableDateTimeOffsetJsonConverter(),
                    new DateTimeOffsetJsonConverter(),
                    new TransactionInfoJsonConverter()
                }
            };

            var actual = JsonSerializer.Deserialize<IEnumerable<BalanceInfo>>(json, options);
        }
    }
}
