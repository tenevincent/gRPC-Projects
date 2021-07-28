using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TraderSys.Portfolios.Repositories
{
    public interface IPortfolioRepository
    {
        Task<Portfolio> GetAsync(Guid traderId, int portfolioId);
        Task<List<Portfolio>> GetAllAsync(Guid traderId);
    }

    public class PortfolioRepository : IPortfolioRepository
    {
        public Task<Portfolio> GetAsync(Guid traderId, int portfolioId)
        {
            return Task.FromResult(Fake(traderId, portfolioId));
        }
        public Task<List<Portfolio>> GetAllAsync(Guid traderId)
        {
            var list = new List<Portfolio>(4);

            // For test data, use Guid bytes as integer Id values
            var bytes = traderId.ToByteArray();
            for (int i = 0; i < 16; i += 4)
            {
                int id = BitConverter.ToInt32(bytes, i);
                list.Add(Fake(traderId, id));
            }

            return Task.FromResult(list);
        }

        private Portfolio Fake(Guid traderId, int portfolioId)
        {
            var portfolio = new Portfolio
            {
                Id = portfolioId,
                TraderId = traderId.ToString(),
            };

            var result = FakeItems(portfolioId).ToList();
            result.ForEach( item => portfolio.Items.Add(item));

            return portfolio;
        }

        private IEnumerable<PortfolioItem> FakeItems(int portfolioId)
        {
            var random = new Random(portfolioId);
            int count = random.Next(2, 8);
            for (int i = 0; i < count; i++)
            {
                yield return new PortfolioItem
                {
                    Id = random.Next(),
                    ShareId = random.Next(),
                    Cost = Convert.ToDouble(random.NextDouble() * 10),
                    //Cost = Convert.ToDecimal(random.NextDouble() * 10),
                    Holding = random.Next(999999)
                };
            }
        }



    }
}
