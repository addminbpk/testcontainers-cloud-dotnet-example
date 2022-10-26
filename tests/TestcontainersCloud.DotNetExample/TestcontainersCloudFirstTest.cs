using System;
using System.Threading.Tasks;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestcontainersCloud.DotNetExample;

[TestClass]
public sealed class TestcontainersCloudFirstTest
{
    [TestMethod]
    public async Task Get_TestcontainersCloudDockerEngine_ReturnsTestcontainersCloud()
    {
        using var dockerClientConfiguration = TestcontainersSettings.OS.DockerEndpointAuthConfig.GetDockerClientConfiguration(ResourceReaper.DefaultSessionId);

        using var dockerClient = dockerClientConfiguration.CreateClient();

        var versionResponse = await dockerClient.System.GetVersionAsync()
            .ConfigureAwait(false);

        StringAssert.Contains(versionResponse.Version, "testcontainerscloud");
        Console.WriteLine(PrettyStrings.Logo);
    }
}