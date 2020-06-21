using Grpc.Core;
using Prime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrpcServiceProjects.Services
{
    public class PrimeNumberService : Prime.PrimeNumberService.PrimeNumberServiceBase
    {
        public override async Task PrimeNumberDecomposition(PrimeNumberDecompositionRequest request, IServerStreamWriter<PrimeNumberDecompositionResponse> responseStream, ServerCallContext context)
        {
            Console.WriteLine("The server received the request :");
            Console.WriteLine(request.ToString());

            int number = request.Number;
            int divisor = 2;

            while (number > 1)
            {
                if (number % divisor == 0)
                {
                    number /= divisor;
                    await responseStream.WriteAsync(new PrimeNumberDecompositionResponse() { PrimeFactor = divisor });
                }
                else
                    divisor++;
            }
        }
    }
}
