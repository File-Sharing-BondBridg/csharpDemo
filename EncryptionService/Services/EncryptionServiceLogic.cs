using System.Security.Cryptography;
using System.Text;

namespace EncryptionService.Services
{
    public class EncryptionServiceLogic
    {
        private readonly byte[] _key;
        private const int KeySize = 256;
        private const int NonceSize = 12; // GCM recommended nonce size
        private const int TagSize = 16;   // GCM authentication tag size

        public EncryptionServiceLogic()
        {
            // Same key as Go/Python/Java versions
            _key = Encoding.UTF8.GetBytes("0123456789abcdef0123456789abcdef");
        }

        public string Encrypt(string plaintext)
        {
            try
            {
                byte[] plaintextBytes = Encoding.UTF8.GetBytes(plaintext);

                // Generate random nonce
                byte[] nonce = new byte[NonceSize];
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(nonce);
                }

                // Create ciphertext buffer (nonce + encrypted data + tag)
                byte[] ciphertext = new byte[nonce.Length + plaintextBytes.Length + TagSize];
                Array.Copy(nonce, 0, ciphertext, 0, nonce.Length);

                // Encrypt using AES-GCM
                using (var aesGcm = new AesGcm(_key))
                {
                    aesGcm.Encrypt(
                        nonce,
                        plaintextBytes,
                        ciphertext.AsSpan(nonce.Length, plaintextBytes.Length),
                        ciphertext.AsSpan(nonce.Length + plaintextBytes.Length, TagSize)
                    );
                }

                return Convert.ToBase64String(ciphertext);
            }
            catch (Exception ex)
            {
                throw new Exception($"Encryption failed: {ex.Message}");
            }
        }

        public string Decrypt(string base64Ciphertext)
        {
            try
            {
                byte[] ciphertext = Convert.FromBase64String(base64Ciphertext);

                if (ciphertext.Length < NonceSize + TagSize)
                {
                    throw new ArgumentException("Invalid ciphertext");
                }

                // Extract nonce, encrypted data, and tag
                byte[] nonce = new byte[NonceSize];
                byte[] encryptedData = new byte[ciphertext.Length - NonceSize - TagSize];
                byte[] tag = new byte[TagSize];

                Array.Copy(ciphertext, 0, nonce, 0, NonceSize);
                Array.Copy(ciphertext, NonceSize, encryptedData, 0, encryptedData.Length);
                Array.Copy(ciphertext, NonceSize + encryptedData.Length, tag, 0, TagSize);

                // Decrypt using AES-GCM
                byte[] plaintextBytes = new byte[encryptedData.Length];
                using (var aesGcm = new AesGcm(_key))
                {
                    aesGcm.Decrypt(
                        nonce,
                        encryptedData,
                        tag,
                        plaintextBytes
                    );
                }

                return Encoding.UTF8.GetString(plaintextBytes);
            }
            catch (Exception ex)
            {
                throw new Exception($"Decryption failed: {ex.Message}");
            }
        }
    }
}
