using Grpc.Net.Client;
using GrpcServiceHelloWorld.Server;
using System;
using System.Threading.Tasks;

namespace GrpcServiceHelloWorld.Client
{
    class Program
    {
        static async Task Main(string[] args)
        {

            HelloReply response;

            using (var channel = GrpcChannel.ForAddress("https://localhost:5001"))
            {
                var client = new Greeter.GreeterClient(channel);
                response = await client.SayHelloAsync(new HelloRequest
                {
                    Name = "Vincent Tene",
                });
                Console.WriteLine(response.Message);


                Console.WriteLine("Hello World!");
                Console.ReadKey();
            }
        }
    }
}
