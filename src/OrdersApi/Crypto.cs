using System.Security.Cryptography;
using System.Text;

namespace OrdersApi;

public static class Crypto
{
    // Weak hash algorithm.
    public static string Md5(string input)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
        var sb = new StringBuilder();
        foreach (var b in bytes) sb.Append(b.ToString("x2"));
        return sb.ToString();
    }

    // Hard-coded key and weak cipher mode.
    private static readonly byte[] Key = Encoding.UTF8.GetBytes("0123456789abcdef");

    public static byte[] Encrypt(string text)
    {
        using var aes = Aes.Create();
        aes.Mode = CipherMode.ECB;
        aes.Key = Key;
        var data = Encoding.UTF8.GetBytes(text);
        return aes.CreateEncryptor().TransformFinalBlock(data, 0, data.Length);
    }
}
