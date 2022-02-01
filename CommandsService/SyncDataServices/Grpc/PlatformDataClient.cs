using AutoMapper;
using CommandsService.Models;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using PlatformService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace CommandsService.SyncDataServices.Grpc
{
    public class PlatformDataClient : IPlatformDataClient
    {
        private readonly IConfiguration configuration;
        private readonly IMapper mapper;

        public PlatformDataClient(IConfiguration configuration, IMapper mapper)
        {
            this.configuration = configuration;
            this.mapper = mapper;
        }

        public IEnumerable<Platform> ReturnAllPlatforms()
        {
            Console.WriteLine($"--> Calling gRPC Service '{configuration["GrpcPlatform"]}'");

            // Return "true" to allow certificates that are untrusted/invalid
            var httpHandler = new HttpClientHandler()
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };

            try
            {
                using (var channel = GrpcChannel.ForAddress(configuration["GrpcPlatform"], new GrpcChannelOptions { HttpHandler = httpHandler }))
                {
                    var client = new GrpcPlatform.GrpcPlatformClient(channel);
                    var request = new GetAllRequest();
                    var response = client.GetAllPlatforms(request);
                    var result = mapper.Map<IEnumerable<Platform>>(response.Platform);
                    return result;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not call gRPC Service, reason: '{ex.Message}'.");
                return Enumerable.Empty<Platform>();
            }
        }
    }
}
