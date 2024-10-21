using MarketSimulator.Server.Entities;

namespace MarketSimulator.Server.Repos
{
    public interface IPlayerRepo
    {
        void Save(Player player);
    }
    public class PlayerRepo : IPlayerRepo
    {
        public virtual void Save(Player player)
        {
            throw new NotImplementedException();
        }
    }
}
