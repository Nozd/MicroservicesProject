using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace CommandsService.EventProcessing
{
    public class EventProcessor : IEventProcessor
    {
        private readonly IServiceScopeFactory scopeFactory;
        private readonly IMapper mapper;

        public EventProcessor(IServiceScopeFactory scopeFactory, IMapper mapper)
        {
            this.scopeFactory = scopeFactory;
            this.mapper = mapper;
        }

        public void ProcessEvent(string message)
        {
            var eventType = DetermineEvent(message);
            switch (eventType)
            {
                case EventType.PlatformPublished:
                    AddPlatform(message);
                    break;
                default:
                    break;
            }
        }

        private void AddPlatform(string platformPublishedMessage)
        {
            using(var scope = scopeFactory.CreateScope())
            {
                var repository = scope.ServiceProvider.GetRequiredService<ICommandRepository>();
                var platformPublishedDto = JsonSerializer.Deserialize<PlatformPublishedDto>(platformPublishedMessage);
                try
                {
                    var platform = mapper.Map<Platform>(platformPublishedDto);
                    if (repository.ExternalPlatformExists(platform.ExternalId))
                    {
                        Console.WriteLine($"--> Platfrom '{platformPublishedMessage}' already exists.");
                    }
                    else
                    {
                        repository.CreatePlatform(platform);
                        repository.SaveChanges();
                        Console.WriteLine($"--> Platfrom '{platformPublishedMessage}' added.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"--> Could not add Platform '{platformPublishedMessage}' to the DB, reason: {ex.Message}");
                    throw;
                }
            }
        }

        private EventType DetermineEvent(string notificationMessage)
        {
            Console.WriteLine($"--> Determining Event: '{notificationMessage}'...");

            var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage);
            switch (eventType.Event)
            {
                case "Platform_Published":
                    Console.WriteLine("--> Platform Published Event Detected");
                    return EventType.PlatformPublished;
                default:
                    Console.WriteLine($"--> Could not determine Event Type: '{eventType.Event}'");
                    return EventType.Undetermined;
            }
        }
    }
}
