using Average;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrpcServiceProjects.Services
{
    public class AverageService : Average.AverageService.AverageServiceBase
    {
        public override async Task<AverageResponse> ComputeAverage(IAsyncStreamReader<AverageRequest> requestStream, ServerCallContext context)
        {
            int total = 0;
            double result = 0.0;

            while (await requestStream.MoveNext())
            {
                result += requestStream.Current.Number;
                total++;
            }

            return new AverageResponse() { Result = result / total };
        }
    }
}
