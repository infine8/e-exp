using NETCore.Encrypt;

namespace EthExplorer.Infrastructure.Common.Extensions;

public static class EncryptExtensions
{
    public static string EncryptAes(this string str, string key) => EncryptProvider.AESEncrypt(str, key);
    public static string DecryptAes(this string str, string key) => EncryptProvider.AESDecrypt(str, key);
    public static string EncryptMd5(this string str) => EncryptProvider.Md5(str);
}