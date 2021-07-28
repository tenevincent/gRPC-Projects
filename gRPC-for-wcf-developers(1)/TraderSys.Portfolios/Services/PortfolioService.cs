using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using TraderSys.Data;
using TraderSys.Data.Models;
using TraderSys.Portfolios.Extensions;
using TraderSys.Portfolios.Protos;


namespace TraderSys.Portfolios
{
    public class PortfolioService : Portfolios.Protos.Portfolios.PortfoliosBase
    {
        private readonly IPortfolioRepository _repository;
        private readonly ILogger<PortfolioService> _logger;



        public PortfolioService(ILogger<PortfolioService> logger, IPortfolioRepository repository)
        {
            this._logger = logger;
            this._repository = repository;
        }


        public async override Task<GetResponse> Get(GetRequest request, ServerCallContext context)
        {
            if (!Guid.TryParse(request.TraderId, out var traderId))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "traderId must be a UUID"));
            }

            PortfolioDao portfolio = await _repository.GetAsync(traderId, request.PortfolioId);

            return new GetResponse
            {
                Portfolio = portfolio.ConvertPortfolio()
            }; 
        }

        public override async Task<GetAllResponse> GetAll(GetAllRequest request, ServerCallContext context)
        {
            if (!Guid.TryParse(request.TraderId, out var traderId))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "traderId must be a UUID"));
            }

            var portfolios = await _repository.GetAllAsync(traderId);
 
            var response = new GetAllResponse();
            response.Portfolios.AddRange(portfolios.ConvertPortfolioItems().AsEnumerable());

            return response; 

        }

      

    }
}
