using MarketSimulator.Server.Entities;

namespace MarketSimulator.Server.Services
{
    public interface IEconomyService
    {
        void MarketCycle(Economy economy);
    }

    public class EconomyService
    {
        readonly ITradeService tradeService;
        readonly IAssetService assetService;
        public EconomyService(ITradeService tradeService, IAssetService assetService)
        {
            this.tradeService = tradeService;
            this.assetService = assetService;
        }

        public void MarketCycle(Economy economy)
        {
            var industries = assetService.GrowIndustries();
            var companies = new List<Company>();
            foreach(var company in economy.Companies)
            {
                companies.Add(assetService.GrowCompany(company, industries));
            }

            var tradeRegistry = tradeService.ExecuteRegisteredTrades();

            return;
        }
    }
}
