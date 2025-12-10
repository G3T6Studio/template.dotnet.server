## 项目模板（服务端）
使用 [.NET Aspire（阿斯拜尔）](https://dotnet.microsoft.com/zh-cn/apps/cloud)构建的云原生应用。

| Type                         | Development                                                                                    | Production                                                                             |
| ---------------------------- | ---------------------------------------------------------------------------------------------- | -------------------------------------------------------------------------------------- |
| Dashboard                    | [aigioltemplate-d.lysoft.pw](https://aigioltemplate-d.lysoft.pw)                                           | [aigioltemplate-d.lysoft.pw](https://aigioltemplate-d.lysoft.pw)                                           |
| Web Frontend(Admin Center)   | [aigioltemplate-admin.lysoft.pw](https://aigioltemplate-admin.lysoft.pw)                                   | [aigioltemplate-admin.lysoft.pw](https://aigioltemplate-admin.lysoft.pw)                                   |
| Web Api(Admin Center)        | [aigioltemplate-admin-api.speedtest.lysoft.pw](https://aigioltemplate-admin-api.speedtest.lysoft.pw)       | [aigioltemplate-admin-api.speedtest.lysoft.pw](https://aigioltemplate-admin-api.speedtest.lysoft.pw)       |
| Web Api(Microservices)       | [aigioltemplate-api.speedtest.lysoft.pw](https://aigioltemplate-api.speedtest.lysoft.pw)                   | [aigioltemplate-api.speedtest.lysoft.pw](https://aigioltemplate-api.speedtest.lysoft.pw)                   |
| CDN                          | [aigioltemplate.speedtest.lysoft.pw](https://aigioltemplate.speedtest.lysoft.pw)                           | [aigioltemplate.speedtest.lysoft.pw](https://aigioltemplate.speedtest.lysoft.pw)                           |
| Official Website             | [aigioltemplate.mossimo.net:29005](https://aigioltemplate.mossimo.net:29005)                               | [aigioltemplate.lysoft.pw](https://aigioltemplate.lysoft.pw)                                               |

![eShop Reference Application architecture diagram](res/eshop_architecture.png)

### 机密值存储位置
[使用 ASP.NET Core 安全地存储开发中的应用机密](https://learn.microsoft.com/zh-cn/aspnet/core/security/app-secrets)  

RSA 公钥(SecurityKey Web API)
```
TODO
```

#### AppHost
- Windows ```%APPDATA%\Microsoft\UserSecrets\{TODO_GUID}\secrets.json```
- Linux/macOS ```~/.microsoft/usersecrets/{TODO_GUID}/secrets.json```
```
{
  "Parameters:cache-password": "**********",
  "AppHost:OtlpApiKey": "**********",
  "Parameters:postgres-password": "**********"
}
```

### PostgreSQL 数据库表数据存储位置
```[ProjPath]\res\postgresql\data```

### [EF Core 迁移](https://docs.microsoft.com/zh-cn/ef/core/managing-schemas/migrations/?tabs=vs)
截止 EF Core 9.0，迁移操作与 AOT 不兼容，执行操作时取消 csproj 末尾行中的
```
<!--<PublishAot>false</PublishAot>-->
```
避免错误
```
Unable to create a 'DbContext' of type 'AppDbContext'. The exception 'Model building is not supported when publishing with NativeAOT. Use a compiled model.' was thrown while ataigioltemplateting to create an instance. For the different patterns supported at design time, see https://go.microsoft.com/fwlink/?linkid=851728
```

#### 创建迁移
```
Add-Migration {本次变更唯一名称} -Context AppDbContext
```

#### 更新迁移
```
Update-Database -Context AppDbContext
```

#### 生成 SQL 脚本
```
Script-Migration {上一次变更唯一名称} -Context AppDbContext
```

### Docker
仅打包所有微服务镜像命令
```
AigioLTemplate.Server.BuildTools spub
```
仅推送镜像命令
```
AigioLTemplate.Server.BuildTools spub --push_only --push_name aigioltemplate --push_domain docker.mossimo.net:10001
```

#### Dockerfile(Native AOT)
```
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
COPY ["src/", "src/"]
COPY ["ref/common/src/", "ref/common/src/"]
COPY ["ref/serversdk/src/", "ref/serversdk/src/"]
RUN dotnet restore "./src/[DirName]/[ProjectName].csproj"

WORKDIR "/workdir/src/[ProjectName]"
RUN dotnet build "./[ProjectName].csproj" -c $BUILD_CONFIGURATION -o /app/build

# 此阶段用于发布要复制到最终阶段的服务项目
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./[ProjectName].csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=true;GenerateDocumentationFile=false

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
FROM ${FINAL_BASE_IMAGE:-mcr.microsoft.com/dotnet/runtime-deps:10.0} AS final
WORKDIR /app
EXPOSE 8080
COPY --from=publish /app/publish .
ENTRYPOINT ["./[ProjectName]"]
```