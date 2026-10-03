using System.Security.Cryptography;
using TrainingCenter.Api.Services.Interfaces;

namespace TrainingCenter.Api.Services.Implementations;

public class PasswordHasherService : IPasswordHasherService
{
    private const int SaltSize = 16; // 128 bit
    private const int KeySize = 32; // 256 bit
    private const int Iterations = 100000;
    private static readonly HashAlgorithmName HashAlgorithm = HashAlgorithmName.SHA512;
    private const char SegmentDelimiter = ':';

    public string HashPassword(string password)
    {
        if (string.IsNullOrEmpty(password))
        {
            throw new ArgumentNullException(nameof(password), "Password cannot be null or empty.");
        }

        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            Iterations,
            HashAlgorithm,
            KeySize);

        return string.Join(
            SegmentDelimiter,
            Convert.ToHexString(hash),
            Convert.ToHexString(salt),
            Iterations,
            HashAlgorithm.Name);
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(passwordHash))
        {
            return false;
        }

        string[] segments = passwordHash.Split(SegmentDelimiter);
        if (segments.Length != 4)
        {
            return false;
        }

        try
        {
            byte[] hash = Convert.FromHexString(segments[0]);
            byte[] salt = Convert.FromHexString(segments[1]);
            int iterations = int.Parse(segments[2]);
            var algorithm = new HashAlgorithmName(segments[3]);

            byte[] inputHash = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                iterations,
                algorithm,
                hash.Length);

            return CryptographicOperations.FixedTimeEquals(hash, inputHash);
        }
        catch
        {
            return false;
        }
    }
}
