using Grpc.Core;
using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TraderSys.Portfolios;
using TraderSys.SimpleStockTickerServer.Protos;
using static TraderSys.Portfolios.Portfolios;

namespace TraderSys.ClientApp
{
    class Program
    {
        private const string ServerAddress = "https://localhost:5001";


        static async Task Main(string[] args)
        {
            await InitClient();

            var arguments = new List<string>();
            arguments.Add("$");
            arguments.Add("E");
            arguments.Add("CFA");
            arguments.Add("NAIRA");

            string[] argsNew = arguments.ToArray();
            await DoSimpleStockTickerAsync(argsNew);

            Console.ReadKey();
        }

        private static async Task DoSimpleStockTickerAsync(string[] args)
        {
            using var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new SimpleStockTicker.SimpleStockTickerClient(channel);
            var request = new SubscribeRequest();
            request.Symbols.AddRange(args);
            using var stream = client.Subscribe(request);
            var tokenSource = new CancellationTokenSource();
            var task = DisplayAsync(stream.ResponseStream, tokenSource.Token);

            Console.ReadKey();

            tokenSource.Cancel();
            await task;


        }

        static async Task DisplayAsync(IAsyncStreamReader<StockTickerUpdate> stream,CancellationToken token)
        {
            try
            {
                await foreach (var update in stream.ReadAllAsync(token))
                {
                    Console.WriteLine($"{update.Symbol}: {update.Price}");
                }
            }
            catch (RpcException e) when (e.StatusCode == StatusCode.Cancelled)
            {
                return;
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Finished.");
            }
        }




        private static async Task InitClient()
        {

            var channel = GrpcChannel.ForAddress(ServerAddress);
            var portfolios = new PortfoliosClient(channel);

            try
            {
                var request = new GetRequest
                {
                    TraderId = "68CB16F7-42BD-4330-A191-FA5904D2E5A0",
                    PortfolioId = 42
                };
                var response = await portfolios.GetAsync(request);

                Console.WriteLine($"Portfolio contains {response.Portfolio.Items.Count} items.");
            }
            catch (RpcException e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
