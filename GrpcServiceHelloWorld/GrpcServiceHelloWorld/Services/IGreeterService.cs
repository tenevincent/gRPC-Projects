using Grpc.Core;
using System.Threading.Tasks;

namespace GrpcServiceHelloWorld.Server
{
    public interface IGreeterService
    {
        Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context);
    }
}