namespace MarketSimulator.Server.Entities
{
    public class OrderTicket
    {
        public Guid GameId;
        public Guid OrderId;
        public Player Buyer;
        public Player Seller;
        public Company Company;
        public int ShareCount;
        public double SharePrice;
    }
}
