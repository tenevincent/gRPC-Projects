using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MeterReaderWeb.Data;
using MeterReaderWeb.Data.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;



namespace MeterReaderWeb.Services
{
    public class MeterReadingServiceImpl : MeterReadingService.MeterReadingServiceBase
    {
        private readonly ILogger<MeterReadingServiceImpl> _logger;
        private readonly IReadingRepository _repository;

        public MeterReadingServiceImpl(ILogger<MeterReadingServiceImpl> logger, IReadingRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public override Task<Empty> Test(Empty request, ServerCallContext context)
        {
            return base.Test(request, context);
        }

        public async override Task<StatusMessage> AddReading(ReadingPacket request, ServerCallContext context)
        {
            var result = new StatusMessage()
            {
                Success = ReadingStatus.Failure
            };
 
            if (request.Successful == ReadingStatus.Success)
            {
                try
                {
                    foreach (var reading in request.Readings)
                    {
                        // Save to the database
                        var item = new MeterReading()
                        {
                            Value = reading.ReadingValue,
                            ReadingDate = reading.ReadingTime.ToDateTime(),
                            CustomerId = reading.CustomerId
                        };
                        // Add Entity
                        _repository.AddEntity(item);
                    }

                    if (await _repository.SaveAllAsync())
                    {
                        result.Success = ReadingStatus.Success;
                    }
                }
                catch (Exception ex)
                {
                    result.Message = "Exeption thrown during process";
                    _logger.LogError($"Exception thrown during saving of readings: {ex}");
                }
            }

            return result;
        }


    }
}
