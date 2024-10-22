using MarketSimulator.Server.Repos;
using Microsoft.AspNetCore.Mvc;

namespace MarketSimulator.Server.Controllers
{
    public class AssetController : Controller
    {
        public IAssetRepo assetRepo;
        public AssetController(IAssetRepo assetRepo)
        {
            this.assetRepo = assetRepo;
        }

        [HttpGet("/getAll/{gameId}")]
        public IActionResult GetAssets(Guid gameId)
        {
            return Ok(assetRepo.GetAll(gameId));
        }

        [HttpGet("/getAsset/{gameId}/{assetId}")]
        public IActionResult GetOneAsset(Guid gameId, Guid assetId)
        {
            return Ok(assetRepo.GetOne(gameId, assetId));
        }
    }
}
