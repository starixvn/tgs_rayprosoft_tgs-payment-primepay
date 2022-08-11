using System.Net.Http.Json;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NSubstitute;
using Tgs.Payment.Clients;
using Tgs.Payment.PrimePay.Services;
using Tgs.Payment.PrimePay.ViewModels;
using Tgs.ServiceUtility;

namespace Tgs.Payment.PrimePay.IntegrationTests;

[TestClass]
public class WithdrawTests
{
    [TestMethod]
    public async Task WithdrawCallback()
    {
        var fakeGetWithdrawFormService = Substitute.For<IGetService<GetWithdrawFormQuery, WithdrawFormInfo?>>();
        var fakeSaveReturnService = Substitute.For<ICommandService<SaveWithdrawReturnCommand>>();
        var fakePaymentClient = Substitute.For<IPaymentClient>();

        await using var web = new TestPrimePayApplication(builder => builder
            .ConfigureServices(services => services
                .AddSingleton(fakeGetWithdrawFormService)
                .AddSingleton(fakeSaveReturnService)
                .AddSingleton(fakePaymentClient)));

        var httpClient = web.CreateClient();

        var cryptor = web.Services.GetRequiredService<PrimePayCryptography>();
        var jsonOptions = web.Services.GetRequiredService<IOptions<JsonOptions>>().Value;

        var key = "0123456789abcdef";
        var iv = "fedcba9876543210";

        var merchantId = "abcdefg";
        var withdrawFormId = Guid.NewGuid();

        var returnData = new PrimePayWithdrawCompletedReturn
        {
            FromBankName = "HSBC",
            ServiceFee = 10M,
            TransactionAmount = 1000M,
            Amount = 1000M,
            Currency = "HKD",
            ReferenceId = Guid.NewGuid().ToString(),
            Remark = string.Empty,
            Status = TransactionStatus.Completed,
            StatusDesc = "Completed",
            TransactionType = "W",
            TransactionTypeDesc = "Withdraw"
        };
        var callbackViewModel = new CallbackViewModel
        {
            MerchantId = merchantId,
            TransactionId = GuidHelper.GuidToString(withdrawFormId),
            Message = cryptor.Encrypt(key, iv, returnData)
        };

        var applyInfo = new PrimePayWithdrawApplyInfo(
            new PrimePayWithdrawSettings(
                merchantId,
                key,
                iv,
                withdrawFormId,
                "HKD",
                "111111111",
                "test",
                "111",
                "bank"),
            new PrimePayWithdrawSuccessResponse());

        _ = fakeGetWithdrawFormService.GetAsync(Arg.Any<GetWithdrawFormQuery>())
            .Returns(new WithdrawFormInfo(
                withdrawFormId,
                "player1",
                Clients.UserType.Player,
                Guid.NewGuid(),
                Guid.NewGuid(),
                1000M,
                Clients.PaymentFlowState.Approved,
                Guid.NewGuid(),
                null,
                applyInfo));

        var requestContent = new StringContent(
            JsonSerializer.Serialize(callbackViewModel, jsonOptions.JsonSerializerOptions),
            Encoding.UTF8,
            MediaTypeNames.Application.Json);
        var response = await httpClient.PostAsync("/withdraw/callback", requestContent);

        response.EnsureSuccessStatusCode();

        var actual = await response.Content.ReadFromJsonAsync<CallbackConfirmViewModel>(jsonOptions.JsonSerializerOptions);

        Assert.AreEqual("success", actual!.Status);

        _ = fakeSaveReturnService.Received(1)
            .ExecuteAsync(Arg.Is<SaveWithdrawReturnCommand>(cmd => cmd.FormId == withdrawFormId));
        _ = fakePaymentClient.Received(1)
            .PaidWithdrawAsync(Arg.Is(withdrawFormId), Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }
}
