namespace MarketSimulator.Server.Entities
{
    public class Player
    {
        public Guid GameId;
        public Guid PlayerId;
        public string Name;
        public decimal Cash;
        public double RiskTolerance;
        public List<Share> Portfolio;
    }
}
