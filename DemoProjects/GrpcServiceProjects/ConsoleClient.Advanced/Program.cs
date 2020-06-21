using Grpc.Core;
using Grpc.Net.Client;
using GrpcServiceProjects.Advanced;
using GrpcServiceProjects.Advanced.Services;
using System;
using System.Threading.Tasks;

namespace ConsoleClient.Advanced
{
    class Program
    {
        static async Task Main(string[] args)
        {

             // await DoHandleSqrtAsync();

            await DoGreetingServiceWithDeadlines();


            Console.WriteLine("Hello World!");
        }



        private static async Task DoHandleSqrtAsync()
        {


            var channel02 = GrpcChannel.ForAddress("https://localhost:5002");
            //await channel02.ConnectAsync().ContinueWith((task) =>
            //{
            //    if (task.Status == TaskStatus.RanToCompletion)
            //        Console.WriteLine("The client connected successfully");
            //});

            var client02 = new SqrtService.SqrtServiceClient(channel02);
            // int number = 16;
            int[] numbers = { 16, 20, -25, -16 };

            for (int i = 0; i < numbers.Length; i++)
            {
                try
                {
                    var response = client02.Sqrt(new SqrtRequest() { Number = numbers[i] },
                                                deadline: DateTime.UtcNow.AddSeconds(3));

                    await Task.Delay(2000);
                    await Task.CompletedTask;
                    await Task.Delay(2000);

                    Console.WriteLine(response.SquareRoot);
                }
                catch (RpcException e) when (e.StatusCode == StatusCode.InvalidArgument)
                {
                    Console.WriteLine("Error : " + e.Status.Detail);
                }
                catch (RpcException e) when (e.StatusCode == StatusCode.DeadlineExceeded)
                {
                    Console.WriteLine("Deadline exceeded !");
                }
            }

        }

        private static async Task DoGreetingServiceWithDeadlines()
        {

            var channel = GrpcChannel.ForAddress("https://localhost:5002");
            var client = new Greeter.GreeterClient(channel);

            try
            {
                var response = await client.Greet_With_DeadlineAsync(new GreetingRequest() { Name = "John" },
                                                          deadline: DateTime.UtcNow.AddMilliseconds(1));

                Console.WriteLine(response.Result);
            }
            catch (RpcException e) when (e.StatusCode == StatusCode.DeadlineExceeded)
            {
                Console.WriteLine("Error : " + e.Status.Detail);
            }
        }



    }
}
