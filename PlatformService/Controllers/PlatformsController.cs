using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;
using System;
using System.Collections.Generic;

namespace PlatformService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlatformsController : ControllerBase
    {
        private readonly IPlatformRepository repository;
        private readonly IMapper mapper;

        public PlatformsController(IPlatformRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
        {
            Console.WriteLine("--> Getting Platforms...");

            var platformItems = repository.GetAll();

            var platformDtos = mapper.Map<IEnumerable<PlatformReadDto>>(platformItems);
            return Ok(platformDtos);
        }

        [HttpGet("{id}", Name = "GetPlatformById")]
        public ActionResult<PlatformReadDto> GetPlatformById(int id)
        {
            var platformItem = repository.GetPlatformById(id);

            return platformItem != null ? Ok(mapper.Map<PlatformReadDto>(platformItem)) : NotFound();
        }

        [HttpPost]
        public ActionResult<PlatformReadDto> CreatePlatform(PlatformCreateDto platformCreateDto)
        {
            var platformModel = mapper.Map<Platform>(platformCreateDto);
            repository.CreatePlatform(platformModel);
            repository.SaveChanges();

            var platformReadDto = mapper.Map<PlatformReadDto>(platformModel);
            var result = CreatedAtRoute(nameof(GetPlatformById), new { Id = platformReadDto.Id }, platformReadDto);
            return result;
        }
    }
}
