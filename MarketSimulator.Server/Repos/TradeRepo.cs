using MarketSimulator.Server.Entities;

namespace MarketSimulator.Server.Repos
{
    public interface ITradeRepo
    {
        void Save(TradeConfirmation tradeTicket);
    }
    public class TradeRepo
    {
        public virtual void Save(TradeConfirmation tradeTicket)
        {
            return;
        }
    }
}
