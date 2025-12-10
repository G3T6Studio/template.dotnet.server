using AigioLTemplate.Server.UnitTest.Abstractions;
using System.Buffers;
using System.Text;

namespace AigioLTemplate.Server.UnitTest;

public sealed class DockerfileTest : BaseUnitTest
{
#pragma warning disable CS0162 // 检测到无法访问的代码
    [Fact]
    public void WriteDockerfile()
    {
        return;

        var projects = GetServerPublishProjects().ToArray();
        foreach (var it in projects)
        {
            StringBuilder publishArgs = new();
            var publishAot = DefaultPublishAot;

            var isJobScheduler = it.ProjectName.Contains("JobScheduler");
            if (isJobScheduler)
            {
                publishAot = false;
            }
            else
            {
                var csprojFilePath = Path.Combine(ProjectUtils.ROOT_ProjPath, "src", it.DirName, $"{it.ProjectName}.csproj");
                var lines = File.ReadAllLines(csprojFilePath);
                for (int i = 0; i < lines.Length; i++)
                {
                    var line = lines[i].AsSpan().Trim();
                    if (line.StartsWith("<PublishAot>", StringComparison.OrdinalIgnoreCase) &&
                       line.EndsWith("</PublishAot>", StringComparison.OrdinalIgnoreCase))
                    {
                        var value = line.Slice("<PublishAot>".Length, line.Length - "<PublishAot></PublishAot>".Length).Trim();
                        if (bool.TryParse(value, out var valueB))
                        {
                            if (publishAot != DefaultPublishAot)
                            {
                                publishAot = valueB;
                                break;
                            }
                        }
                    }
                }
            }
            {
                // https://learn.microsoft.com/en-us/ef/core/performance/nativeaot-and-precompiled-queries
                publishAot = false;
                // EFCore + AOT 不支持动态查询、全局过滤器
                // error : The entity type 'BMButton' has a query filter configured. Compiled model can't be generated, because query filters are not supported. 
            }
            publishArgs.Append($" -p:PublishAot={publishAot.ToString().ToLowerInvariant()}");
            var dockerfilePath = Path.Combine(ProjectUtils.ROOT_ProjPath, "src", it.DirName, "Dockerfile");
            var dockerfile_content = GetDockerfile(it, publishArgs.ToString());
            File.WriteAllText(dockerfilePath, dockerfile_content);
        }
    }

    static string GetDockerfile(ServerPublishProject m, string? publishArgs)
    {
        var r = new StringBuilder(dockerfile_content_template)
            .Replace("[ProjectName]", m.ProjectName)
            .Replace("[DirName]", m.DirName)
            .Replace("[PublishArgs]", publishArgs);
        return r.ToString();
    }

    const bool DefaultPublishAot = true;

    static IEnumerable<ServerPublishProject> GetServerPublishProjects()
    {
        var srcPath = Path.Combine(ProjectUtils.ROOT_ProjPath, "src");
        var directories = Directory.EnumerateDirectories(srcPath);
        foreach (var it in directories)
        {
            var filePathDockerfile = Path.Combine(it, "Dockerfile");
            if (!File.Exists(filePathDockerfile))
            {
                continue;
            }
            var filePathCsproj = Directory.EnumerateFiles(it, "*.csproj").FirstOrDefault();
            if (filePathCsproj == null)
            {
                continue;
            }
            var fileNameWithoutExtCsproj = Path.GetFileNameWithoutExtension(filePathCsproj);
            yield return new ServerPublishProject()
            {
                DirName = new DirectoryInfo(it).Name,
                ProjectName = fileNameWithoutExtCsproj,
            };
        }
    }

    const string dockerfile_content_template =
"""
# 请参阅 https://aka.ms/customizecontainer 以了解如何自定义调试容器，以及 Visual Studio 如何使用此 Dockerfile 生成映像以更快地进行调试。

# 通过这些 ARG，可以在从 VS 进行调试时交换用于生成最终映像的基础
ARG LAUNCHING_FROM_VS
# 此操作会设置最终的基础映像，但仅当已定义 LAUNCHING_FROM_VS 时才会如此
ARG FINAL_BASE_IMAGE=${LAUNCHING_FROM_VS:+aotdebug}

# 此阶段用于在快速模式(默认为调试配置)下从 VS 运行时
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080


# 此阶段用于生成服务项目
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
# 安装 clang/zlib1g 开发依赖项以发布到本机
RUN apt-get update \
    && apt-get install -y --no-install-recommends \
    clang zlib1g-dev
ARG BUILD_CONFIGURATION=Release
WORKDIR /workdir
COPY ["src/", "src"]
COPY ["res/i18n/", "res/i18n"]
COPY ["ref/common/src/", "ref/common/src"]
COPY ["ref/common/res/", "ref/common/res"]
COPY ["ref/serversdk/src/", "ref/serversdk/src"]
COPY ["ref/serversdk/res/", "ref/serversdk/res"]
RUN dotnet restore "./src/[DirName]/[ProjectName].csproj"

WORKDIR "/workdir/src/[ProjectName]"
RUN dotnet build "./[ProjectName].csproj" -c $BUILD_CONFIGURATION -o /app/build

# 此阶段用于发布要复制到最终阶段的服务项目
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./[ProjectName].csproj" -r linux-x64 -c $BUILD_CONFIGURATION -o /app/publish -p:UseAppHost=true -p:GenerateDocumentationFile=false [PublishArgs]

# remove the symbols so they aren't part of the published app
RUN rm -rf /app/publish/*.dbg
RUN rm -rf /app/publish/*.pdb

# 从 VS 启动以支持常规模式(不使用调试配置时为默认值)下的调试时，此阶段用作最终阶段的基础
#FROM base AS aotdebug
#USER roots
## 安装 GDB 以支持本机调试
#RUN apt-get update \
    #&& apt-get install -y --no-install-recommends \
    #gdb
#USER app

# 此阶段在生产中使用，或在常规模式下从 VS 运行时使用(在不使用调试配置时为默认值)
FROM ${FINAL_BASE_IMAGE:-mcr.microsoft.com/dotnet/aspnet:10.0.0} AS final
WORKDIR /app
EXPOSE 8080
COPY --from=publish /app/publish .
ENTRYPOINT ["./[ProjectName]"]
""";
}

public sealed record class ServerPublishProject
{
    public required string ProjectName { get; set; }

    public required string DirName { get; set; }
}