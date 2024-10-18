using MarketSimulator.Server.Entities;
using MarketSimulator.Server.Repos;

namespace MarketSimulator.Server.Services
{
    public class TradeService
    {
        readonly Guid gameId;
        readonly TradeRepo tradeRepo;
        public TradeService(Guid gameId, TradeRepo tradeRepo)
        {
            this.gameId = gameId;
            this.tradeRepo = tradeRepo;
        }
        public TradeTicket Trade(Player buyer, Player seller, Company company, List<Share> shares, decimal sharePrice)
        {
            var outcome = new TradeTicket
            {
                Buyer = buyer,
                Seller = seller,
                Company = company,
                SharePrice = sharePrice,
                GameId = this.gameId,
                Shares = shares,
            };

            tradeRepo.Save(outcome);
            return outcome;
        }
    }
}
