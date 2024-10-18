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
    public class With_A_Player_Repo
    {
        Establish context = () =>
        {
            gameId = Guid.NewGuid();
            playerRepoMock = new Mock<PlayerRepo>();
            assetRepoMock = new Mock<AssetRepo>();
        };
        protected static Guid gameId;
        protected static Mock<PlayerRepo> playerRepoMock;
        protected static Mock<AssetRepo> assetRepoMock;
    }

    public class When_Creating_A_Player : With_A_Player_Repo
    {
        Establish context = () =>
        {
            playerRepoMock.Setup(p => p.Save(Moq.It.IsAny<Player>()));
            playerService = new PlayerService(gameId, playerRepoMock.Object, assetRepoMock.Object);
            inputs = new List<(string,int)>
            {
                ("TestPlayer",100000)
            };
            outcomes = new List<Player>();
        };

        Because of = () =>
        {
            for (var i = 0; i < inputs.Count; i++)
            {
                outcomes.Add(playerService.CreatePlayer(inputs[i].name, inputs[i].cash));
            }
        };

        It Should_Return_A_Fully_Formed_Player = () =>
        {
            for (var i = 0; i < outcomes.Count; i++)
            {
                outcomes[i].GameId.ShouldEqual(gameId);
                outcomes[i].Name.ShouldEqual(inputs[i].name);
                outcomes[i].Cash.ShouldEqual(inputs[i].cash);
                outcomes[i].RiskTolerance.ShouldBeGreaterThan(0);
                outcomes[i].RiskTolerance.ShouldBeLessThan(1);
                outcomes[i].Portfolio.ShouldNotBeNull();
            }
        };

        It Should_Persist_The_Player = () =>
        {
            for (var i = 0; i < inputs.Count; i++)
            {
                playerRepoMock.Verify(p => p.Save(Moq.It.IsAny<Player>()), Times.Once);
            }
        };

        private static PlayerService playerService;
        private static List<(string name, int cash)> inputs;
        private static List<Player> outcomes;
    }

    public class When_Creating_Multiple_Player_Bots : With_A_Player_Repo
    {
        Establish context = () =>
        {
            playerRepoMock.Setup(p => p.Save(Moq.It.IsAny<Player>()));
            playerService = new PlayerService(gameId, playerRepoMock.Object, assetRepoMock.Object);
            inputs = new List<(int qty, int totalCash)>
            {
                (4,5000000)
            };
            outcomes = new List<List<Player>>();
        };

        Because of = () =>
        {
            for (var i = 0; i < inputs.Count; i++)
            {
                 outcomes.Add(playerService.CreatePlayerBots(gameId, inputs[i].qty, inputs[i].totalCash));
            }
        };

        It Should_Return_A_List_Of_Players = () =>
        {
            for (var i = 0; i < inputs.Count; i++)
            {
                for(var j=0; j < outcomes[i].Count; j++)
                {
                    outcomes[i][j].GameId.ShouldEqual(gameId);
                    outcomes[i][j].PlayerId.ShouldNotBeNull();
                    outcomes[i][j].Name.ShouldNotBeNull();
                    outcomes[i][j].Cash.ShouldBeGreaterThan(0);
                    outcomes[i][j].RiskTolerance.ShouldBeGreaterThan(0);
                    outcomes[i][j].RiskTolerance.ShouldBeLessThan(1);
                    outcomes[i][j].Portfolio.ShouldNotBeNull();
                }
            }
        };

        It Should_Persist_Each_Player = () =>
        {
            for (var i = 0; i < inputs.Count; i++)
            {
                playerRepoMock.Verify(p => p.Save(Moq.It.IsAny<Player>()), Times.Exactly(inputs[i].qty));
            }
        };

        private static PlayerService playerService;
        private static List<(int qty, int totalCash)> inputs;
        private static List<List<Player>> outcomes;
    }
}
