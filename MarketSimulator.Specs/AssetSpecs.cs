using Machine.Specifications;
using MarketSimulator.Server.Entities;
using MarketSimulator.Server.Repos;
using MarketSimulator.Server.Services;
using Moq;
using It = Machine.Specifications.It;

namespace MarketSimulator.Specs
{
    public class With_An_Asset_Repo
    {
        Establish context = () =>
        {
            gameId = Guid.NewGuid();
            assetRepoMock = new Mock<AssetRepo>();
            companyRepoMock = new Mock<CompanyRepo>();
        };

        protected static Guid gameId;
        protected static Mock<AssetRepo> assetRepoMock;
        protected static Mock<CompanyRepo> companyRepoMock;
    }
    public class When_Creating_An_Asset : With_An_Asset_Repo
    {
        Establish context = () =>
        {
            assetRepoMock.Setup(a => a.Save(Moq.It.IsAny<Asset>()));
            assetService = new AssetService(gameId, assetRepoMock.Object, companyRepoMock.Object);
            inputs = new List<(int,int)>
            {
                (1000000,1)
            };
            outcomes = new List<List<Asset>>();
        };

        Because of = () =>
        {
            for (var i = 0; i < inputs.Count; i++)
            {
                List<Asset> assets = assetService.FormAssets(inputs[i].value, inputs[i].assets);
                outcomes.Add(assets);
            }
        };

        It Should_Return_Fully_Formed_Assets = () =>
        {
            for (var i = 0; i < inputs.Count; i++)
            {
                for(var j=0; j< inputs[i].assets; j++)
                {
                    outcomes[i][j].GameId.ShouldEqual(gameId);
                    outcomes[i][j].AssetId.ShouldNotBeNull();
                    outcomes[i][j].PrimaryIndustry.ShouldNotBeNull();
                    outcomes[i][j].SecondaryIndustry.ShouldNotBeNull();

                }
                 outcomes[i].Select(a => a.Value).Sum().ShouldEqual(inputs[i].value);
            }
        };

        It Should_Persist_The_Asset = () =>
        {
            for (var i = 0; i < inputs.Count; i++)
            {
                assetRepoMock.Verify(a => a.Save(Moq.It.IsAny<Asset>()), Times.Once());
            }
        };

        private static AssetService assetService;
        private static List<(int value, int assets)> inputs;
        private static List<List<Asset>> outcomes;
    }
}