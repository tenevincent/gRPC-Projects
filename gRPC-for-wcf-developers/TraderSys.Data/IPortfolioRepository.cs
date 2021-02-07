using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TraderSys.Data.Models;

namespace TraderSys.Data
{
    public interface IPortfolioRepository
    {
        Task<PortfolioDao> GetAsync(Guid traderId, int portfolioId);
        Task<List<PortfolioDao>> GetAllAsync(Guid traderId);
    }
}
