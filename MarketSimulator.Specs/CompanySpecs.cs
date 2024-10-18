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
    public class With_A_Company_Repo
    {
        Establish context = () =>
        {
            gameId = Guid.NewGuid();
            assetRepoMock = new Mock<AssetRepo>();
            companyRepoMock = new Mock<CompanyRepo>();
        };

        protected static Guid gameId;
        protected static Mock<AssetRepo> assetRepoMock { get; private set; }
        protected static Mock<CompanyRepo> companyRepoMock { get; private set; }
    }
    public class When_Forming_A_Company : With_A_Company_Repo
    {
        Establish context = () =>
        {
            assetService = new AssetService(gameId, assetRepoMock.Object, companyRepoMock.Object);
            companyRepoMock.Setup(c => c.Save(Moq.It.IsAny<Company>()));
            assets = 3;
            inputs = new List<Company>
            {
                new Company
                {
                    GameId = gameId,
                    Debt = 3,
                    Name = "TestCompany",
                }
            };
            outcomes = new Dictionary<Company, Company>();
        };

        Because of = () =>
        {
            for (var i = 0; i < inputs.Count; i++)
            {
                Company company = assetService.FormCompany(inputs[i].GameId, inputs[i].Name, inputs[i].Debt, 1000000,assets);
                outcomes.Add(inputs[i],company);
            }
        };

        It Should_Return_A_Fully_Formed_Copmany = () =>
        {
            for (var i = 0; i < inputs.Count; i++)
            {
                var companyId = outcomes.Keys.ToList()[i].CompanyId;
                outcomes.Keys.ToList()[i].GameId.ShouldEqual(outcomes.Values.ToList()[i].GameId);
                outcomes.Keys.ToList()[i].CompanyId.ShouldNotBeNull();
                outcomes.Keys.ToList()[i].Debt.ShouldEqual(outcomes.Values.ToList()[i].Debt);
                outcomes.Keys.ToList()[i].Name.ShouldEqual(outcomes.Values.ToList()[i].Name);
                outcomes.Values.ToList()[i].Portfolio.ShouldNotBeNull();
                outcomes.Values.ToList()[i].Portfolio.Count.ShouldEqual(assets);
            }
        };

        It Should_Persist_The_Company = () =>
        {
            for (var i = 0; i < inputs.Count; i++)
            {
                companyRepoMock.Verify(c => c.Save(Moq.It.IsAny<Company>()), Times.Once());
            }
        };

        private static Dictionary<Company, Company> outcomes;
        private static Guid asset1Id;
        private static Guid asset2Id;
        private static Asset asset1;
        private static Asset asset2;
        private static List<Asset> portfolio;
        private static int assets;
        private static List<Company> inputs;
        protected static AssetService assetService;
    }
}
