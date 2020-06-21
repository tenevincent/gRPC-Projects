using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace GrpcServiceProjects
{
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly ILogger<GreeterService> _logger;
        public GreeterService(ILogger<GreeterService> logger)
        {
            _logger = logger;
        }

        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            return Task.FromResult(new HelloReply
            {
                Message = "Hello " + request.Name
            });
        }



        public override async Task GreetManyTimes(GreetManyTimesRequest request, IServerStreamWriter<GreetManyTimesResponse> responseStream, ServerCallContext context)
        {
            Console.WriteLine("The server received the request : ");
            Console.WriteLine(request.ToString());

            string result = String.Format("hello {0} {1}", request.Greeting.FirstName, request.Greeting.LastName);

            foreach (int ii in Enumerable.Range(1, 50))
            {
                await responseStream.WriteAsync(new GreetManyTimesResponse() { Result = result + " " + ii });
                await Task.Delay(300);
            }
        }



        public override async Task<LongGreetResponse> LongGreet(IAsyncStreamReader<LongGreetRequest> requestStream, ServerCallContext context)
        {
            string result = "";

            while (await requestStream.MoveNext())
            {
                result += String.Format("Hello {0} {1} {2}",
                    requestStream.Current.Greeting.FirstName,
                    requestStream.Current.Greeting.LastName,
                    Environment.NewLine);
            }

            return new LongGreetResponse() { Result = result };
        }


        public override async Task GreetEveryone(IAsyncStreamReader<GreetEveryoneRequest> requestStream, IServerStreamWriter<GreetEveryoneResponse> responseStream, ServerCallContext context)
        {
            while (await requestStream.MoveNext())
            {
                var result = String.Format("Hello {0} {1}",
                                            requestStream.Current.Greeting.FirstName,
                                            requestStream.Current.Greeting.LastName);

                Console.WriteLine("Sending : " + result);
                await responseStream.WriteAsync(new GreetEveryoneResponse() { Result = result });
            }
        }




    }
}
 
