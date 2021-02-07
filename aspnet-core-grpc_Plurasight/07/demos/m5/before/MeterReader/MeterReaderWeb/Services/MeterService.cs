﻿using System;
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

    public override async Task<Empty> SendDiagnostics(IAsyncStreamReader<ReadingMessage> requestStream, 
                                                ServerCallContext context)
    {
      var theTask = Task.Run(async () =>
      {
        await foreach (var reading in requestStream.ReadAllAsync())
        {
          _logger.LogInformation($"Received Reading: {reading}");
        }
      });

      await theTask;

      return new Empty();
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

            if (r.ReadingValue < 1000)
            {
              _logger.LogDebug("Reading Value below acceptable level");
              var trailer = new Metadata()
              {
                { "BadValue", r.ReadingValue.ToString() },
                { "Field", "ReadingValue" },
                { "Message", "Readings are invalid" }
              };
              throw new RpcException(new Status(StatusCode.OutOfRange, "Value too low"), trailer);
            }
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
            _logger.LogInformation($"Stored {request.Readings.Count} New Readings...");
            result.Success = ReadingStatus.Success;
          }
        }
        catch (RpcException)
        {
          throw;
        }
        catch (Exception ex)
        {
          _logger.LogError($"Exception thrown during saving of readings: {ex}");
          throw new RpcException(Status.DefaultCancelled, "Exeption thrown during process");
        }
      }

      return result;
    }
  }
}
