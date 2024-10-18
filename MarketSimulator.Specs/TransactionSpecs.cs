using Machine.Specifications;
using MarketSimulator.Server.Entities;
using MarketSimulator.Server.Repos;
using MarketSimulator.Server.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using It = Machine.Specifications.It;

namespace MarketSimulator.Specs
{
    public class With_A_Trade_Repo
    {
        Establish context = () =>
        {
            gameId = Guid.NewGuid();
            TradeRepoMock = new Mock<TradeRepo>();
        };

        protected static Guid gameId;
        protected static Mock<TradeRepo> TradeRepoMock { get; private set; }
    }
    public class When_Trading_Shares : With_A_Trade_Repo
    {
        Establish context = () =>
        {
            tradeService = new TradeService(gameId, TradeRepoMock.Object);
            TradeRepoMock.Setup(t => t.Save(Moq.It.IsAny<TradeTicket>()));
            companyId = Guid.NewGuid();
            assetId = Guid.NewGuid();
            shares = new List<Share>();
            asset = new Asset
            {
                GameId = gameId,
                AssetId = assetId,
                PrimaryIndustry = IndustryTypes.Red,
                SecondaryIndustry = IndustryTypes.Green,
                Value = 10000000
            };
            for (var i = 0; i < 100; i++) 
            {
                shares.Add(new Share
                {
                    GameId = gameId,
                    AssetId = assetId,
                    ShareId = Guid.NewGuid()
                }); 
            }
            buyer = new Player
            {
                GameId = gameId,
                PlayerId = Guid.NewGuid(),
                Name = "Buyer",
                Cash = 1000000,
                Portfolio = shares
            };
            seller = new Player
            {
                GameId = gameId,
                PlayerId = Guid.NewGuid(),
                Name = "Seller",
                Cash = 1000000,
                Portfolio = new List<Share>()
            };
            company = new Company
            {
                GameId = gameId,
                CompanyId = companyId,
                Portfolio = new List<Asset> { asset },
                Debt = 3,
            };
            tradeTicket = new TradeTicket
            {
                Buyer = buyer,
                Seller = seller,
                Company = company,
                GameId = gameId,
                SharePrice = 20,
                Shares = shares
            };
            inputs = new List<(Player Buyer, Player Seller, Company Company, List<Share> Shares, decimal SharePrice, TradeTicket Expected)>
            {
                (buyer, seller, company, shares, 20, tradeTicket)
            };
            outcomes = new Dictionary<TradeTicket,TradeTicket>();
        };

        Because of = () =>
        {
            for (var i = 0; i < inputs.Count; i++)
            {
                outcomes.Add(inputs[i].Expected,tradeService.Trade(inputs[i].Buyer, inputs[i].Seller, inputs[i].Company, inputs[i].Shares, inputs[i].SharePrice));
            }
        };

        It Should_Return_A_Trade_Ticket = () =>
        {
            for (var i = 0; i < inputs.Count; i++)
            {
                outcomes.Keys.ToList()[i].Buyer.ShouldEqual(outcomes.Values.ToList()[i].Buyer);
                outcomes.Keys.ToList()[i].Seller.ShouldEqual(outcomes.Values.ToList()[i].Seller);
                outcomes.Keys.ToList()[i].Company.CompanyId.ShouldEqual(outcomes.Values.ToList()[i].Company.CompanyId);
                outcomes.Keys.ToList()[i].Shares.ShouldContainOnly(outcomes.Values.ToList()[i].Shares);
                outcomes.Keys.ToList()[i].SharePrice.ShouldEqual(outcomes.Values.ToList()[i].SharePrice);
            }
        };

        It Should_Persist_The_Trade_Ticket = () =>
        {
            for (var i = 0; i < inputs.Count; i++)
            {
                TradeRepoMock.Verify(t => t.Save(Moq.It.IsAny<TradeTicket>()), Times.Once());
            }
        };

        private static List<(Player Buyer, Player Seller, Company Company, List<Share> Shares, decimal SharePrice, TradeTicket Expected)> inputs;
        private static Dictionary<TradeTicket, TradeTicket> outcomes;
        private static TradeService tradeService;
        private static Guid companyId;
        private static Guid assetId;
        private static List<Share> shares;
        private static Asset asset;
        private static Player buyer;
        private static Player seller;
        private static Company company;
        private static TradeTicket tradeTicket;
    }
}
