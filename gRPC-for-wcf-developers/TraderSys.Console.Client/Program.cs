using Grpc.Core;
using Grpc.Net.Client;
using System;
using System.Threading;
using System.Threading.Tasks;
using TraderSys.Portfolios.Protos;

namespace TraderSys.ConsoleClient
{
  public  class Program
    {
        public static async Task Main(string[] args)
        {
           // await RequestResponsegRPCClientDemo(args);


            await gRPCDuplexClientDemo(args);



        }

        private async static Task gRPCDuplexClientDemo(string[] args)
        {
            using GrpcChannel channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new SimpleStockTicker.SimpleStockTickerClient(channel);

            var request = new SubscribeRequest();
            request.Symbols.AddRange(args);
            using var stream = client.Subscribe(request);

            var tokenSource = new CancellationTokenSource();
            var task = DisplayAsync(stream.ResponseStream, tokenSource.Token);

            WaitForExitKey();

            tokenSource.Cancel();
            await task;
        }

        private static async Task RequestResponsegRPCClientDemo(string[] args)
        {
            GetResponse response;

            using (var channel = GrpcChannel.ForAddress("https://localhost:5001"))
            {
                var client = new Portfolios.Protos.Portfolios.PortfoliosClient(channel);

                response = await client.GetAsync(new GetRequest
                {
                    TraderId = args[0],
                    PortfolioId = int.Parse(args[1])
                });
            }

            foreach (var item in response.Portfolio.Items)
            {
                System.Console.WriteLine($"Holding {item.Holding} of Share ID {item.ShareId}.");
            }
        }

        static async Task DisplayAsync(IAsyncStreamReader<StockTickerUpdate> stream, CancellationToken token)
        {
            try
            {
                await foreach (var update in stream.ReadAllAsync(token))
                {
                    Console.WriteLine($"{update.Symbol}: {update.Price}");
                }
            }
            catch (RpcException e)
            {
                if (e.StatusCode == StatusCode.Cancelled)
                {
                    return;
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Finished.");
            }
        }

        static void WaitForExitKey()
        {
            Console.WriteLine("Press E to exit...");

            char ch = ' ';

            while (ch != 'e')
            {
                ch = char.ToLowerInvariant(Console.ReadKey().KeyChar);
            }
        }






    }
}
