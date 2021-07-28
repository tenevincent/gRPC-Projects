using Grpc.Core;
using Grpc.Net.Client;
using Infrastructure.Api;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ClientApp
{
    class Program
    {
        private static GrpcChannel channel = GrpcChannel.ForAddress("https://localhost:5001");
        private static Greeter.GreeterClient client = new Greeter.GreeterClient(channel);
        private static CancellationTokenSource tokenSource = new CancellationTokenSource();


        static async Task Main(string[] args)
        {

            await ExecuteGreetingAsync();

            // var token = tokenSource.Token;
            // await TellTheTimeAsync(token);

            await ClientStreamingAsync();


            Console.ReadKey();


        }

        private static async Task ClientStreamingAsync()
        {
            int counter = 0;
            var channel = GrpcChannel.ForAddress("https://localhost:5001");
            ThingLogger.ThingLoggerClient client = new ThingLogger.ThingLoggerClient(channel);
            var thingLoggerClient = new ThingLoggerClient(client);
            var stream = client.OpenConnection();

            // await stream.RequestStream.MoveNext()
            while (counter < 20)
            {
                ++counter;
                await stream.RequestStream.WriteAsync(
                   new ThingLogRequest()
                   {
                       Time = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(DateTime.UtcNow),
                       Description = $"The logger is now send from by the client, {DateTime.Now.ToString()}"
                   });
                await Task.Delay(1000);
            }

            await stream.RequestStream.CompleteAsync();
        }

        private static async Task ExecuteGreetingAsync()
        {
            var metadata = new Metadata();

            metadata.Add("Requester", "Vincent Tene");
            metadata.Add("Email", "vincent.tene@gmail.com");

            try
            {
                var reply = await client.SayHelloAsync(new HelloRequest { Name = "GreeterClient" }, metadata);
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.PermissionDenied)
            {
                 Console.WriteLine($"User '{ex.Message}' does not have permission to view this portfolio.");
            }
            catch (RpcException)
            {
                // Handle any other error type ...
            }


            Console.WriteLine("Greeting: " + reply.Message);
            Console.WriteLine("Press any key to exit...");
        }

        private static async Task TellTheTimeAsync(CancellationToken token)
        {
            var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new ClockStreamer.ClockStreamerClient(channel);

            var request = new ClockSubscribeRequest();
            var response = client.Subscribe(request);

            await foreach (var update in response.ResponseStream.ReadAllAsync(token))
            {
                Console.WriteLine(update.Message);
            }
        }


    }
}
