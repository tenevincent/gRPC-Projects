using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Grpc.Core;



namespace GrpcServiceProjects.Advanced.Services
{
    public class SqrtServiceImpl : SqrtService.SqrtServiceBase
    {
        
        public override async Task<SqrtReponse> Sqrt(SqrtRequest request, ServerCallContext context)
        {
            //await Task.Delay(1500);
            await Task.CompletedTask;

            int number = request.Number;

            if (number >= 0)
                return new SqrtReponse() { SquareRoot = Math.Sqrt(number) };
            else
                throw new RpcException(new Status(StatusCode.InvalidArgument, "number < 0"));
        }
    }
}
