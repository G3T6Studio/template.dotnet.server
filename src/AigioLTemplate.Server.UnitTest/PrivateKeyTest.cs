using AigioLTemplate.Server.UnitTest.Abstractions;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text.Json;

namespace AigioLTemplate.Server.UnitTest;

public sealed class PrivateKeyTest : BaseUnitTest
{
    /// <summary>
    /// 生成 RSA 密钥
    /// </summary>
    [Fact]
    public void Create()
    {
        using var rsa = RSA.Create();

        // 私钥用于解密
        var privateKey = rsa.ExportParameters(true);
#pragma warning disable CS0618 // 类型或成员已过时
        RSAUtils.Parameters privateKeyO = privateKey;
        var privateKeyJson = JsonSerializer.Serialize(privateKeyO, RSAUtils.Parameters.GetJsonTypeInfo());
#pragma warning restore CS0618 // 类型或成员已过时

        // 公钥用于加密
        var publicKey = rsa.ExportParameters(false);
        RsaSecurityKey publicKeyW = new(publicKey);
        var publicKeyJ = JsonWebKeyConverter.ConvertFromRSASecurityKey(publicKeyW);
        var publicKeyJson = JsonSerializer.Serialize(publicKeyJ);

        Console.WriteLine("Private Key:");
        Console.WriteLine(privateKeyJson);
        Console.WriteLine("Public Key:");
        Console.WriteLine(publicKeyJson);
    }
}
