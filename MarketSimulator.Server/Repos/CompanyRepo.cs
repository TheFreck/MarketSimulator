using MarketSimulator.Server.Entities;

namespace MarketSimulator.Server.Repos
{
    public interface ICompanyRepo
    {
        void Save(Company company);
    }
    public class CompanyRepo : ICompanyRepo
    {
        public virtual void Save(Company company)
        {
            return;
        }
    }
}