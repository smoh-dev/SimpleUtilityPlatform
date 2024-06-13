using System.Security.Cryptography;
using System.Text;

namespace Sup.Common.Utils;

public class SupHash
{
    public SupHash()
    {
        
    }

    public string Hash512(string input)
    {
        using var sha512 = SHA512.Create();
        byte[] bytes = sha512.ComputeHash(Encoding.UTF8.GetBytes(input));
        StringBuilder builder = new StringBuilder();
        foreach (byte b in bytes)
            builder.Append(b.ToString("x2"));
        return builder.ToString();
    }
    
    public bool VerityHash512(string original, string hashedInput)
    {
        var computedHash = Hash512(original);
        return StringComparer.OrdinalIgnoreCase.Compare(computedHash, hashedInput) == 0;
    }
}