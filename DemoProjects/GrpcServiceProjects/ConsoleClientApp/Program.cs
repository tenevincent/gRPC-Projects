using Average;
using Grpc.Core;
using Grpc.Net.Client;
using GrpcServiceProjects;
using GrpcServiceProjects.Advanced.Services;
using GrpcServiceProjects.Services;
using Max;
using Prime;
using System;
using System.Linq;
using System.Threading.Tasks;


namespace ConsoleClientApp
{
    class Program
    {
        private static GrpcChannel channel = GrpcChannel.ForAddress("https://localhost:5001");
        private static Greeter.GreeterClient client = new Greeter.GreeterClient(channel);

        static async Task Main(string[] args)
        {
            // The port number(5001) must match the port of the gRPC server.



            //await DoCallSingleCallAsync();

            //await DoManyGreetings();

            //await DoCallCalculatorServiceAsync();

            //   await DoExecutePrimeNumberAsync();

            // await DoLongGreet();


            //  await DoAverageAsync();

            // await DoGreetEveryoneAsync();

            // await DoFindMaximumAsync();

           

            Console.ReadKey();

        }


        private static async Task DoFindMaximumAsync()
        {

            var clientMax= new FindMaxService.FindMaxServiceClient(channel);
            var stream = clientMax.findMaximum();

            var responseReaderTask = Task.Run(async () =>
            {
                while (await stream.ResponseStream.MoveNext())
                    Console.WriteLine(stream.ResponseStream.Current.Max);
            });

            int[] numbers = { 1, 5, 3, 6, 2, 20 };

            foreach (var number in numbers)
            {
                await stream.RequestStream.WriteAsync(new FindMaxRequest() { Number = number });
            }

            await stream.RequestStream.CompleteAsync();
            await responseReaderTask;
        }

        private static async Task DoAverageAsync()
        {

            var client = new AverageService.AverageServiceClient(channel);
            var stream = client.ComputeAverage();

            foreach (int number in Enumerable.Range(1, 4))
            {
                var request = new AverageRequest() { Number = number };

                await stream.RequestStream.WriteAsync(request);
            }

            await stream.RequestStream.CompleteAsync();

            var response = await stream.ResponseAsync;

            Console.WriteLine(response.Result);

        }




        private static async Task DoLongGreet()
        {
            var greeting = new Greeting()
            {
                FirstName = "Clement",
                LastName = "Jean"
            };

            var request = new LongGreetRequest() { Greeting = greeting };
            var stream = client.LongGreet();

            foreach (int i in Enumerable.Range(1, 20))
            {
                //request.Greeting += " from the client " + i + "...";
                await stream.RequestStream.WriteAsync(request);
            }

            await stream.RequestStream.CompleteAsync();

            var response = await stream.ResponseAsync;

            Console.WriteLine(response.Result);
        }


        private static async Task DoGreetEveryoneAsync()
        {
            var stream = client.GreetEveryone();

            var responseReaderTask = Task.Run(async () =>
            {
                while (await stream.ResponseStream.MoveNext())
                {
                    Console.WriteLine("Received : " + stream.ResponseStream.Current.Result);
                }
            });

            Greeting[] greetings =
            {
                new Greeting() { FirstName = "John", LastName = "Doe" },
                new Greeting() { FirstName = "Clement", LastName = "Jean" },
                new Greeting() { FirstName = "Patricia", LastName = "Hertz" }
            };

            foreach (var greeting in greetings)
            {
                Console.WriteLine("Sending : " + greeting.ToString());
                await stream.RequestStream.WriteAsync(new GreetEveryoneRequest()
                {
                    Greeting = greeting
                });
            }

            await stream.RequestStream.CompleteAsync();
            await responseReaderTask;
        }




        private static async Task DoExecutePrimeNumberAsync()
        {
            var client = new PrimeNumberService.PrimeNumberServiceClient(channel);

            var request = new PrimeNumberDecompositionRequest()
            {
                Number = 120
            };

            var response = client.PrimeNumberDecomposition(request);

            while (await response.ResponseStream.MoveNext())
            {
                Console.WriteLine(response.ResponseStream.Current.PrimeFactor);
                await Task.Delay(200);
            }

        }


        public static async Task DoManyGreetings()
        {


            var greeting = new Greeting()
            {
                FirstName = "Clement",
                LastName = "Jean"
            };

            var request = new GreetManyTimesRequest() { Greeting = greeting };
            var response = client.GreetManyTimes(request);

            while (await response.ResponseStream.MoveNext())
            {
                Console.WriteLine(response.ResponseStream.Current.Result);
                await Task.Delay(200);
            }
        }



        private static async Task DoCallSingleCallAsync()
        {
            var reply = await client.SayHelloAsync(new HelloRequest { Name = "GreeterClient" });
            Console.WriteLine("Greeting: " + reply.Message);
            Console.WriteLine("Press any key to exit...");
             
        }


        private static async Task DoCallCalculatorServiceAsync()
        {
            var client = new Calculs.CalculsClient(channel);
            var res = await client.AddAsync(new ParamsIn() { Num = 10, Denum = 2 });
            Console.WriteLine($"Add is {res.Result}");


            var resultRequest = await client.AddAsync(new ParamsIn() { Num = 10, Denum = 2 });
            Console.WriteLine($"AddAsync is {resultRequest.Result}");

            resultRequest = client.Add(new ParamsIn() { Num = 10, Denum = 2 });
            Console.WriteLine($"Add is {resultRequest.Result}");


            resultRequest = await client.DivideAsync(new ParamsIn() { Num = 10, Denum = 2 });
            Console.WriteLine($"Divide is {resultRequest.Result}");

            resultRequest = client.Divide(new ParamsIn() { Num = 10, Denum = 2 });
            Console.WriteLine($"Divide is {resultRequest.Result}");


            resultRequest = await client.SubstractAsync(new ParamsIn() { Num = 10, Denum = 2 });
            Console.WriteLine($"Substract is {resultRequest.Result}");

            resultRequest = await client.MultiplyAsync(new ParamsIn() { Num = 10, Denum = 2 });
            Console.WriteLine($"Multiply is {resultRequest.Result}");
        }








    }
}
