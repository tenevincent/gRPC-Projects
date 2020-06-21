using Grpc.Core;
using Max;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrpcServiceProjects.Services
{
    public class FindMaxService : Max.FindMaxService.FindMaxServiceBase
    {
        public override async Task findMaximum(IAsyncStreamReader<FindMaxRequest> requestStream, IServerStreamWriter<FindMaxResponse> responseStream, ServerCallContext context)
        {
            int? max = null;

            while (await requestStream.MoveNext())
            {
                if (max == null || max < requestStream.Current.Number)
                {
                    max = requestStream.Current.Number;
                    await responseStream.WriteAsync(new FindMaxResponse() { Max = max.Value });
                }
            }
        }
    }
}
