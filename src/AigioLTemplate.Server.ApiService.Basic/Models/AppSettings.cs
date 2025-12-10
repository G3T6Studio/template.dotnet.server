using AigioL.Common.AspNetCore.AppCenter.Basic.Models.Abstractions;
using AigioL.Common.AspNetCore.AppCenter.Models;

namespace AigioLTemplate.Server.ApiService.Basic.Models;

public sealed partial class AppSettings : MSAppSettings, IAppSettings
{
    /// <inheritdoc/>
    public string? ImageUrl { get; set; }

    /// <inheritdoc/>
    public string? OfficialWebsite { get; set; }
}
