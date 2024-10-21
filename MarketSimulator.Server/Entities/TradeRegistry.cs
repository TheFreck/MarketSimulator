using MarketSimulator.Server.Services;

namespace MarketSimulator.Server.Entities
{
    public class TradeRegistry
    {
        public List<OrderTicket> Orders = new List<OrderTicket>();
        public List<TradeConfirmation> SuccessfulTrades = new List<TradeConfirmation>();
        public List<TradeConfirmation> FailedTrades = new List<TradeConfirmation>();
    }
}
