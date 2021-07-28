using Grpc.Core;
using Infrastructure.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace gRPCFromWcf.Services
{
    public class ThingService : Things.ThingsBase
    {

        public override Task<GetThingResponse> Get(GetThingRequest request, ServerCallContext context)
        {
            // Get thing from database
            return Task.FromResult(new GetThingResponse
            {
                Message = "Hello " + request.Name
            });
        }

    }
}
