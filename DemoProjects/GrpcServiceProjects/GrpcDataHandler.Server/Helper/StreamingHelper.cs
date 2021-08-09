using Google.Protobuf;
using Grpc.Core;
using GrpcDataStreaming.Server.Services;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GrpcDataStreaming.Server.Helper
{
    public class StreamingHelper<T>
    {
        //private readonly int ImageChunkSize = 1000;
        private readonly ILogger<T> _logger;

        public StreamingHelper(ILogger<T> logger)
        {
            this._logger = logger;
        }

        public async Task DownloadFileAsync(PersonMessage request, IServerStreamWriter<PersonImageMessage> responseStream, int ImageChunkSize)
        {
            var imageFileName = request.FileName;
            // Example of exception
            if (File.Exists(imageFileName) == false)
            {
                _logger.LogError($"File '{imageFileName}' not found.");
                Metadata metadata = new Metadata() { { "Filename", imageFileName } };
                throw new RpcException(new Status(StatusCode.NotFound, "Image file not found."),
                    metadata, "More details for the exception...");
            }


            PersonImageMessage personImageMessage = new PersonImageMessage();
            personImageMessage.TransferStatusMessage = new TransferStatusMessage();
            personImageMessage.TransferStatusMessage.Status = TransferStatus.Success;
            personImageMessage.PersonId = request.PersonId;
            personImageMessage.ImageType = ImageType.Jpg;
            personImageMessage.FileName = Path.GetFileName(imageFileName);
 
            using (var fileStream = File.OpenRead(imageFileName))
            {
                byte[] buffer = new byte[ImageChunkSize];
                int bytesRead;
                while ((bytesRead = await fileStream.ReadAsync(buffer)) > 0)
                {
                    ByteString byteString = ByteString.CopyFrom(buffer, 0, bytesRead);
                    personImageMessage.ImageChunk = byteString;
                    await responseStream.WriteAsync(personImageMessage).ConfigureAwait(false);
                }
            }


        }





        public async Task<TransferStatusMessage> UploadPersonImageAsync(IAsyncStreamReader<PersonImageMessage> requestStream, ServerCallContext context)
        {
            // TODO:
            string fileName = null;
            //-------------------------------------

            bool success = true;
            
            TransferStatusMessage transferStatusMessage = new TransferStatusMessage();
            transferStatusMessage.Status = TransferStatus.Success;
            try
            {
                await Task.Run(
                    async () =>
                    {
                        CancellationToken cancellationToken = context.CancellationToken;
                        Stream fileStream = null;
                        try
                        {
                            await foreach (PersonImageMessage fileMessage in requestStream.ReadAllAsync(cancellationToken).ConfigureAwait(false))
                            {
                                if (null == fileStream)
                                {
                                    fileName = fileMessage.FileName;
                                    fileStream = File.OpenWrite(fileMessage.FileName);
                                }
                                fileStream.Write(fileMessage.ImageChunk.ToByteArray());
                            }
                        }
                        catch (Exception ex)
                        {
                            success = false;
                        }
                        finally
                        {
                            success = true;
                            fileStream?.Close();
                        }

                        if (!success)
                        {
                            fileStream?.Close();
                            File.Delete(fileName);
                        }

                    }).ConfigureAwait(false);
            }
            // Is thrown on cancellation -> ignore...
            catch (OperationCanceledException)
            {
                transferStatusMessage.Status = TransferStatus.Cancelled;
            }
            catch (RpcException rpcEx)
            {
                if (rpcEx.StatusCode == StatusCode.Cancelled)
                {
                    transferStatusMessage.Status = TransferStatus.Cancelled;
                }
                else
                {
                    _logger.LogError($"Exception while processing image file '{fileName}'. Exception: '{requestStream}'");
                    transferStatusMessage.Status = TransferStatus.Failure;
                }
            }
            // Delete incomplete file
            if (transferStatusMessage.Status != TransferStatus.Success)
            {
                File.Delete(fileName);
            }
            return transferStatusMessage;
        }





    }
}
