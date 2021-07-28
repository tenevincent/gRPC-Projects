using System;
using System.Collections.Generic;
using System.Text;

namespace TraderSys.Data.Models
{
    public class PortfolioItemDao
    {
        public int Id { get; set; }
        public int ShareId { get; set; }
        public int Holding { get; set; }
        public decimal Cost { get; set; }
    }
}
