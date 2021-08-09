using Google.Protobuf;
using Grpc.Core;
using Grpc.Net.Client;
using GrpcDataStreaming.Server.Services;
using System;
using System.IO;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;

namespace GrpcDataStreaming.Client
{
    /// <summary>
    /// https://blog.noser.com/grpc-tutorial-teil-2-streaming-mit-grpc/
    /// </summary>
    class Program
    {
        private static GrpcChannel channel = GrpcChannel.ForAddress("https://localhost:5001");
        private static StreamingService.StreamingServiceClient client = new StreamingService.StreamingServiceClient(channel);


        static async Task Main(string[] args)
        {



            try
            {
                Ping myPing = new Ping();
                PingReply reply =   myPing.Send("raspberrypi.local", 5000);
                if (reply != null)
                {
                    Console.WriteLine("Status :  " + reply.Status + " \n Time : " + reply.RoundtripTime.ToString() + " \n Address : " + reply.Address);
                    //Console.WriteLine(reply.ToString());

                }
            }
            catch
            {
                Console.WriteLine("ERROR: You have Some TIMEOUT issue");
            }
 



            int personId = 1;
            string fileName = "projec01_sdcard_conf_charge02.PNG";
            CancellationTokenSource cancelationToken = new CancellationTokenSource();
            CancellationToken token = cancelationToken.Token;

            await DownloadPersonImageAsync(personId, fileName, token);

            fileName = @"C:\Users\Tene\Downloads\gratisexam.com-Microsoft.pass4sureexam.AZ-203.v2020-03-02.by.georgia.83q.pdf";
            await UploadPersonImageAsync(personId, fileName, token);



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
                    await foreach (PersonImageMessage itemMessage in streamingCall.ResponseStream.ReadAllAsync(cancellationToken).ConfigureAwait(false))
                    {
                        var readBytes = itemMessage.ImageChunk.ToByteArray();
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

 

        private static async Task UploadPersonImageAsync(int personId, string fileName, CancellationToken cancellationToken)
        {
            // TODO:
            int imageChunkSize = 1000;

            var stream = client.UploadPersonImage();
            PersonImageMessage personImageMessage = new PersonImageMessage();
            personImageMessage.PersonId = personId;
            personImageMessage.ImageType = ImageType.Jpg;

             
            using (var fileStream = File.OpenRead(fileName))
            {
                byte[] buffer = new byte[imageChunkSize];
                int bytesRead;
                 
                while (((bytesRead = await fileStream.ReadAsync(buffer)) > 0) && !cancellationToken.IsCancellationRequested)
                {
                    ByteString byteString = ByteString.CopyFrom(buffer, 0, bytesRead);
                    personImageMessage.FileName = Path.GetFileName(fileName);
                    personImageMessage.ImageChunk = byteString;
                    await stream.RequestStream.WriteAsync(personImageMessage).ConfigureAwait(false);
                }
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
