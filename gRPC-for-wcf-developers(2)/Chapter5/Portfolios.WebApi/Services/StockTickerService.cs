using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TraderSys.Portfolios.Repositories;
using TraderSys.SimpleStockTickerServer.Protos;
using TraderSys.StockMarket;

namespace TraderSys.Portfolios.Services
{
    public class StockTickerService :  SimpleStockTicker.SimpleStockTickerBase
    {
        private readonly ILogger<StockTickerService> _logger;
        private readonly IStockPriceSubscriberFactory _subscriberFactory;
        public StockTickerService(ILogger<StockTickerService> logger, IStockPriceSubscriberFactory subscriberFactory)
        {
            this._logger = logger;
            this._subscriberFactory = subscriberFactory;
        }


        public override async Task Subscribe(SubscribeRequest request,   IServerStreamWriter<StockTickerUpdate> responseStream, ServerCallContext context)
        {
            var subscriber = _subscriberFactory.GetSubscriber(request.Symbols.ToArray());
            subscriber.Update += async (sender, args) =>
            {
                await WriteUpdateAsync(responseStream, args.Symbol, args.Price);
            };
      
            await AwaitCancellation(context.CancellationToken);
        }


        private async Task WriteUpdateAsync(IServerStreamWriter<StockTickerUpdate> stream,
string symbol, decimal price)
        {
            try
            {
                await stream.WriteAsync(new StockTickerUpdate
                {
                    Symbol = symbol,
                    PriceCents = (int)(price * 100),
                    Price = (double)price,
                    Time = Timestamp.FromDateTimeOffset(DateTimeOffset.UtcNow)
                });
            }
            catch (Exception e)
            {
                // Handle any errors caused by broken connection, etc.
                _logger.LogError($"Failed to write message: {e.Message}");
            }
        }


        private static Task AwaitCancellation(CancellationToken token)
        {
            var completion = new TaskCompletionSource<object>();
            token.Register(() => completion.SetResult(null));
            return completion.Task;
        }

    }
}
