namespace TestFinAppApi.Services
{
    public interface IFintachartsService
    {
        Task<string> GetTokenAsync();
        Task<List<string>> GetSupportedAssetsAsync();
        Task<decimal> GetAssetPriceAsync(string symbol);
        void StartRealtimeUpdates(Action<string, decimal> updateCallback);
    }
}
