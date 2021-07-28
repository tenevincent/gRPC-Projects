using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TraderSys.Portfolios.Protos;

namespace TraderSys.Portfolios.Extensions
{
    public static class PortfolioExtensions
    {

        public static Portfolio ConvertPortfolio(this Data.Models.PortfolioDao source)
        {
            if (source is null) return null;

            var target = new Portfolio
            {
                Id = source.Id,
                TraderId = source.TraderId.ToString(),
            };

            target.Items.AddRange(source.Items.Select(ConvertPortfolioItem));

            return target;
        }


        public static List<Portfolio> ConvertPortfolioItems(this List<Data.Models.PortfolioDao> sources)
        {

            List<Portfolio> listResult = new List<Portfolio>();


            if (sources is null) return listResult;

            foreach (var source in sources)
            {
                var target = new Portfolio
                {
                    Id = source.Id,
                    TraderId = source.TraderId.ToString(),
                };
                target.Items.AddRange(source.Items.Select(ConvertPortfolioItem));

                listResult.Add(target);
            }

            return listResult;
        }



        public static PortfolioItem ConvertPortfolioItem(this Data.Models.PortfolioItemDao source)
        {
            if (source is null) return null;

            return new PortfolioItem
            {
                Id = source.Id,
                ShareId = source.ShareId,
                Holding = source.Holding,
                Cost = Convert.ToDouble(source.Cost)
            };
        }


    }
}
