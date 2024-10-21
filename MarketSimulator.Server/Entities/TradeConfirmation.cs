namespace MarketSimulator.Server.Entities
{
    public class TradeConfirmation
    {
        public Guid GameId;
        public Guid TradeId;
        public Player Buyer;
        public Player Seller;
        public Company Company;
        public List<Share> Shares;
        public double SharePrice;
        public bool Success;
    }
}
