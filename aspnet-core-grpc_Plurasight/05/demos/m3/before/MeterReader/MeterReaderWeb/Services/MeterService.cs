using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MeterReaderWeb.Data;
using MeterReaderWeb.Data.Entities;
using Microsoft.Extensions.Logging;

namespace MeterReaderWeb.Services
{
  public class MeterService : MeterReadingService.MeterReadingServiceBase
  {
    private readonly ILogger<MeterService> _logger;
    private readonly IReadingRepository _repository;

    public MeterService(ILogger<MeterService> logger, IReadingRepository repository)
    {
      _logger = logger;
      _repository = repository;
    }

    public override Task<Empty> Test(Empty request, ServerCallContext context)
    {
      return base.Test(request, context);
    }

    public async override Task<StatusMessage> AddReading(ReadingPacket request,
                                                   ServerCallContext context)
    {
      var result = new StatusMessage()
      {
        Success = ReadingStatus.Failure
      };

      if (request.Successful == ReadingStatus.Success)
      {
        try
        {

          foreach (var r in request.Readings)
          {
            // Save to the database
            var reading = new MeterReading()
            {
              Value = r.ReadingValue,
              ReadingDate = r.ReadingTime.ToDateTime(),
              CustomerId = r.CustomerId
            };

            _repository.AddEntity(reading);
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
