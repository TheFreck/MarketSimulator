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
    public class With_An_Econ_Repo
    {
        Establish context = () =>
        {
            gameId = Guid.NewGuid();
            econRepoMock = new Mock<IEconRepo>();
            tradeServiceMock = new Mock<ITradeService>();
            assetServiceMock = new Mock<IAssetService>();

            company1Id = Guid.NewGuid();
            company2Id = Guid.NewGuid();
            asset1Id = Guid.NewGuid();
            asset2Id = Guid.NewGuid();
            asset3Id = Guid.NewGuid();
            asset4Id = Guid.NewGuid();
            asset5Id = Guid.NewGuid();
            player1Id = Guid.NewGuid();
            player2Id = Guid.NewGuid();
            asset1 = new Asset
            {
                GameId = gameId,
                AssetId = asset1Id,
                CompanyId = company1Id,
                PrimaryIndustry = IndustryTypes.Red,
                SecondaryIndustry = IndustryTypes.Blue,
                Value = 100000,
            };
            asset2 = new Asset
            {
                GameId = gameId,
                AssetId = asset2Id,
                CompanyId = company1Id,
                PrimaryIndustry = IndustryTypes.Green,
                SecondaryIndustry = IndustryTypes.Yellow,
                Value = 100000
            };
            asset3 = new Asset
            {
                GameId = gameId,
                AssetId = asset3Id,
                CompanyId = company2Id,
                PrimaryIndustry = IndustryTypes.Orange,
                SecondaryIndustry = IndustryTypes.Orange,
                Value = 100000
            };
            asset4 = new Asset
            {
                GameId = gameId,
                AssetId = asset4Id,
                CompanyId = company2Id,
                PrimaryIndustry = IndustryTypes.Red,
                SecondaryIndustry = IndustryTypes.Blue,
                Value = 100000
            };
            asset5 = new Asset
            {
                GameId = gameId,
                AssetId = asset5Id,
                CompanyId = company2Id,
                PrimaryIndustry = IndustryTypes.Yellow,
                SecondaryIndustry = IndustryTypes.Blue,
                Value = 100000
            };
            company1Portfolio = new List<Asset>
            {
                asset1,asset2
            };
            company2Portfolio = new List<Asset> 
            { 
                asset3,asset4,asset5
            };
            company1 = new Company
            {
                GameId = gameId,
                CompanyId = company1Id,
                Name = "company1",
                Portfolio = company1Portfolio,
                Debt = 3,
                TotalShares = 1000000
            };
            company2 = new Company
            {
                GameId = gameId,
                CompanyId = company2Id,
                Name = "company2",
                Portfolio = company2Portfolio,
                Debt = 6,
                TotalShares = 1000000
            };
            player1Portfolio = new List<Share>();
            player2Portfolio = new List<Share>();
            player1 = new Player
            {
                GameId=gameId,
                PlayerId=player1Id,
                Name="Player1",
                Cash=10000,
                RiskTolerance = .56,
                Portfolio = player1Portfolio
            };
            player2 = new Player
            {
                GameId = gameId,
                PlayerId = player2Id,
                Name = "Player2",
                Cash = 10000,
                RiskTolerance = .98,
                Portfolio = player2Portfolio
            };
        };
        protected static Guid gameId;
        protected static Mock<IEconRepo> econRepoMock;
        protected static Mock<ITradeService> tradeServiceMock;
        protected static Mock<IAssetService> assetServiceMock;
        protected static Guid company1Id;
        protected static Guid company2Id;
        protected static Guid asset1Id;
        protected static Guid asset2Id;
        protected static Guid asset3Id;
        protected static Guid asset4Id;
        protected static Guid asset5Id;
        protected static List<Asset> company1Portfolio;
        protected static List<Asset> company2Portfolio;
        protected static Guid player1Id;
        protected static Guid player2Id;
        protected static Asset asset1;
        protected static Asset asset2;
        protected static Asset asset3;
        protected static Asset asset4;
        protected static Asset asset5;
        protected static Company company1;
        protected static Company company2;
        protected static List<Share> player1Portfolio;
        protected static List<Share> player2Portfolio;
        protected static Player player1;
        protected static Player player2;
    }

    public class When_Completing_One_Market_Cycle : With_An_Econ_Repo
    {
        Establish context = () =>
        {
            tradeShares1 = new List<Share>();
            tradeShares2 = new List<Share>();
            for(var i=0; i<10; i++)
            {
                tradeShares1.Add(new Share
                {
                    GameId=gameId,
                    CompanyId=company1Id,
                    CompanyName="company1",
                    ShareId=Guid.NewGuid(),
                });
                tradeShares2.Add(new Share
                {
                    GameId = gameId,
                    CompanyId = company2Id,
                    CompanyName = "company2",
                    ShareId = Guid.NewGuid()
                });
            }
            tradeRegistry = new TradeRegistry
            {
                Orders = new List<OrderTicket>(),
                SuccessfulTrades = new List<TradeConfirmation> 
                {
                    new TradeConfirmation
                    {
                        GameId=gameId,
                        Buyer=player1,
                        Seller=player2,
                        Company=company1,
                        SharePrice=10,
                        Shares=tradeShares1,
                        Success=true
                    },
                    new TradeConfirmation
                    {
                        GameId=gameId,
                        Buyer=player2,
                        Seller=player1,
                        Company=company2,
                        SharePrice=10,
                        Shares=tradeShares2,
                        Success=true
                    }
                },
                FailedTrades=new List<TradeConfirmation>()
            };
            economyService = new EconomyService(tradeServiceMock.Object, assetServiceMock.Object);
            tradeServiceMock.Setup(t => t.ExecuteRegisteredTrades()).Returns(tradeRegistry);
            economy = new Economy
            {
                Companies = new Company[] { company1, company2 },
                GameId = gameId,
                Players = new Player[] { player1, player2 },
                IndustryGrowth = new Dictionary<IndustryTypes, double> 
                {
                    { IndustryTypes.Red, .1 },
                    { IndustryTypes.Orange, .2 },
                    { IndustryTypes.Yellow, .3 },
                    { IndustryTypes.Green, .15 },
                    { IndustryTypes.Blue, .25 },
                    { IndustryTypes.Violet, .35 },
                }
            };
            expectedGrowth = new Dictionary<Company, int>
            {
                {company1, 235033},
                {company2, 363333}
            };
        };

        Because of = () => economyService.MarketCycle(economy);

        It Should_Process_Industry_Growth = () => assetServiceMock.Verify(a => a.GrowIndustries(), Times.Once());

        It Should_Process_Company_Growth = () => assetServiceMock.Verify(a => a.GrowCompany(Moq.It.IsAny<Company>(), Moq.It.IsAny<Dictionary<IndustryTypes,double>>()), Times.Exactly(2));

        It Should_Execute_Trades = () => tradeServiceMock.Verify(t => t.ExecuteRegisteredTrades(), Times.Once());

        private static TradeRegistry tradeRegistry;
        private static EconomyService economyService;
        private static Economy economy;
        private static Dictionary<Company, int> expectedGrowth;
        private static List<Share> tradeShares1;
        private static List<Share> tradeShares2;
    }
}
