using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TraderSys.Data.Models;

namespace TraderSys.Data
{
    public class PortfolioRepository : IPortfolioRepository
    {
        public Task<PortfolioDao> GetAsync(Guid traderId, int portfolioId)
        {
            return Task.FromResult(Fake(traderId, portfolioId));
        }
        public Task<List<PortfolioDao>> GetAllAsync(Guid traderId)
        {
            var list = new List<PortfolioDao>(4);

            // For test data, use Guid bytes as integer Id values
            var bytes = traderId.ToByteArray();
            for (int i = 0; i < 16; i += 4)
            {
                int id = BitConverter.ToInt32(bytes, i);
                list.Add(Fake(traderId, id));
            }

            return Task.FromResult(list);
        }

        private PortfolioDao Fake(Guid traderId, int portfolioId)
        {
            return new PortfolioDao
            {
                Id = portfolioId,
                TraderId = traderId,
                Items = FakeItems(portfolioId).ToList()
            };
        }

        private IEnumerable<PortfolioItemDao> FakeItems(int portfolioId)
        {
            var random = new Random(portfolioId);
            int count = random.Next(2, 8);
            for (int i = 0; i < count; i++)
            {
                yield return new PortfolioItemDao
                {
                    Id = random.Next(),
                    ShareId = random.Next(),
                    Cost = Convert.ToDecimal(random.NextDouble() * 10),
                    Holding = random.Next(999999)
                };
            }
        }

    }
    }
