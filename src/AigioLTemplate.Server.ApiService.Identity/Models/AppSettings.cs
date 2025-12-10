using AigioL.Common.AspNetCore.AppCenter.Identity.Models.Abstractions;
using AigioL.Common.AspNetCore.AppCenter.Models;
using AigioL.Common.AspNetCore.AppCenter.Models.Abstractions;
using AigioL.Common.SmsSender.Models;
using AigioL.Common.SmsSender.Models.Abstractions;

namespace AigioLTemplate.Server.ApiService.Identity.Models;

public sealed partial class AppSettings : MSAppSettings
{
}

partial class AppSettings : IDisableSms
{
    /// <inheritdoc/>
    public bool DisableSms { get; set; }
}

partial class AppSettings : ISmsSettings
{
    /// <inheritdoc/>
    public bool? UseDebugSmsSender { get; set; }

    /// <inheritdoc/>
    public SmsOptions? SmsOptions { get; set; }
}

partial class AppSettings : IExternalLoginRedirect
{
    /// <inheritdoc/>
    public string? AccountCenterBindUrl { get; set; }

    /// <inheritdoc/>
    public string? AccountLoginUrl { get; set; }
}