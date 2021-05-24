using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Tgs.Payment.PrimePay
{
    internal class PrimePayClient
    {
        private readonly PrimePayCryptography _cryptography;
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions? _jsonSerializerOptions;
        private readonly ILogger<PrimePayClient> _logger;

        public PrimePayClient(
            HttpClient httpClient,
            IOptions<PrimePayOptions> optionsAccessor,
            PrimePayCryptography cryptography,
            ILogger<PrimePayClient> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _jsonSerializerOptions = optionsAccessor.Value.SystemSerializerOptions;
            _cryptography = cryptography ?? throw new ArgumentNullException(nameof(cryptography));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async ValueTask<PrimePayDepositResponse?> DepositAsync(string key, string iv, DepositOrder requestMessage)
        {
            var request = new ApiRequestBody
            {
                MerchantId = requestMessage.MerchantId,
                Message = _cryptography.Encrypt(key, iv, requestMessage)
            };

            var requestContent = new StringContent(SerializeObject(request));
            requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

            var response = await _httpClient
                .PostAsync("/api/merchant/transact", requestContent)
                .ConfigureAwait(false);

            _logger.LogInformation($"Deposit StatusCode: {response.StatusCode}");

            var bodyJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            _logger.LogInformation($"Deposit Body: {bodyJson}");

            response.EnsureSuccessStatusCode();

            var apiResponseData = DeserializeObject<PrimePayDepositResponse>(bodyJson)!;

            if (apiResponseData.Status == "success")
                return apiResponseData;
            else
                //throw new ApiException(response.StatusCode, bodyJson);
                return null;
        }

        public async ValueTask<BalanceInfo[]?> GetMarchantBalanceInfosAsync(
            string key,
            string iv,
            string merchantId,
            string? currency = null)
        {
            var requestMessage = new
            {
                MerchantId = merchantId,
                Currency = currency
            };

            var request = new ApiRequestBody
            {
                MerchantId = merchantId,
                Message = _cryptography.Encrypt(key, iv, requestMessage)
            };

            var requestContent = new StringContent(SerializeObject(request));
            requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

            var response = await _httpClient
                .PostAsync("/api/merchant/balance", requestContent)
                .ConfigureAwait(false);

            _logger.LogInformation($"Deposit StatusCode: {response.StatusCode}");

            var bodyJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            _logger.LogInformation($"Deposit Body: {bodyJson}");

            response.EnsureSuccessStatusCode();

            var apiResponseData = DeserializeObject<BalanceInfoResponse>(bodyJson)!;

            if (apiResponseData.Status == "success")
            {
                var balanceInfos = _cryptography.Decrypt<BalanceInfo[]>(key, iv, apiResponseData.Result);

                return balanceInfos;
            }
            else
                throw new ApiException(response.StatusCode, bodyJson);
        }

        public async Task<TransactionInfo?> QueryTransactionAsync(
            string key,
            string iv,
            string merchantId,
            string? referenceId = null,
            string? transactionId = null)
        {
            var requestMessage = new
            {
                MerchantId = merchantId,
                ReferenceId = referenceId,
                TransactionId = transactionId
            };

            var request = new ApiRequestBody
            {
                MerchantId = merchantId,
                Message = _cryptography.Encrypt(key, iv, requestMessage)
            };

            var requestContent = new StringContent(SerializeObject(request));
            requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

            var response = await _httpClient
                .PostAsync("/api/merchant/query", requestContent)
                .ConfigureAwait(false);

            _logger.LogInformation($"Deposit StatusCode: {response.StatusCode}");

            var bodyJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            _logger.LogInformation($"Deposit Body: {bodyJson}");

            response.EnsureSuccessStatusCode();

            var apiResponseData = DeserializeObject<QueryTransactionResponse>(bodyJson)!;

            if (apiResponseData.Status == "success")
            {
                var balanceInfos = _cryptography.Decrypt<TransactionInfo>(key, iv, apiResponseData.Result);

                return balanceInfos;
            }
            else
                throw new ApiException(response.StatusCode, bodyJson);
        }

        public async ValueTask<PrimePayWithdrawResponse> WithdrawAsync(string key, string iv, WithdrawOrder requestMessage)
        {
            var request = new ApiRequestBody
            {
                MerchantId = requestMessage.MerchantId,
                Message = _cryptography.Encrypt(key, iv, requestMessage)
            };

            var requestContent = new StringContent(SerializeObject(request));
            requestContent.Headers.ContentType = MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json);

            var response = await _httpClient
                .PostAsync("/api/merchant/transact", requestContent)
                .ConfigureAwait(false);

            _logger.LogInformation($"Withdraw StatusCode: {response.StatusCode}");

            var bodyJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            _logger.LogInformation($"Withdraw Body: {bodyJson}");

            response.EnsureSuccessStatusCode();

            var apiResponseData = DeserializeObject<PrimePayWithdrawResponse>(bodyJson)!;

            if (apiResponseData.Status == "success")
                return apiResponseData;
            else
                throw new ApiException(response.StatusCode, bodyJson);
        }

        private TData? DeserializeObject<TData>(string json)
            => JsonSerializer.Deserialize<TData>(json, _jsonSerializerOptions);

        private string SerializeObject<TData>(TData data)
            => JsonSerializer.Serialize(data, _jsonSerializerOptions);
    }
}
