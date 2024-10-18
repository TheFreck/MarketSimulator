using MarketSimulator.Server.Entities;
using MarketSimulator.Server.Repos;

namespace MarketSimulator.Server.Services
{
    public class PlayerService
    {
        Guid gameId;
        PlayerRepo playerRepo;
        AssetRepo assetRepo;
        Random randy;
        public PlayerService(Guid gameId, PlayerRepo playerRepo, AssetRepo assetRepo)
        {
            this.gameId = gameId;
            this.playerRepo = playerRepo;
            this.assetRepo = assetRepo;
            this.randy = new Random();
        }

        public Player CreatePlayer(string name, int cash)
        {
            Player player = new Player
            {
                GameId = gameId,
                Name = name,
                Cash = cash,
                PlayerId = Guid.NewGuid(),
                RiskTolerance = randy.NextDouble(),
                Portfolio = new List<Share>()
            };

            playerRepo.Save(player);

            return player;
        }

        public List<Player> CreatePlayerBots(Guid gameId, int qty, int totalCash)
        {
            var players = new List<Player>();
            var remainingCash = totalCash;
            for(var i=0; i<qty-1; i++)
            {
                players.Add(CreatePlayer($"playerBot-{i}", randy.Next(remainingCash / 2)));
            }
            players.Add(CreatePlayer($"playerBot-{qty-1}", remainingCash));

            return players;
        }
    }
}
