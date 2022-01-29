using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace CommandsService.Controllers
{
    [Route("api/c/platforms/{platformId}/[controller]")]
    [ApiController]
    public class CommandsController : ControllerBase
    {
        private readonly ICommandRepository repository;
        private readonly IMapper mapper;

        public CommandsController(ICommandRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<CommandReadDto>> GetCommandsForPlatform(int platformId)
        {
            Console.WriteLine($"--> Hit {nameof(GetCommandsForPlatform)}: {platformId}.");

            if (!repository.PlatformExists(platformId))
            {
                return NotFound();
            }

            var commandItems = repository.GetCommandsForPlatform(platformId);
            var commandDtos = mapper.Map<IEnumerable<CommandReadDto>>(commandItems);
            return Ok(commandDtos);
        }

        [HttpGet("{commandId}", Name = "GetCommandForPlatform")]
        public ActionResult<CommandReadDto> GetCommandForPlatform(int platformId, int commandId)
        {
            Console.WriteLine($"--> Hit {nameof(GetCommandForPlatform)}: {platformId} / {commandId}.");

            if (!repository.PlatformExists(platformId))
            {
                return NotFound();
            }

            var command = repository.GetCommand(platformId, commandId);
            if (command == null)
            {
                return NotFound();
            }

            var commandDto = mapper.Map<CommandReadDto>(command);
            return Ok(commandDto);
        }

        [HttpPost]
        public ActionResult<CommandReadDto> CreateCommandForPlatform(int platformId, CommandCreateDto commandDto)
        {
            Console.WriteLine($"--> Hit {nameof(CreateCommandForPlatform)}: {platformId} / {JsonSerializer.Serialize(commandDto)}.");

            if (!repository.PlatformExists(platformId))
            {
                return NotFound();
            }

            var command = mapper.Map<Command>(commandDto);

            repository.CreateCommand(platformId, command);
            repository.SaveChanges();

            var commandReadDto = mapper.Map<CommandReadDto>(command);

            return CreatedAtRoute(nameof(GetCommandForPlatform), new { platformId = platformId, commandId = commandReadDto.Id }, commandReadDto);
        }
    }
}
