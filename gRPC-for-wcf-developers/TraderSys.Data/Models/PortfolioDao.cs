using System;
using System.Collections.Generic;
using System.Text;

namespace TraderSys.Data.Models
{
    public class PortfolioDao
    {
        public int Id { get; set; }
        public Guid TraderId { get; set; }
        public List<PortfolioItemDao> Items { get; set; }
    }
}
