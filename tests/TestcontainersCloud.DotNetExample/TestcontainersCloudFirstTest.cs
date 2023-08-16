using System;
using System.Text;
using System.Threading.Tasks;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Testcontainers.PostgreSql;

namespace TestcontainersCloud.DotNetExample;

[TestClass]
public sealed class TestcontainersCloudFirstTest
{
    [TestMethod]
    public async Task TestcontainersCloudDockerEngine()
    {
        using var dockerClientConfiguration =
            TestcontainersSettings.OS.DockerEndpointAuthConfig.GetDockerClientConfiguration(ResourceReaper
                .DefaultSessionId);

        using var dockerClient = dockerClientConfiguration.CreateClient();

        var versionResponse = await dockerClient.System.GetVersionAsync()
            .ConfigureAwait(false);

        var isTestcontainersDesktop = versionResponse.Version.Contains("Testcontainers Desktop");
        var isTestcontainersCloud = versionResponse.Version.Contains("testcontainerscloud");
        if (!(isTestcontainersDesktop || isTestcontainersCloud))
        {
            Console.WriteLine(PrettyStrings.OhNo);
            Assert.Fail();
        }

        var runtimeName = "Testcontainers Cloud";
        if (!versionResponse.Version.Contains("testcontainerscloud"))
        {
            runtimeName = versionResponse.Os;
        }

        if (versionResponse.Version.Contains("Testcontainers Desktop"))
        {
            runtimeName = "via Testcontainers Desktop app";
        }

        Console.WriteLine(PrettyStrings.Logo.Replace("::::::", runtimeName));
    }

    [TestMethod]
    public async Task CreatePostgreSQLContainer()
    {
        const string initScript = "create table guides\n" +
                         "(\n" +
                         "    id         bigserial     not null,\n" +
                         "    title      varchar(1023)  not null,\n" +
                         "    url        varchar(1023) not null,\n" +
                         "    primary key (id)\n" +
                         ");\n" +
                         "\n" +
                         "insert into guides(title, url)\n" +
                         "values ('Getting started with Testcontainers', 'https://testcontainers.com/getting-started/'),\n" +
                         "       ('Getting started with Testcontainers for Java', 'https://testcontainers.com/guides/getting-started-with-testcontainers-for-java/'),\n" +
                         "       ('Getting started with Testcontainers for .NET', 'https://testcontainers.com/guides/getting-started-with-testcontainers-for-dotnet/'),\n" +
                         "       ('Getting started with Testcontainers for Node.js', 'https://testcontainers.com/guides/getting-started-with-testcontainers-for-nodejs/'),\n" +
                         "       ('Getting started with Testcontainers for Go', 'https://testcontainers.com/guides/getting-started-with-testcontainers-for-go/'),\n" +
                         "       ('Testcontainers container lifecycle management using JUnit 5', 'https://testcontainers.com/guides/testcontainers-container-lifecycle/')\n" +
                         ";";
        await using var postgreSqlContainer = new PostgreSqlBuilder()
            .WithImage("postgres:14-alpine")
            .Build();

        await postgreSqlContainer.StartAsync()
            .ConfigureAwait(false);

        await postgreSqlContainer.CopyAsync(Encoding.Default.GetBytes(initScript), "/docker-entrypoint-initdb.d/init.sql").ConfigureAwait(false);
    }
}