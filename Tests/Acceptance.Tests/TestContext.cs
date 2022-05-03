using System;
using Microsoft.Extensions.Configuration;

namespace Acceptance.Tests;

public class TestContext : IDisposable
{
    public static readonly IConfiguration _config = Configuration.Load();
    private readonly DockerComposeHelper.ComposedService _composedService;

    public TestContext()
    {
        _composedService = new DockerComposeHelper(_config).Start();
    }

    public void Dispose()
    {
        _composedService.Dispose();
    }
}