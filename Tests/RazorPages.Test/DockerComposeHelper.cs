using System;
using Ductus.FluentDocker.Builders;
using Ductus.FluentDocker.Services;
using Flurl;
using Microsoft.Extensions.Configuration;

namespace RazorPages.Test;

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
                               .FromFile(_configuration["DockerFilePath"])
                               .RemoveOrphans()
                               .WaitForHttp(
                                   _configuration["ServiceName"],
                                   healthUrl.ToString(),
                                   continuation: (response, _) => response.Body == "Healthy" ? 0 : -1
                               )
                               .Build()
                               .Start();

        return new ComposedService(compositeService);
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