using Grpc.Net.Client;
using GrpcServiceProjects;
using GrpcServiceProjects.Services;
using System;
using System.Threading.Tasks;

namespace ConsoleClientApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // The port number(5001) must match the port of the gRPC server.
            using var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new Greeter.GreeterClient(channel);
            var reply = await client.SayHelloAsync(
                              new HelloRequest { Name = "GreeterClient" });
            Console.WriteLine("Greeting: " + reply.Message);
            Console.WriteLine("Press any key to exit...");



             var clientCalc = new Calculs.CalculsClient(channel);
            var res = await clientCalc.AddAsync(new ParamsIn() { Num = 10, Denum = 2 });
            Console.WriteLine($"Add is {res.Result}");


            
            //var result = clientCalc.Add(new ParamsIn() { Num = 10, Denum = 2 });
            //Console.WriteLine($"Add is {result.Result}");

            var resultRequest = await clientCalc.AddAsync(new ParamsIn() { Num = 10, Denum = 2 });
            Console.WriteLine($"AddAsync is {resultRequest.Result}");

            resultRequest = clientCalc.Add(new ParamsIn() { Num = 10, Denum = 2 });
            Console.WriteLine($"Add is {resultRequest.Result}");



            resultRequest = await clientCalc.DivideAsync(new ParamsIn() { Num = 10, Denum = 2 });
            Console.WriteLine($"Divide is {resultRequest.Result}");

            resultRequest = clientCalc.Divide(new ParamsIn() { Num = 10, Denum = 2 });
            Console.WriteLine($"Divide is {resultRequest.Result}");


            resultRequest = await clientCalc.SubstractAsync(new ParamsIn() { Num = 10, Denum = 2 });
            Console.WriteLine($"Substract is {resultRequest.Result}");

            resultRequest = await clientCalc.MultiplyAsync(new ParamsIn() { Num = 10, Denum = 2 });
            Console.WriteLine($"Multiply is {resultRequest.Result}");


            Console.ReadKey();
 
        }
    }
}
