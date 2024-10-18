using MarketSimulator.Server.Entities;
using MarketSimulator.Server.Repos;

namespace MarketSimulator.Server.Services
{
    public class AssetService
    {
        private Guid gameId;
        private AssetRepo assetRepo;
        private CompanyRepo companyRepo;
        private Random randy;

        public AssetService(Guid gameId, AssetRepo assetRepo, CompanyRepo copmanyRepo)
        {
            this.gameId = gameId;
            this.assetRepo = assetRepo;
            this.companyRepo = copmanyRepo;
            this.randy = new Random();
        }

        public List<Asset> FormAssets(int value, int qty)
        {
            var assets = new List<Asset>();
            var remainingValue = value;
            for(var i=0; i<qty-1; i++)
            {
                var val = randy.Next(remainingValue/2);
                remainingValue -= val;
                var asset = new Asset
                {
                    GameId = gameId,
                    AssetId = Guid.NewGuid(),
                    PrimaryIndustry = (IndustryTypes)randy.Next(6),
                    SecondaryIndustry = (IndustryTypes)randy.Next(6),
                    Value = val
                };
                assets.Add(asset);
                assetRepo.Save(asset);
            }
            var finalAsset = new Asset
            {
                GameId = gameId,
                AssetId = Guid.NewGuid(),
                PrimaryIndustry = (IndustryTypes)randy.Next(6),
                SecondaryIndustry = (IndustryTypes)randy.Next(6),
                Value = remainingValue
            };
            assets.Add(finalAsset);
            assetRepo.Save(finalAsset);

            return assets;
        }

        public Company FormCompany(Guid gameId, string name, int debt, int value, int assets)
        {
            var company = new Company
            {
                GameId = gameId,
                CompanyId = Guid.NewGuid(),
                Name = name,
                Debt = debt,
                Portfolio = FormAssets(value, assets)
            };

            companyRepo.Save(company);
            return company;
        }
    }
}
