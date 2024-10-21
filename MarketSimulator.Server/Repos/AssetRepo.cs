using MarketSimulator.Server.Entities;

namespace MarketSimulator.Server.Repos
{
    public interface IAssetRepo
    {
        void Save(Asset company);
    }
    public class AssetRepo : IAssetRepo
    {
        public virtual void Save(Asset company)
        {
            return;
        }
    }
}
