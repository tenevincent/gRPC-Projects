using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Infrastructure.Api;
using Microsoft.Extensions.Logging;

namespace gRPCFromWcf.Services
{
    public class ThingLogService : ThingLogger.ThingLoggerBase
    {
        private readonly ILogger<ThingLogService> _logger;

        public ThingLogService(ILogger<ThingLogService> logger)
        {
            this._logger = logger;
        }


         public override async Task<ConnectionClosedResponse> OpenConnection(IAsyncStreamReader<ThingLogRequest> requestStream, ServerCallContext context)
        {
            while (await requestStream.MoveNext(context.CancellationToken))
            {
                _logger.LogInformation(requestStream.Current.Description);
            }
            return new ConnectionClosedResponse();
        }
    }
}
