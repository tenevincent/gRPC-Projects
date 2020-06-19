using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrpcServiceProjects.Services
{
    public class CalculatorService : Calculs.CalculsBase
    {

        public override Task<ResultOut> Add(ParamsIn request, ServerCallContext context)
        {
            return Task.FromResult(new ResultOut
            {
                Result = request.Num + request.Denum
            });
        }

        public override Task<ResultOut> Divide(ParamsIn request, ServerCallContext context)
        {
            return Task.FromResult(new ResultOut
            {
                Result = request.Num / request.Denum
            });
        }


        public override Task<ResultOut> Substract(ParamsIn request, ServerCallContext context)
        {
            return Task.FromResult(new ResultOut
            {
                Result = request.Num - request.Denum
            });
        }


        public override Task<ResultOut> Multiply(ParamsIn request, ServerCallContext context)
        {
            return Task.FromResult(new ResultOut
            {
                Result = request.Num * request.Denum
            });
        }



    }
}
