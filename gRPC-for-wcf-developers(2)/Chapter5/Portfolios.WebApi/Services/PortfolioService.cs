using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using TraderSys.Portfolios.Repositories;

namespace TraderSys.Portfolios
{
    public class PortfolioService : Portfolios.PortfoliosBase
    {
        private readonly IPortfolioRepository _repository;
        private readonly ILogger<PortfolioService> _logger;
        public PortfolioService(IPortfolioRepository portfolioRepository, ILogger<PortfolioService> logger)
        {
            this._repository = portfolioRepository;
            _logger = logger;
        }

        public async override Task<GetResponse> Get(GetRequest request, ServerCallContext context)
        {
            if (!Guid.TryParse(request.TraderId, out var traderId))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "traderId must be a UUID"));
            }

            var portfolio = await _repository.GetAsync(traderId, request.PortfolioId);

            return new GetResponse
            {
                Portfolio =  portfolio 
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
            response.Portfolios.AddRange(portfolios);

            return response;
        }

    }
}
