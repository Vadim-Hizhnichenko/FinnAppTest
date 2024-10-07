using TestFinAppApi.Models;

namespace TestFinAppApi.Services
{
    public interface IMarketAssetService
    {
        Task<List<MarketAsset>> GetAllAssetsAsync();
        Task<MarketAsset> GetAssetBySymbolAsync(string symbol);
        Task UpdateAssetPriceAsync(string symbol, decimal price);
    }
}
