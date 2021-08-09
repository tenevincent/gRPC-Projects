using Google.Protobuf;
using Grpc.Core;
using GrpcDataStreaming.Server.Helper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GrpcDataStreaming.Server.Services
{
    /// <summary>
    /// https://blog.noser.com/grpc-tutorial-teil-2-streaming-mit-grpc/
    /// </summary>
    public class RcpStreamingService : StreamingService.StreamingServiceBase
    {
        //private readonly int ImageChunkSize = 1000;
        private readonly ILogger<RcpStreamingService> _logger;
        private readonly StreamingHelper<RcpStreamingService> _handler;

        public RcpStreamingService(ILogger<RcpStreamingService> logger)
        {
            this._logger = logger;
            this._handler = new StreamingHelper<RcpStreamingService>(_logger);

        }

        public override async Task DownloadPersonImage(PersonMessage request, IServerStreamWriter<PersonImageMessage> responseStream, ServerCallContext context)
        {
            int imageChunkSize = 100;
            // TODO:
            // var directoryPath = @"C:\Users\Tene\AppData\Local\GannDialog\GannDialog 1.1\AppsDB\APP\LOG\LOG02.LOG";

            //var allFiles = Directory.EnumerateFiles(directoryPath);


            //foreach (var item in allFiles)
            //{
            string imageFileName = @"C:\Users\Tene\AppData\Local\GannDialog\GannDialog 1.1\AppsDB\AppImage\projec01_sdcard_conf_charge02.PNG";

            request.FileName = imageFileName;
            //-------------------------------------
            await _handler.DownloadFileAsync(request, responseStream, imageChunkSize).ConfigureAwait(false);

        }



        public override async Task<TransferStatusMessage> UploadPersonImage(IAsyncStreamReader<PersonImageMessage> requestStream, ServerCallContext context)
        {
            // TODO:
            //string imageFileName = null;
            //-------------------------------------
            return await _handler.UploadPersonImageAsync(requestStream, context);

        }





    }
}
