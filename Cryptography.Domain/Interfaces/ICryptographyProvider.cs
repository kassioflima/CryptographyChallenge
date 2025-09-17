namespace Cryptography.Domain.Interfaces
{
    public interface ICryptographyProvider
    {
        string Encrypt(string plainText);
        string Decrypt(string cipherText);
    }
}
