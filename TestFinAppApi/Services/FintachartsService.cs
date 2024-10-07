using System.Net.WebSockets;
using System.Text.Json;
using System.Text;
using System.Net.Http.Headers;

namespace TestFinAppApi.Services
{
    public class FintachartsService : IFintachartsService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private string _token;

        public FintachartsService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _httpClient.BaseAddress = new Uri(_configuration["Fintacharts:URI"]);
        }

        public async Task<string> GetTokenAsync()
        {
            if (!string.IsNullOrEmpty(_token))
                return _token;

            var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/auth/token");
            request.Content = new StringContent(JsonSerializer.Serialize(new
            {
                username = _configuration["Fintacharts:USERNAME"],
                password = _configuration["Fintacharts:PASSWORD"]
            }), System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
                _token = result["token"];
                return _token;
            }
            else
            {
                throw new HttpRequestException($"Failed to get token. Status code: {response.StatusCode}");
            }
        }

        public async Task<List<string>> GetSupportedAssetsAsync()
        {
            var token = await GetTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync("/api/v1/symbols");

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<List<string>>();
                return result;
            }
            else
            {
                throw new HttpRequestException($"Failed to get supported assets. Status code: {response.StatusCode}");
            }
        }

        public async Task<decimal> GetAssetPriceAsync(string symbol)
        {
            var token = await GetTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync($"/api/v1/quotes/{symbol}");

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<Dictionary<string, decimal>>();
                return result["price"];
            }
            else
            {
                throw new HttpRequestException($"Failed to get asset price for {symbol}. Status code: {response.StatusCode}");
            }
        }

        public async void StartRealtimeUpdates(Action<string, decimal> updateCallback)
        {
            var token = await GetTokenAsync();
            using var ws = new ClientWebSocket();
            await ws.ConnectAsync(new Uri($"{_configuration["Fintacharts:URI_WSS"]}/ws"), CancellationToken.None);

            var subscribeMessage = JsonSerializer.Serialize(new
            {
                action = "subscribe",
                symbols = await GetSupportedAssetsAsync()
            });

            await ws.SendAsync(Encoding.UTF8.GetBytes(subscribeMessage), WebSocketMessageType.Text, true, CancellationToken.None);

            var buffer = new byte[1024 * 4];
            while (ws.State == WebSocketState.Open)
            {
                var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    var update = JsonSerializer.Deserialize<Dictionary<string, object>>(message);
                    if (update.ContainsKey("symbol") && update.ContainsKey("price"))
                    {
                        updateCallback(update["symbol"].ToString(), decimal.Parse(update["price"].ToString()));
                    }
                }
            }
        }
    }
}
