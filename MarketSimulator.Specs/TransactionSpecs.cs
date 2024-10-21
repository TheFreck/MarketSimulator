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
            TradeRepoMock = new Mock<ITradeRepo>();
            PlayerRepoMock = new Mock<IPlayerRepo>();
            asset1aId = Guid.NewGuid();
            asset1bId = Guid.NewGuid();
            asset1cId = Guid.NewGuid();
            company1Port = new List<Asset>
            {
                new Asset
                {
                    GameId=gameId,
                    AssetId=asset1aId,
                    CompanyId=company1Id,
                    PrimaryIndustry=IndustryTypes.Red,
                    SecondaryIndustry=IndustryTypes.Green,
                    Value=100000
                }
            };
            asset2aId = Guid.NewGuid();
            asset2bId = Guid.NewGuid();
            company2Port = new List<Asset>
            {
                new Asset
                {
                    GameId =gameId,
                    AssetId=asset2aId,
                    CompanyId=company2Id,
                    PrimaryIndustry=IndustryTypes.Orange,
                    SecondaryIndustry=IndustryTypes.Yellow,
                    Value=75000
                },
                new Asset
                {
                    GameId=gameId,
                    AssetId=asset2bId,
                    CompanyId=company2Id,
                    PrimaryIndustry=IndustryTypes.Yellow,
                    SecondaryIndustry=IndustryTypes.Blue,
                    Value=125000
                }
            };
            company1Id = Guid.NewGuid();
            company2Id = Guid.NewGuid();
            company1 = new Company
            {
                GameId = gameId,
                CompanyId = company1Id,
                Name = "Company1",
                Debt = 3,
                TotalShares = 1000000,
                Portfolio = company1Port
            };
            company2 = new Company
            {
                GameId = gameId,
                CompanyId = company2Id,
                Name = "Company2",
                Debt = 3,
                TotalShares = 1000000,
                Portfolio = company2Port
            };
            companies = new List<Company>
            {
                company1,
                company2
            };
            startingCash = 3000;
            startingShares = 200;
            player1Shares = new List<Share>();
            for (var i = 0; i < startingShares; i++)
            {
                player1Shares.Add(new Share
                {
                    GameId = gameId,
                    ShareId = Guid.NewGuid(),
                    CompanyId = company1Id,
                    CompanyName = "company1"
                });
            }
            player2Shares = new List<Share>();
            for (var i = 0; i < startingShares; i++)
            {
                player2Shares.Add(new Share
                {
                    GameId = gameId,
                    ShareId = Guid.NewGuid(),
                    CompanyId = company2Id,
                    CompanyName = "company2"
                });
            }
            player3Shares = new List<Share>();
            for (var i = 0; i < startingShares/2; i++)
            {
                player3Shares.Add(new Share
                {
                    GameId = gameId,
                    ShareId = Guid.NewGuid(),
                    CompanyId = company1Id,
                    CompanyName = "company1"
                });
                player3Shares.Add(new Share
                {
                    GameId = gameId,
                    ShareId = Guid.NewGuid(),
                    CompanyId = company2Id,
                    CompanyName = "company2"
                });
            }
            player1 = new Player
            {
                GameId = gameId,
                PlayerId = Guid.NewGuid(),
                Name = "Player1",
                Cash = startingCash,
                RiskTolerance = .75,
                Portfolio = player1Shares
            };
            player2 = new Player
            {
                GameId = gameId,
                PlayerId = Guid.NewGuid(),
                Name = "Player2",
                Cash = startingCash,
                RiskTolerance = .45,
                Portfolio = player2Shares
            };
            player3 = new Player
            {
                GameId = gameId,
                PlayerId = Guid.NewGuid(),
                Name = "Player3",
                Cash = startingCash,
                RiskTolerance = .3,
                Portfolio = player3Shares
            };
            inputPlayers = new List<Player>
            {
                player1,
                player2,
                player3
            };
        };

        protected static Guid gameId;
        protected static Mock<ITradeRepo> TradeRepoMock { get; private set; }
        protected static Mock<IPlayerRepo> PlayerRepoMock { get; private set; }

        protected static Company company1;
        protected static List<Asset> company1Port;
        protected static Guid asset1aId;
        protected static Guid asset1bId;
        protected static Guid asset1cId;
        protected static Guid asset2aId;
        protected static Guid asset2bId;
        protected static Company company2;
        protected static List<Asset> company2Port;
        protected static Guid company1Id;
        protected static Guid company2Id;
        protected static List<Company> companies;
        protected static int startingShares;
        protected static int startingCash;
        protected static List<Share> player1Shares;
        protected static List<Share> player2Shares;
        protected static List<Share> player3Shares;
        protected static Player player1;
        protected static Player player2;
        protected static Player player3;
        protected static List<Player> inputPlayers;
    }
    public class When_Trading_Shares : With_A_Trade_Repo
    {
        Establish context = () =>
        {
            TradeRepoMock.Setup(t => t.Save(Moq.It.IsAny<TradeConfirmation>()));
            PlayerRepoMock.Setup(p => p.Save(Moq.It.IsAny<Player>()));
            tradeService = new TradeService(gameId, TradeRepoMock.Object, PlayerRepoMock.Object);
            buyer = player1;
            seller = player2;
            shareCount = 100;
            tradeTicket = new TradeConfirmation
            {
                Buyer = buyer,
                Seller = seller,
                Company = company2,
                GameId = gameId,
                SharePrice = 20,
                Shares = player2Shares.Where(s => s.CompanyId == company2Id).Take(shareCount).ToList()
            };
            inputs = new List<(Player Buyer, Player Seller, Company Company, int Shares, double SharePrice, TradeConfirmation Expected)>
            {
                (buyer, seller, company2, shareCount, 20, tradeTicket)
            };
            outcomes = new Dictionary<TradeConfirmation,TradeConfirmation>();
        };

        Because of = () =>
        {
            for (var i = 0; i < inputs.Count; i++)
            {
                outcomes.Add(inputs[i].Expected,tradeService.Trade(inputs[i].Buyer, inputs[i].Seller, inputs[i].Company, inputs[i].Shares, inputs[i].SharePrice));
            }
        };

        It Should_Return_A_Trade_Confirmation = () =>
        {
            for (var i = 0; i < inputs.Count; i++)
            {
                outcomes.Keys.ToList()[i].Buyer.ShouldEqual(outcomes.Values.ToList()[i].Buyer);
                outcomes.Keys.ToList()[i].Seller.ShouldEqual(outcomes.Values.ToList()[i].Seller);
                outcomes.Keys.ToList()[i].Company.CompanyId.ShouldEqual(outcomes.Values.ToList()[i].Company.CompanyId);
                buyer.Portfolio.Where(p => p.CompanyId == company2Id).Count().ShouldEqual(outcomes.Values.ToList()[i].Shares.Count);
                outcomes.Keys.ToList()[i].SharePrice.ShouldEqual(outcomes.Values.ToList()[i].SharePrice);
            }
        };

        It Should_Persist_The_Trade_Ticket = () =>
        {
            for (var i = 0; i < inputs.Count; i++)
            {
                TradeRepoMock.Verify(t => t.Save(Moq.It.IsAny<TradeConfirmation>()), Times.Once());
            }
        };

        It Should_Update_Buyer_And_Seller_Portfolios = () =>
        {
            buyer.Portfolio.Count.ShouldEqual(startingShares+shareCount);
            seller.Portfolio.Count.ShouldEqual(startingShares-shareCount);
        };

        It Should_Update_Buyer_And_Seller_Cash = () =>
        {
            buyer.Cash.ShouldEqual(startingCash-2000);
            seller.Cash.ShouldEqual(startingCash+2000);
        };

        It Should_Persist_Buyer_And_Seller = () =>
        {
            PlayerRepoMock.Verify(p => p.Save(Moq.It.IsAny<Player>()), Times.Exactly(2));
        };

        private static List<(Player Buyer, Player Seller, Company Company, int Shares, double SharePrice, TradeConfirmation Expected)> inputs;
        private static Dictionary<TradeConfirmation, TradeConfirmation> outcomes;
        private static TradeService tradeService;
        private static Player buyer;
        private static Player seller;
        private static int shareCount;
        private static List<Share> shares;
        private static TradeConfirmation tradeTicket;
    }

    public class When_Executing_Registered_Trades : With_A_Trade_Repo
    {
        Establish context = () =>
        {
            TradeRepoMock.Setup(t => t.Save(Moq.It.IsAny<TradeConfirmation>()));
            PlayerRepoMock.Setup(p => p.Save(Moq.It.IsAny<Player>()));
            tradeService = new TradeService(gameId, TradeRepoMock.Object, PlayerRepoMock.Object);

            player1.Portfolio = new List<Share>();
            player2.Portfolio = new List<Share>();
            player3.Portfolio = new List<Share>();
            for(var i=0; i<startingShares; i++)
            {
                player1.Portfolio.Add(new Share
                {
                    CompanyId = company1Id,
                    CompanyName = "company1",
                    GameId = gameId,
                    ShareId = Guid.NewGuid()
                });
                player2.Portfolio.Add(new Share
                {
                    CompanyName = "company2",
                    GameId = gameId,
                    ShareId = Guid.NewGuid(),
                    CompanyId = company2Id
                });
                player3.Portfolio.Add(new Share
                {
                    CompanyName = i % 2 == 0 ? "company1" : "company2",
                    CompanyId = i % 2 == 0 ? company1Id : company2Id,
                    GameId = gameId,
                    ShareId = Guid.NewGuid()
                });
            }

            trades = new List<OrderTicket>
            {
                new OrderTicket
                {
                    GameId = gameId,
                    Buyer = player1,
                    Seller = player3,
                    Company = company1,
                    ShareCount = 10,
                    SharePrice = 10,
                },
                new OrderTicket
                {
                    GameId = gameId,
                    Buyer = player2,
                    Seller = player3,
                    Company = company2,
                    ShareCount = 10,
                    SharePrice = 20,
                },
                new OrderTicket
                {
                    GameId = gameId,
                    Buyer = player1,
                    Seller = player3,
                    Company = company1,
                    ShareCount = 20,
                    SharePrice = 20,
                },
                new OrderTicket
                {
                    GameId = gameId,
                    Buyer = player2,
                    Seller = player3,
                    Company = company2,
                    ShareCount = 20,
                    SharePrice = 40,
                },
                new OrderTicket
                {
                    GameId = gameId,
                    Buyer = player1,
                    Seller = player3,
                    Company = company1,
                    ShareCount = 30,
                    SharePrice = 30,
                },
                new OrderTicket
                {
                    GameId = gameId,
                    Buyer = player2,
                    Seller = player3,
                    Company = company2,
                    ShareCount = 30,
                    SharePrice = 60,
                },
                new OrderTicket
                {
                    GameId = gameId,
                    Buyer = player1,
                    Seller = player3,
                    Company = company1,
                    ShareCount = 40,
                    SharePrice = 40,
                },
                new OrderTicket
                {
                    GameId = gameId,
                    Buyer = player2,
                    Seller = player3,
                    Company = company2,
                    ShareCount = 40,
                    SharePrice = 80,
                }
            };
            tradeService.RegisterTrades(trades);
            expectedTrades = new List<TradeConfirmation>
            {
                new TradeConfirmation
                {
                    GameId = gameId,
                    Buyer = player1,
                    Seller = player3,
                    Company = company1,
                    Shares = new List<Share>(),
                    SharePrice = 10,
                    Success = true,
                },
                new TradeConfirmation
                {
                    GameId = gameId,
                    Buyer = player2,
                    Seller = player3,
                    Company = company2,
                    Shares = new List<Share>(),
                    SharePrice = 20,
                    Success = true,
                },
                new TradeConfirmation
                {
                    GameId = gameId,
                    Buyer = player1,
                    Seller = player3,
                    Company = company1,
                    Shares = new List<Share>(),
                    SharePrice = 20,
                    Success = true,
                },
                new TradeConfirmation
                {
                    GameId = gameId,
                    Buyer = player2,
                    Seller = player3,
                    Company = company2,
                    Shares = new List<Share>(),
                    SharePrice = 40,
                    Success = true,
                },
                new TradeConfirmation
                {
                    GameId = gameId,
                    Buyer = player1,
                    Seller = player3,
                    Company = company1,
                    Shares = new List<Share>(),
                    SharePrice = 30,
                    Success = true,
                },
                new TradeConfirmation
                {
                    GameId = gameId,
                    Buyer = player2,
                    Seller = player3,
                    Company = company2,
                    Shares = new List<Share>(),
                    SharePrice = 60,
                    Success = true,
                },
                new TradeConfirmation
                {
                    GameId = gameId,
                    Buyer = player1,
                    Seller = player3,
                    Company = company1,
                    Shares = new List<Share>(),
                    SharePrice = 40,
                    Success = true,
                },
                new TradeConfirmation
                {
                    GameId = gameId,
                    Buyer = player2,
                    Seller = player3,
                    Company = company2,
                    Shares = new List<Share>(),
                    SharePrice = 80,
                    Success = false
                }
            };
            expectedOutcomes = new TradeRegistry
            {
                Orders = trades,
                SuccessfulTrades = expectedTrades.Where(t => t.Success).ToList(),
                FailedTrades = expectedTrades.Where(t => !t.Success).ToList()
            };
        };

        Because of = () => outcomes = tradeService.ExecuteRegisteredTrades();

        It Should_Assign_A_TradeId = () => outcomes.SuccessfulTrades.Where(t => t.TradeId != Guid.Empty).Count().ShouldEqual(7);
        It Should_Assign_Success = () => outcomes.SuccessfulTrades.Count.ShouldEqual(7);
        It Should_Assign_Failure = () => outcomes.FailedTrades.Count.ShouldEqual(1);
        It Should_Only_Execute_Eligible_Trades_P3_C2_Shares = () => player3.Portfolio.Where(c => c.CompanyId == company2Id).Count().ShouldEqual(40);
        It Should_Only_Execute_Eligible_Trades_P3_C1_Shares = () => player3.Portfolio.Where(c => c.CompanyId == company1Id).Count().ShouldEqual(0);
        It Should_Only_Execute_Eligible_Trades_P2_C2_Shares = () => player2.Portfolio.Where(c => c.CompanyId == company2Id).Count().ShouldEqual(260);
        It Should_Not_Trade_The_Wrong_Shares_P2_C1 = () => player2.Portfolio.Where(c => c.CompanyId == company1Id).Count().ShouldEqual(0);
        It Should_Only_Execute_Eligible_Trades_P1_C1_Shares = () => player1.Portfolio.Where(c => c.CompanyId == company1Id).Count().ShouldEqual(300);
        It Should_Not_Trade_The_Wrong_Shares_P1_C2 = () => player1.Portfolio.Where(c => c.CompanyId == company2Id).Count().ShouldEqual(0);
        It Should_Only_Execute_Eligible_Trades_P1_Cash = () => player1.Cash.ShouldEqual(0);
        It Should_Only_Execute_Eligible_Trades_P2_Cash = () => player2.Cash.ShouldEqual(200);
        It Should_Only_Execute_Eligible_Trades_P3_Cash = () => player3.Cash.ShouldEqual(8800);

        private static TradeService tradeService;
        private static List<OrderTicket> trades;
        private static TradeRegistry outcomes;
        private static List<TradeConfirmation> expectedTrades;
        private static TradeRegistry expectedOutcomes;
    }
}
