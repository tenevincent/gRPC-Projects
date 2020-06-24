using Google.Protobuf;
using Grpc.Core;
using Grpc.Net.Client;
using GrpcDataStreaming.Server.Services;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace GrpcDataStreaming.Client
{
    class Program
    {
        private static GrpcChannel channel = GrpcChannel.ForAddress("https://localhost:5001");
        private static StreamingService.StreamingServiceClient client = new StreamingService.StreamingServiceClient(channel);


        static async Task Main(string[] args)
        {
            int personId = 1;
            string fileName = "LOG03.LOG";
            CancellationTokenSource cancelationToken = new CancellationTokenSource();
            CancellationToken token = cancelationToken.Token;

            await DownloadPersonImageAsync(personId, fileName, token);

            Console.WriteLine("Hello World!");
            Console.ReadKey();
        }



        private static async Task DownloadPersonImageAsync(int personId, string fileName, CancellationToken cancellationToken)
        {
            bool success = true;
            try
            {
                PersonMessage personMessage = new PersonMessage { PersonId = personId };
                using var streamingCall = client.DownloadPersonImage(personMessage);
                await using (Stream stream = File.OpenWrite(fileName))
                {
                    await foreach (PersonImageMessage personImageMsg in streamingCall.ResponseStream.ReadAllAsync(cancellationToken).ConfigureAwait(false))
                    {
                        var readBytes = personImageMsg.ImageChunk.ToByteArray();
                        if(readBytes.Length > 0)
                            stream.Write(readBytes);
                    }
                }
            }
            // Is thrown on cancellation -> ignore...
            catch (OperationCanceledException opEx)
            {
                success = false;
            }
            catch (Exception ex)
            {
                //_logger.LogError(e, "Exception thrown");
                success = false;
            }
            if (!success)
            {
                File.Delete(fileName);
            }
        }

        private static byte [] ReadAllBytes(Stream stream)
        {
            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                var bytesRead = memoryStream.ToArray();
            }
            return null;
        }

        private async Task UploadPersonImageAsync(int personId, string fileName, CancellationToken cancellationToken)
        {
            // TODO:
            int imageChunkSize = 0;

            var stream = client.UploadPersonImage();
            PersonImageMessage personImageMessage = new PersonImageMessage();
            personImageMessage.PersonId = personId;
            personImageMessage.ImageType = ImageType.Jpg;
            byte[] image = File.ReadAllBytes(fileName);
            int imageOffset = 0;
           
            byte[] imageChunk = new byte[imageChunkSize];
            while (imageOffset < image.Length && !cancellationToken.IsCancellationRequested)
            {
                int length = Math.Min(imageChunkSize, image.Length - imageOffset);
                Buffer.BlockCopy(image, imageOffset, imageChunk, 0, length);
                imageOffset += length;
                ByteString byteString = ByteString.CopyFrom(imageChunk);
                personImageMessage.ImageChunk = byteString;
                await stream.RequestStream.WriteAsync(personImageMessage).ConfigureAwait(false);
            }
            await stream.RequestStream.CompleteAsync().ConfigureAwait(false);
            if (!cancellationToken.IsCancellationRequested)
            {
                var uploadPersonImageResult = await stream.ResponseAsync.ConfigureAwait(false);
                // Process answer...
            }
        }

    }
}
