using System;
using System.IO;
using System.Linq;
using System.Net;
using Ductus.FluentDocker.Builders;
using Ductus.FluentDocker.Services;
using Flurl;
using Microsoft.Extensions.Configuration;

namespace Acceptance.Tests;

internal class DockerComposeHelper
{
    private readonly IConfiguration _configuration;

    public DockerComposeHelper(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public ComposedService Start()
    {
        var healthUrl = new Url(_configuration["BaseAddress"])
            .AppendPathSegment("health");

        var compositeService = new Builder()
                               .UseContainer()
                               .UseCompose()
                               .FromFile(FindDockerFile())
                               .RemoveOrphans()
                               .ForceBuild()
                               .WaitForHttp(
                                   _configuration["ContainerName"],
                                   healthUrl.ToString(),
                                   continuation: (response, _) => response.Code == HttpStatusCode.OK ? 0 : 300
                               )
                               .Build()
                               .Start();

        return new ComposedService(compositeService);
    }

    private string FindDockerFile()
    {
        var directory = Directory.GetCurrentDirectory();
        while (!Directory.EnumerateFiles(directory!, _configuration["DockerFileName"]).Any())
        {
            directory = Path.GetDirectoryName(directory);
        }

        return Path.Join(directory, _configuration["DockerFileName"]);
    }

    internal class ComposedService : IDisposable
    {
        private readonly ICompositeService _compositeService;

        public ComposedService(ICompositeService compositeService)
        {
            _compositeService = compositeService;
        }

        public void Dispose()
        {
            _compositeService.Stop();
            _compositeService.Dispose();
        }
    }
}