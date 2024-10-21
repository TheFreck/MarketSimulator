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
                Company company = assetService.FormCompany(inputs[i].GameId, inputs[i].Name, inputs[i].Debt, 1000000, assets);
                outcomes.Add(inputs[i], company);
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

    public class When_Processing_Growth_For_A_Company : With_A_Company_Repo
    {
        Establish context = () =>
        {
            assetService = new AssetService(gameId, assetRepoMock.Object, companyRepoMock.Object);
            companyId = Guid.NewGuid();
            industryInputs = new Dictionary<IndustryTypes, double>
            {
                { IndustryTypes.Red, .15 },
                { IndustryTypes.Orange, .2 },
                { IndustryTypes.Yellow, .25 },
                { IndustryTypes.Green, .3 },
                { IndustryTypes.Blue, .35 },
                { IndustryTypes.Violet, .4 }
            };
            assets = new List<Asset>
            {
                new Asset
                {
                    GameId = gameId,
                    AssetId = Guid.NewGuid(),
                    CompanyId = companyId,
                    PrimaryIndustry = IndustryTypes.Red,
                    SecondaryIndustry = IndustryTypes.Orange,
                    Value = 100000
                },
                new Asset
                {
                    GameId=gameId,
                    AssetId=Guid.NewGuid(),
                    CompanyId=companyId,
                    PrimaryIndustry=IndustryTypes.Yellow,
                    SecondaryIndustry=IndustryTypes.Green,
                    Value = 100000
                },
                new Asset
                {
                    GameId=gameId,
                    AssetId=Guid.NewGuid(),
                    CompanyId=companyId,
                    PrimaryIndustry=IndustryTypes.Blue,
                    SecondaryIndustry=IndustryTypes.Violet,
                    Value = 100000
                }
            };
            grownAssets = new List<Asset>
            {
                new Asset
                {
                    GameId = gameId,
                    AssetId = Guid.NewGuid(),
                    CompanyId = companyId,
                    PrimaryIndustry = IndustryTypes.Red,
                    SecondaryIndustry = IndustryTypes.Orange,
                    Value = 116667
                },
                new Asset
                {
                    GameId=gameId,
                    AssetId=Guid.NewGuid(),
                    CompanyId=companyId,
                    PrimaryIndustry=IndustryTypes.Yellow,
                    SecondaryIndustry=IndustryTypes.Green,
                    Value = 126667
                },
                new Asset
                {
                    GameId=gameId,
                    AssetId=Guid.NewGuid(),
                    CompanyId=companyId,
                    PrimaryIndustry=IndustryTypes.Blue,
                    SecondaryIndustry=IndustryTypes.Violet,
                    Value = 136667
                }
            };
            company = new Company
            {
                GameId = gameId,
                CompanyId = companyId,
                Name = "CompanyTest",
                Debt = 4,
                Portfolio = assets
            };
            expectedCompany = new Company
            {
                GameId = gameId,
                CompanyId = companyId,
                Name = "CompanyTest",
                Debt = 4,
                Portfolio = assets
            };
            expectedGrowth = 380000;
        };

        Because of = () => outcome = assetService.GrowCompany(company,industryInputs);

        It Should_Grow_Assets_And_Copmany_Value = () =>
        {
            expectedCompany.GameId.ShouldEqual(gameId);
            expectedCompany.CompanyId.ShouldEqual(companyId);
            expectedCompany.Name.ShouldEqual(company.Name);
            expectedCompany.Debt.ShouldEqual(company.Debt);
            expectedCompany.Portfolio.Count.ShouldEqual(company.Portfolio.Count);
            expectedCompany.Value.ShouldEqual(expectedGrowth);
        };

        It Should_Persist_Changes_In_Company_Value = () => companyRepoMock.Verify(a => a.Save(Moq.It.IsAny<Company>()),Times.Once);

        private static AssetService assetService;
        private static Guid companyId;
        private static Dictionary<IndustryTypes, double> industryInputs;
        private static List<Asset> assets;
        private static List<Asset> grownAssets;
        private static Company company;
        private static Company expectedCompany;
        private static int expectedGrowth;
        private static Company outcome;
    }
}
