using Microsoft.Extensions.Configuration;
using PlatformService.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PlatformService.SyncDataServices.Http
{
    public class HttpCommandDataClient : ICommandDataClient
    {
        private readonly HttpClient httpClient;

        public IConfiguration configuration { get; }

        public HttpCommandDataClient(HttpClient httpClient, IConfiguration configuration)
        {
            this.httpClient = httpClient;
            this.configuration = configuration;
        }

        public async Task SendPlatformToCommand(PlatformReadDto platform)
        {
            var httpContent = new StringContent(
                JsonSerializer.Serialize(platform), 
                Encoding.UTF8, 
                "application/json"
            );

            var response = await httpClient.PostAsync($"{configuration["CommandsService"]}", httpContent);

            Console.WriteLine(response.IsSuccessStatusCode ? "--> Sync POST to CommandsService was OK!" : "--> Sync POST to CommandsService was NOT OK!");
        }
    }
}
