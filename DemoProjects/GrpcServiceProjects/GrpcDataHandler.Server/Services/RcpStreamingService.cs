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
    public class RcpStreamingService :  StreamingService.StreamingServiceBase
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
                string imageFileName = @"C:\Users\Tene\AppData\Local\GannDialog\GannDialog 1.1\AppsDB\APP\LOG\LOG03.LOG";

            request.FileName = imageFileName;
                //-------------------------------------
                await _handler.DownloadFileAsync(request, responseStream,  imageChunkSize).ConfigureAwait(false);

           // }


        }

   

        public override async Task<TransferStatusMessage> UploadPersonImage(IAsyncStreamReader<PersonImageMessage> requestStream, ServerCallContext context)
        {
            // TODO:
            string imageFileName = null;
            //-------------------------------------

            return await _handler.UploadPersonImageAsync(requestStream, context, imageFileName);

        }


        //private async Task DownloadFileAsync(PersonMessage request, IServerStreamWriter<PersonImageMessage> responseStream, string ImageFileName, int ImageChunkSize)
        //{

        //    // Example of exception
        //    if (File.Exists(ImageFileName) == false)
        //    {
        //        _logger.LogError($"File '{ImageFileName}' not found.");
        //        Metadata metadata = new Metadata() { { "Filename", ImageFileName } };
        //        throw new RpcException(new Status(StatusCode.NotFound, "Image file not found."),
        //            metadata, "More details for the exception...");
        //    }
        //    PersonImageMessage personImageMessage = new PersonImageMessage();
        //    personImageMessage.TransferStatusMessage = new TransferStatusMessage();
        //    personImageMessage.TransferStatusMessage.Status = TransferStatus.Success;
        //    personImageMessage.PersonId = request.PersonId;
        //    personImageMessage.ImageType = ImageType.Jpg;
        //    byte[] image;
        //    try
        //    {
        //        image = File.ReadAllBytes(ImageFileName);
        //    }
        //    catch (Exception)
        //    {
        //        _logger.LogError($"Exception while reading image file '{ImageFileName}'.");
        //        throw new RpcException(Status.DefaultCancelled, "Exception while reading image file.");
        //    }
        //    int imageOffset = 0;

        //    byte[] imageChunk = new byte[ImageChunkSize];
        //    while (imageOffset < image.Length)
        //    {
        //        int length = Math.Min(ImageChunkSize, image.Length - imageOffset);
        //        Buffer.BlockCopy(image, imageOffset, imageChunk, 0, length);
        //        imageOffset += length;
        //        ByteString byteString = ByteString.CopyFrom(imageChunk);
        //        personImageMessage.ImageChunk = byteString;
        //        await responseStream.WriteAsync(personImageMessage).ConfigureAwait(false);
        //    }
        //}




        //public override async Task<TransferStatusMessage> UploadPersonImage(IAsyncStreamReader<PersonImageMessage> requestStream, ServerCallContext context)
        //{
        //    // TODO:
        //    string ImageFileName = null;
        //    //-------------------------------------

        //    TransferStatusMessage transferStatusMessage = new TransferStatusMessage();
        //    transferStatusMessage.Status = TransferStatus.Success;
        //    try
        //    {
        //        await Task.Run(
        //            async () =>
        //            {
        //                CancellationToken cancellationToken = context.CancellationToken;
        //                await using (Stream stream = File.OpenWrite(ImageFileName))
        //                {
        //                    await foreach (PersonImageMessage personImageMessage in
        //                        requestStream.ReadAllAsync(cancellationToken).ConfigureAwait(false))
        //                    {
        //                        stream.Write(personImageMessage.ImageChunk.ToByteArray());
        //                    }
        //                }
        //            }).ConfigureAwait(false);
        //    }
        //    // Is thrown on cancellation -> ignore...
        //    catch (OperationCanceledException)
        //    {
        //        transferStatusMessage.Status = TransferStatus.Cancelled;
        //    }
        //    catch (RpcException rpcEx)
        //    {
        //        if (rpcEx.StatusCode == StatusCode.Cancelled)
        //        {
        //            transferStatusMessage.Status = TransferStatus.Cancelled;
        //        }
        //        else
        //        {
        //            _logger.LogError($"Exception while processing image file '{ImageFileName}'. Exception: '{requestStream}'");
        //            transferStatusMessage.Status = TransferStatus.Failure;
        //        }
        //    }
        //    // Delete incomplete file
        //    if (transferStatusMessage.Status != TransferStatus.Success)
        //    {
        //        File.Delete(ImageFileName);
        //    }
        //    return transferStatusMessage;
        //}


    }
}
