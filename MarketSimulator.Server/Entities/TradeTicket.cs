namespace MarketSimulator.Server.Entities
{
    public class TradeTicket
    {
        public Guid GameId;
        public Player Buyer;
        public Player Seller;
        public Company Company;
        public List<Share> Shares;
        public decimal SharePrice;
    }
}
