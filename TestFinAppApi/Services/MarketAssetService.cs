using Microsoft.EntityFrameworkCore;
using TestFinAppApi.Data;
using TestFinAppApi.Models;

namespace TestFinAppApi.Services
{
    public class MarketAssetService : IMarketAssetService
    {
        private readonly AppDbContext _context;
        private readonly IFintachartsService _fintachartsService;

        public MarketAssetService(AppDbContext context, IFintachartsService fintachartsService)
        {
            _context = context;
            _fintachartsService = fintachartsService;
            Task.Run(InitializeAssetsAsync).Wait(); // Run initialization asynchronously
        }

        private async Task InitializeAssetsAsync()
        {
            if (!await _context.MarketAssets.AnyAsync())
            {
                try
                {
                    var symbols = await _fintachartsService.GetSupportedAssetsAsync();
                    foreach (var symbol in symbols)
                    {
                        var price = await _fintachartsService.GetAssetPriceAsync(symbol);
                        _context.MarketAssets.Add(new MarketAsset
                        {
                            Symbol = symbol,
                            Price = price,
                            LastUpdate = DateTime.UtcNow
                        });
                    }
                    await _context.SaveChangesAsync();

                    // Start real-time updates after initialization
                    _fintachartsService.StartRealtimeUpdates((symbol, price) => UpdateAssetPriceAsync(symbol, price).Wait());
                }
                catch (Exception ex)
                {
                    // Log the exception
                    Console.WriteLine($"Error initializing assets: {ex.Message}");
                    // You might want to throw the exception or handle it differently based on your requirements
                }
            }
        }

        public async Task<List<MarketAsset>> GetAllAssetsAsync()
        {
            return await _context.MarketAssets.ToListAsync();
        }

        public async Task<MarketAsset> GetAssetBySymbolAsync(string symbol)
        {
            return await _context.MarketAssets.FirstOrDefaultAsync(a => a.Symbol == symbol);
        }

        public async Task UpdateAssetPriceAsync(string symbol, decimal price)
        {
            var asset = await _context.MarketAssets.FirstOrDefaultAsync(a => a.Symbol == symbol);
            if (asset != null)
            {
                asset.Price = price;
                asset.LastUpdate = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }
    }
}
