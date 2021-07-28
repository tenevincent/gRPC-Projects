using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Infrastructure.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientApp
{
    public class ThingLoggerClient : IAsyncDisposable
    {
        private readonly ThingLogger.ThingLoggerClient _client;
        private readonly AsyncClientStreamingCall<ThingLogRequest, ConnectionClosedResponse> _stream;

        public ThingLoggerClient(ThingLogger.ThingLoggerClient client)
        {
            _client = client;
            _stream = client.OpenConnection();
        }

        public async Task WriteAsync(string description)
        {
            await _stream.RequestStream.WriteAsync(new ThingLogRequest
            {
                Description = description,
                Time = Timestamp.FromDateTimeOffset(DateTimeOffset.UtcNow)
            });
        }

        public async ValueTask DisposeAsync()
        {
            await _stream.RequestStream.CompleteAsync();
            _stream.Dispose();
        }
    }
}
