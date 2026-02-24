using Application.Interfaces;
using Konscious.Security.Cryptography;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Security;

public sealed class Argon2PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16;            // 128 bits
    private const int HashSize = 32;            // 256 bits
    private const int DegreeOfParallelism = 2;
    private const int MemorySize = 65536;       // 64 MB
    private const int Iterations = 3;

    public string Hash(string password)
    {
        // Generar salt aleatorio
        var salt = RandomNumberGenerator.GetBytes(SaltSize);

        // Crear hash con Argon2id
        using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password));
        argon2.Salt = salt;
        argon2.DegreeOfParallelism = DegreeOfParallelism;
        argon2.MemorySize = MemorySize;
        argon2.Iterations = Iterations;

        var hash = argon2.GetBytes(HashSize);

        return $"{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
    }

    public bool Verify(string password, string storedHash)
    {
        var parts = storedHash.Split('.');
        if (parts.Length != 2) return false;

        var salt = Convert.FromBase64String(parts[0]);
        var hash = Convert.FromBase64String(parts[1]);

        using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password));
        argon2.Salt = salt;
        argon2.DegreeOfParallelism = DegreeOfParallelism;
        argon2.MemorySize = MemorySize;
        argon2.Iterations = Iterations;

        var computedHash = argon2.GetBytes(HashSize);

        // Comparación en tiempo constante para evitar timing attacks
        return CryptographicOperations.FixedTimeEquals(hash, computedHash);
    }
}
