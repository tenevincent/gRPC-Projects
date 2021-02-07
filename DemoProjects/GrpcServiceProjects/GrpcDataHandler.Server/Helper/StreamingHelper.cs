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


        //public async Task DownloadFileAsync(PersonMessage request, IServerStreamWriter<PersonImageMessage> responseStream, int ImageChunkSize)
        //{

        //    var imageFileName = request.FileName;
        //    // Example of exception
        //    if (File.Exists(imageFileName) == false)
        //    {
        //        _logger.LogError($"File '{imageFileName}' not found.");
        //        Metadata metadata = new Metadata() { { "Filename", imageFileName } };
        //        throw new RpcException(new Status(StatusCode.NotFound, "Image file not found."),
        //            metadata, "More details for the exception...");
        //    }


        //    PersonImageMessage personImageMessage = new PersonImageMessage();
        //    personImageMessage.TransferStatusMessage = new TransferStatusMessage();
        //    personImageMessage.TransferStatusMessage.Status = TransferStatus.Success;
        //    personImageMessage.PersonId = request.PersonId;
        //    personImageMessage.ImageType = ImageType.Jpg;
        //    personImageMessage.FileName = Path.GetFileName(imageFileName);
        //    byte[] image;
        //   // Stream stream = null;
        //    try
        //    {
        //        image = File.ReadAllBytes(imageFileName);
        //      //  stream = File.OpenRead(imageFileName);
        //    }
        //    catch (Exception)
        //    {
        //        _logger.LogError($"Exception while reading image file '{imageFileName}'.");
        //        throw new RpcException(Status.DefaultCancelled, "Exception while reading image file.");
        //    }
        //    int imageOffset = 0;
        //    int remaining = image.Length;

        //    await   ReadWholeArray(imageFileName, ImageChunkSize, personImageMessage,  responseStream);

        //    //byte[] imageChunk = new byte[ImageChunkSize];
        //    //while (imageOffset < image.Length)
        //    //{
        //    //    int length = Math.Min(ImageChunkSize, image.Length - imageOffset);


        //    //    Buffer.BlockCopy(image, imageOffset, imageChunk, 0, length);
        //    //    imageOffset += length;
        //    //    ByteString byteString = ByteString.CopyFrom(imageChunk);
        //    //    personImageMessage.ImageChunk = byteString;
        //    //    await responseStream.WriteAsync(personImageMessage).ConfigureAwait(false);

        //    //    remaining -= byteString.Length;
        //    //}
        //}



        public static async Task ReadWholeArray( string fileName, int ImageChunkSize, PersonImageMessage personImageMessage, IServerStreamWriter<PersonImageMessage> responseStream)
        {

            byte[] data = new byte[ImageChunkSize];
            int offset = 0;
            Stream stream = File.OpenRead(fileName); 
            int remaining = data.Length;
            while (remaining > 0)
            {
                int read = stream.Read(data, offset, remaining);
                if (read <= 0)
                    throw new EndOfStreamException
                        (String.Format("End of stream reached with {0} bytes left to read", remaining));
                remaining -= read;
                offset += read;

                ByteString byteString = ByteString.CopyFrom(data);
                personImageMessage.ImageChunk = byteString;

                await responseStream.WriteAsync(personImageMessage).ConfigureAwait(false);
            }
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
            byte[] image;
            Stream stream = null;
            try
            {
                image = File.ReadAllBytes(imageFileName);
                stream = File.OpenRead(imageFileName);
            }
            catch (Exception)
            {
                _logger.LogError($"Exception while reading image file '{imageFileName}'.");
                throw new RpcException(Status.DefaultCancelled, "Exception while reading image file.");
            }
            int imageOffset = 0;
            int remaining = image.Length;


            byte[] imageChunk = new byte[ImageChunkSize];
            while (remaining > 0)
            {
                int length = Math.Min(ImageChunkSize, image.Length - imageOffset);


                Buffer.BlockCopy(image, imageOffset, imageChunk, 0, length);
                imageOffset += length;
                ByteString byteString = ByteString.CopyFrom(imageChunk);
                personImageMessage.ImageChunk = byteString;
                await responseStream.WriteAsync(personImageMessage).ConfigureAwait(false);

                remaining  = remaining - byteString.Length;
            
            }
        }





        public async Task<TransferStatusMessage> UploadPersonImageAsync(IAsyncStreamReader<PersonImageMessage> requestStream, ServerCallContext context, string imageFileName)
        {
            // TODO:
           // string ImageFileName = null;
            //-------------------------------------


            TransferStatusMessage transferStatusMessage = new TransferStatusMessage();
            transferStatusMessage.Status = TransferStatus.Success;
            try 
            {
                await Task.Run(
                    async () =>
                    {
                        CancellationToken cancellationToken = context.CancellationToken;
                        await using (Stream stream = File.OpenWrite(imageFileName))
                        {
                            await foreach (PersonImageMessage fileImageMessage in
                                requestStream.ReadAllAsync(cancellationToken).ConfigureAwait(false))
                            {
                                fileImageMessage.FileName = imageFileName;

                                stream.Write(fileImageMessage.ImageChunk.ToByteArray());
                            }
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
                    _logger.LogError($"Exception while processing image file '{imageFileName}'. Exception: '{requestStream}'");
                    transferStatusMessage.Status = TransferStatus.Failure;
                }
            }
            // Delete incomplete file
            if (transferStatusMessage.Status != TransferStatus.Success)
            {
                File.Delete(imageFileName);
            }
            return transferStatusMessage;
        }





    }
}
