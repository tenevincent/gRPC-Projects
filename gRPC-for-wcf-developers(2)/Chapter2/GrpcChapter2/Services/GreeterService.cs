using Common.Library;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using TraderSys;
using static TraderSys.Order.Types;

namespace GrpcChapter2
{
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly ILogger<GreeterService> _logger;
        public GreeterService(ILogger<GreeterService> logger)
        {
            _logger = logger;

           // Init();

        }



        public override Task<HelloResponse> SayHello(HelloRequest request, ServerCallContext context)
        {
            

            return Task.FromResult(new HelloResponse
            {
                Message = "Hello " + request.Name
            });
        }

        //private static void Init()
        //{
        //    Stock stock = new();
        //    var inner = new Outer.Types.Inner { Text = "Hello" };

        //    var persons = new Person();
        //    var listPersons = persons.Aliases.ToList();

        //    AccountStatus accountStatus = AccountStatus.Closed;

        //    Order order = new();
        //    var attribute = new Attributes();
        //    attribute.Values["data"] = "data";
        //    order.Attributes.Add(attribute);
        //}

    }
}
