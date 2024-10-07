using Microsoft.AspNetCore.Mvc;
using TestFinAppApi.Models;
using TestFinAppApi.Services;

namespace TestFinAppApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MarketAssetController : ControllerBase
    {
        private readonly IMarketAssetService _marketAssetService;

        public MarketAssetController(IMarketAssetService marketAssetService)
        {
            _marketAssetService = marketAssetService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<string>>> GetSupportedAssets()
        {
            var assets = await _marketAssetService.GetAllAssetsAsync();
            return Ok(assets.Select(a => a.Symbol));
        }

        [HttpGet("{symbol}")]
        public async Task<ActionResult<MarketAsset>> GetAssetPrice(string symbol)
        {
            var asset = await _marketAssetService.GetAssetBySymbolAsync(symbol);
            if (asset == null)
                return NotFound();

            return Ok(asset);
        }
    }
}
