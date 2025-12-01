using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Eventify.Service.DTOs.Tickets;
using Eventify.Service.Interfaces;

namespace Eventify.Service.Services
{
    public class TicketEncryptionService : ITicketEncryptionService
    {
        private readonly string _encryptionKey;

        public TicketEncryptionService(string encryptionKey)
        {
            if (string.IsNullOrEmpty(encryptionKey))
                throw new ArgumentException("Encryption key cannot be null or empty", nameof(encryptionKey));

            _encryptionKey = encryptionKey.PadRight(32).Substring(0, 32); // Ensure 32 chars for AES-256
        }

        public string GenerateTicketToken(int ticketId, int bookingId)
        {
            var data = new TicketTokenData
            {
                TicketId = ticketId,
                BookingId = bookingId,
                IssuedAt = DateTime.UtcNow
            };

            var json = JsonSerializer.Serialize(data);
            var jsonBytes = Encoding.UTF8.GetBytes(json);

            using var aes = Aes.Create();
            var keyBytes = Encoding.UTF8.GetBytes(_encryptionKey);
            aes.Key = keyBytes;
            aes.GenerateIV();

            using var encryptor = aes.CreateEncryptor();
            var encrypted = encryptor.TransformFinalBlock(jsonBytes, 0, jsonBytes.Length);

            var result = new byte[aes.IV.Length + encrypted.Length];
            Buffer.BlockCopy(aes.IV, 0, result, 0, aes.IV.Length);
            Buffer.BlockCopy(encrypted, 0, result, aes.IV.Length, encrypted.Length);

            return Convert.ToBase64String(result)
                .Replace("+", "-")
                .Replace("/", "_")
                .Replace("=", "");
        }

        public TicketTokenData DecryptToken(string encryptedToken)
        {
            try
            {
                // Restore base64 padding
                var base64 = encryptedToken.Replace("-", "+").Replace("_", "/");
                var padding = (4 - base64.Length % 4) % 4;
                base64 = base64.PadRight(base64.Length + padding, '=');

                var tokenBytes = Convert.FromBase64String(base64);
                var keyBytes = Encoding.UTF8.GetBytes(_encryptionKey);

                using var aes = Aes.Create();
                aes.Key = keyBytes;
                aes.IV = tokenBytes.Take(16).ToArray();

                using var decryptor = aes.CreateDecryptor();
                var decrypted = decryptor.TransformFinalBlock(tokenBytes, 16, tokenBytes.Length - 16);
                var json = Encoding.UTF8.GetString(decrypted);

                return JsonSerializer.Deserialize<TicketTokenData>(json);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to decrypt token", ex);
            }
        }
    }
}
