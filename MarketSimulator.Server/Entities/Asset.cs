namespace MarketSimulator.Server.Entities
{
    public class Asset
    {
        public Guid GameId;
        public Guid AssetId;
        public Guid CompanyId;
        public IndustryTypes PrimaryIndustry;
        public IndustryTypes SecondaryIndustry;
        public decimal Value;
    }
}
