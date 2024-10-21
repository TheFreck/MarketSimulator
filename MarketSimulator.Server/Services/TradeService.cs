using MarketSimulator.Server.Entities;
using MarketSimulator.Server.Repos;

namespace MarketSimulator.Server.Services
{
    public interface ITradeService
    {
        void RegisterTrades(List<OrderTicket> orders);
        TradeConfirmation Trade(Player buyer, Player seller, Company company, int shareCount, double sharePrice);
    }

    public class TradeService : ITradeService
    {
        readonly Guid gameId;
        readonly ITradeRepo tradeRepo;
        readonly IPlayerRepo playerRepo;
        public TradeRegistry registry;

        public TradeService(Guid gameId, ITradeRepo tradeRepo, IPlayerRepo playerRepo)
        {
            this.gameId = gameId;
            this.tradeRepo = tradeRepo;
            this.playerRepo = playerRepo;
            this.registry = new TradeRegistry();
        }

        public TradeRegistry ExecuteRegisteredTrades()
        {
            var executedTrades = new List<TradeConfirmation>();
            foreach (var order in registry.Orders)
            {
                var trade = Trade(order.Buyer, order.Seller, order.Company, order.ShareCount, order.SharePrice);
                if (trade.Success)
                {
                    registry.SuccessfulTrades.Add(trade);
                    executedTrades.Add(trade);
                }
                else
                {
                    registry.FailedTrades.Add(trade);
                }
            }

            var c1 = new List<Share>();
            var c2 = new List<Share>();

            c1.AddRange(registry.SuccessfulTrades.SelectMany(t => t.Shares.Where(s => s.CompanyName == "company1")));
            c2.AddRange(registry.SuccessfulTrades.SelectMany(t => t.Shares.Where(s => s.CompanyName == "company2")));
            var player1Trades = executedTrades.Where(t => t.Buyer.Name == "Player1" || t.Seller.Name == "Player1").ToList();
            var player2Trades = executedTrades.Where(t => t.Buyer.Name == "Player2" || t.Seller.Name == "Player2").ToList();
            var player3Trades = executedTrades.Where(t => t.Buyer.Name == "Player3" || t.Seller.Name == "Player3").ToList();

            return registry;
        }

        public void RegisterTrades(List<OrderTicket> orders)
        {
            registry.Orders.AddRange(orders);
        }

        public TradeConfirmation Trade(Player buyer, Player seller, Company company, int shareCount, double sharePrice)
        {
            if (buyer.Cash < shareCount * sharePrice || seller.Portfolio.Where(s => s.CompanyId == company.CompanyId).ToList().Count < shareCount)
            {
                return new TradeConfirmation
                {
                    GameId = gameId,
                    Success = false,
                    TradeId = Guid.NewGuid(),
                };
            }
            buyer.Cash -= sharePrice * shareCount;
            seller.Cash += sharePrice * shareCount;
            var shares = seller.Portfolio.Where(s => s.CompanyId == company.CompanyId).Take(shareCount).ToList();

            foreach(var share in shares)
            {
                seller.Portfolio.Remove(share);
            }
            buyer.Portfolio.AddRange(shares);

            var confirmation = new TradeConfirmation
            {
                Buyer = buyer,
                Seller = seller,
                Company = company,
                SharePrice = sharePrice,
                GameId = gameId,
                Shares = shares,
                TradeId = Guid.NewGuid(),
                Success = true
            };

            tradeRepo.Save(confirmation);
            playerRepo.Save(buyer);
            playerRepo.Save(seller);


            return confirmation;
        }
    }
}
