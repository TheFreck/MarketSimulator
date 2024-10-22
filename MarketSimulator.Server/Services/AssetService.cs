using MarketSimulator.Server.Entities;
using MarketSimulator.Server.Repos;

namespace MarketSimulator.Server.Services
{
    public interface IAssetService
    {
        List<Asset> FormAssets(int value, int qty);
        Company FormCompany(Guid gameId, string name, int debt, int value, int assets);
        Company GrowCompany(Company company, Dictionary<IndustryTypes, double> industryInputs);
        Dictionary<IndustryTypes, double> GrowIndustries();
    }
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

        public Company GrowCompany(Company company, Dictionary<IndustryTypes, double> industryInputs)
        {
            for (var i = 0; i < company.Portfolio.Count; i++)
            {
                var grossRate = industryInputs[company.Portfolio[i].PrimaryIndustry] * 2;
                grossRate += industryInputs[company.Portfolio[i].SecondaryIndustry];
                company.Portfolio[i].Value *= 1 + grossRate / 3;
            }
            var companyValue = company.Value;
            companyRepo.Save(company);
            return company;
        }

        public Dictionary<IndustryTypes,double> GrowIndustries()
        {
            var a = .01;
            var b = 2.0129435;
            var c = .01;
            var mag = Math.Tan(Math.PI * (randy.NextDouble() * 2 - 1) / b) * c - a;
            return new Dictionary<IndustryTypes, double>
            {
                { IndustryTypes.Red, Math.Tan(Math.PI * (randy.NextDouble() * 2 - 1) / b) * c - a},
                { IndustryTypes.Orange, Math.Tan(Math.PI * (randy.NextDouble() * 2 - 1) / b) * c - a},
                { IndustryTypes.Yellow, Math.Tan(Math.PI * (randy.NextDouble() * 2 - 1) / b) * c - a},
                { IndustryTypes.Green, Math.Tan(Math.PI * (randy.NextDouble() * 2 - 1) / b) * c - a},
                { IndustryTypes.Blue, Math.Tan(Math.PI * (randy.NextDouble() * 2 - 1) / b) * c - a},
                { IndustryTypes.Violet, Math.Tan(Math.PI * (randy.NextDouble() * 2 - 1) / b) * c - a},
            };
        }
    }
}
