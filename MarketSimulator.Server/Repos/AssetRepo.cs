using MarketSimulator.Server.Entities;

namespace MarketSimulator.Server.Repos
{
    public interface IAssetRepo
    {
        void Save(Asset company);
        List<Asset> GetAll(Guid gameId);
        Asset GetOne(Guid gameId, Guid assetId);
    }
    public class AssetRepo : IAssetRepo
    {
        public List<Asset> GetAll(Guid gameId)
        {
            throw new NotImplementedException();
        }

        public Asset GetOne(Guid gameId, Guid assetId)
        {
            throw new NotImplementedException();
        }

        public virtual void Save(Asset company)
        {
            return;
        }
    }
}
