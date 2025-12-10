#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace AigioL.Common.BuildTools.Commands;

partial interface IServerPublishCommand
{
    private const string DefaultPushName = "AigioLTemplate.Server";
    private const string DefaultPushDomain = "docker.mossimo.net:10001";
    private static readonly string[] IgnoreProjects =
    [
        "AigioLTemplate.Server.ApiService.Cluster",
        "AigioLTemplate.Server.WebUI.AdminCenter",
    ];
}
