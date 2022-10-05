using System.Net.Http.Json;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NSubstitute;
using Tgs.Payment.PrimePay.Services;
using Tgs.Payment.PrimePay.ViewModels;
using Tgs.ServiceUtility;

namespace Tgs.Payment.PrimePay.IntegrationTests;

[TestClass]
public class DepositTests
{
    [TestMethod]
    public async Task DepositCallback()
    {
        var fakeGetDepositFormService = Substitute.For<IGetService<GetDepositFormQuery, DepositFormInfo?>>();
        var fakeSaveReturnService = Substitute.For<ICommandService<SaveDepositReturnCommand>>();
        var fakeApproveService = Substitute.For<ICommandService<ApproveDepositFormCommand>>();

        await using var web = new TestPrimePayApplication(builder => builder
            .ConfigureServices(services => services
                .AddSingleton(fakeGetDepositFormService)
                .AddSingleton(fakeSaveReturnService)
                .AddSingleton(fakeApproveService)));

        var httpClient = web.CreateClient();

        var cryptor = web.Services.GetRequiredService<PrimePayCryptography>();
        var jsonOptions = web.Services.GetRequiredService<IOptions<JsonOptions>>().Value;

        var key = "0123456789abcdef";
        var iv = "fedcba9876543210";

        var merchantId = "abcdefg";
        var depositFormId = Guid.NewGuid();

        var returnData = new PrimePayDepositCompletedReturn
        {
            Amount = 1000M,
            Currency = "HKD",
            ReferenceId = Guid.NewGuid().ToString(),
            Remark = String.Empty,
            RequestDate = DateTimeOffset.Now,
            Status = TransactionStatus.Completed,
            StatusDesc = "Completed",
            TransactionDate = DateTimeOffset.Now,
            TransactionType = "D",
            TransactionTypeDesc = "Member Deposit"
        };
        var callbackViewModel = new CallbackViewModel
        {
            MerchantId = merchantId,
            TransactionId = GuidHelper.GuidToString(depositFormId),
            Message = cryptor.Encrypt(key, iv, returnData)
        };

        var applyInfo = new PrimePayDepositApplyInfo(
            new PrimePayDepositSettings(
                merchantId,
                key,
                iv,
                "HKD",
                depositFormId,
                "http://localhost"),
            new PrimePayDepositSuccessResponse());

        _ = fakeGetDepositFormService.GetAsync(Arg.Any<GetDepositFormQuery>())
            .Returns(new DepositFormInfo(
                depositFormId,
                "player1",
                Guid.NewGuid(),
                Guid.NewGuid(),
                1000M,
                Clients.PaymentFlowState.WaitPayment,
                applyInfo));

        var requestContent = new StringContent(
            JsonSerializer.Serialize(callbackViewModel, jsonOptions.JsonSerializerOptions),
            Encoding.UTF8,
            MediaTypeNames.Application.Json);
        var response = await httpClient.PostAsync("/deposit/callback", requestContent);

        _ = response.EnsureSuccessStatusCode();

        var actual = await response.Content.ReadFromJsonAsync<CallbackConfirmViewModel>(jsonOptions.JsonSerializerOptions);

        Assert.AreEqual("success", actual!.Status);

        _ = fakeSaveReturnService.Received(1)
            .ExecuteAsync(Arg.Is<SaveDepositReturnCommand>(cmd => cmd.FormId == depositFormId));
        _ = fakeApproveService.Received(1)
            .ExecuteAsync(Arg.Is<ApproveDepositFormCommand>(cmd => cmd.FormId == depositFormId));
    }
}
