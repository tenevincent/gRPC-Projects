using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Infrastructure.Api;
namespace gRPCFromWcf.Services
{
    public class ClockStreamerService : ClockStreamer.ClockStreamerBase
    {

        public override async Task Subscribe(ClockSubscribeRequest request, IServerStreamWriter<ClockMessageResponse> responseStream, ServerCallContext context)
        {
            while (!context.CancellationToken.IsCancellationRequested)
            {
                var time = DateTimeOffset.UtcNow;
                await responseStream.WriteAsync(new ClockMessageResponse { Message = $"The time is {time.ToLocalTime()}." });
                await Task.Delay(TimeSpan.FromSeconds(10), context.CancellationToken);
            }
        }

    }
}
