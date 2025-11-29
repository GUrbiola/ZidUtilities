using System;
using ZidUtilities.CommonCode;

namespace ZidUtilities.Tester
{
    /// <summary>
    /// Examples demonstrating how to use the modernized Crypter class
    /// </summary>
    /// <remarks>
    /// IMPORTANT FRAMEWORK COMPATIBILITY:
    /// - .NET Framework 4.x: Can use DES, Triple_Des, Rijndael, AES
    /// - .NET Core 3.0+ / .NET 5+: Can use all algorithms including AES_GCM and ChaCha20_Poly1305
    ///
    /// Examples marked with "⚠️ .NET Core 3.0+ / .NET 5+ ONLY" will not compile on .NET Framework 4.x.
    /// To use these features, upgrade your project to .NET 6 or .NET 8 (recommended).
    /// </remarks>
    public class CrypterUsageExample
    {
        /// <summary>
        /// Runs all Crypter usage examples
        /// </summary>
        public static void RunExamples()
        {
            // ============================================
            // MODERN USAGE (RECOMMENDED)
            // ============================================

            // Example 1: Encrypt/Decrypt text with modern AES (Works on all frameworks)
            var modernCrypter = new Crypter();
            // Defaults: AES, SHA256, 100,000 iterations, 256-bit key

            string plainText = "Hello, secure world!";
            string password = "MyStrongPassword123!";

            // Encrypt with modern AES (default)
            string encrypted = modernCrypter.Encrypt(plainText, password);
            Console.WriteLine($"Encrypted (AES): {encrypted}");

            // Decrypt
            string decrypted = modernCrypter.Decrypt(encrypted, password);
            Console.WriteLine($"Decrypted: {decrypted}");

            // ============================================
            // Example 2: Using AES-GCM (Authenticated Encryption - HIGHLY RECOMMENDED)
            // ⚠️ .NET Core 3.0+ / .NET 5+ ONLY
            // ============================================
#if NETCOREAPP3_0_OR_GREATER || NET5_0_OR_GREATER
            var gcmCrypter = new Crypter();

            // Encrypt with AES-GCM (provides both confidentiality and authenticity)
            string encryptedGcm = gcmCrypter.Encrypt(plainText, password, Crypter.Algorithm.AES_GCM);
            Console.WriteLine($"Encrypted (AES-GCM): {encryptedGcm}");

            // Decrypt - will throw exception if data has been tampered with
            string decryptedGcm = gcmCrypter.Decrypt(encryptedGcm, password, Crypter.Algorithm.AES_GCM);
            Console.WriteLine($"Decrypted (AES-GCM): {decryptedGcm}");
#else
            Console.WriteLine("AES-GCM requires .NET Core 3.0+ or .NET 5+. Upgrade your project to use this feature.");
#endif

            // ============================================
            // Example 3: Using ChaCha20-Poly1305 (Modern Stream Cipher)
            // ⚠️ .NET Core 3.0+ / .NET 5+ ONLY
            // ============================================
#if NETCOREAPP3_0_OR_GREATER || NET5_0_OR_GREATER
            var chachaCrypter = new Crypter();

            string encryptedChacha = chachaCrypter.Encrypt(plainText, password, Crypter.Algorithm.ChaCha20_Poly1305);
            Console.WriteLine($"Encrypted (ChaCha20): {encryptedChacha}");

            string decryptedChacha = chachaCrypter.Decrypt(encryptedChacha, password, Crypter.Algorithm.ChaCha20_Poly1305);
            Console.WriteLine($"Decrypted (ChaCha20): {decryptedChacha}");
#else
            Console.WriteLine("ChaCha20-Poly1305 requires .NET Core 3.0+ or .NET 5+. Upgrade your project to use this feature.");
#endif

            // ============================================
            // Example 4: File Encryption with AES
            // ============================================

            var fileCrypter = new Crypter();
            int result = fileCrypter.EncryptFile(
                "input.txt",
                "encrypted.bin",
                password,
                Crypter.Algorithm.AES
            );

            if (result == 1)
            {
                Console.WriteLine("File encrypted successfully");

                // Decrypt the file
                fileCrypter.DecryptFile(
                    "encrypted.bin",
                    "decrypted.txt",
                    password,
                    Crypter.Algorithm.AES
                );
                Console.WriteLine("File decrypted successfully");
            }

            // ============================================
            // Example 5: Custom Configuration
            // ============================================

            var customCrypter = new Crypter
            {
                HashAlgorithm = "SHA512",       // Use SHA512 for key derivation
                PasswordIterations = 200000,    // Extra security (slower)
                KeySize = 256,                  // 256-bit keys
                SaltValue = "MyAppSalt2024",    // Custom salt (store this securely)
                InitVector = "MyIV1234567890!!" // 16-byte IV for AES
            };

            string customEncrypted = customCrypter.Encrypt(plainText, password);
            string customDecrypted = customCrypter.Decrypt(customEncrypted, password);

            // ============================================
            // Example 6: Hash Functions (SHA256, SHA512)
            // ============================================

            string data = "Important data to hash";

            // Compute SHA256 hash
            string sha256Hash = Crypter.GetSha256Sum(data);
            Console.WriteLine($"SHA256: {sha256Hash}");

            // Verify hash
            bool isValid = Crypter.VerifySha256Sum(data, sha256Hash);
            Console.WriteLine($"SHA256 Valid: {isValid}");

            // Compute SHA512 hash
            string sha512Hash = Crypter.GetSha512Sum(data);
            Console.WriteLine($"SHA512: {sha512Hash}");

            // Hash a file
            string fileHash = Crypter.GetSha256Sum("myfile.txt");
            Console.WriteLine($"File SHA256: {fileHash}");

            // ============================================
            // LEGACY USAGE (For Backward Compatibility)
            // ============================================

            // If you need to decrypt old data encrypted with legacy settings
            var legacyCrypter = new Crypter();
            legacyCrypter.SetLegacyValues(); // Sets SHA1, 2 iterations, 128-bit key

            // Or manually configure for old data
            var oldCrypter = new Crypter
            {
                UseModernKdf = false,           // Use old PasswordDeriveBytes
                HashAlgorithm = "SHA1",         // Legacy hash
                PasswordIterations = 2,         // Old iteration count
                KeySize = 128                   // Old key size
            };

            // Decrypt old data with Rijndael
            // string oldEncrypted = "..."; // Your old encrypted data
            // string oldDecrypted = oldCrypter.Decrypt(oldEncrypted, password, Crypter.Algorithm.Rijndael);

            // ============================================
            // SECURITY RECOMMENDATIONS
            // ============================================

            /*
             * FRAMEWORK COMPATIBILITY:
             * ⚠️ .NET Framework 4.x: Only DES, Triple_Des, Rijndael, AES available
             * ✅ .NET Core 3.0+ / .NET 5+: All algorithms including AES_GCM and ChaCha20_Poly1305
             *
             * RECOMMENDED ALGORITHMS (in order of preference):
             *
             * For .NET Core 3.0+ / .NET 5+ / .NET 6+:
             * 1. AES_GCM          - Best choice: Fast, secure, authenticated encryption
             * 2. ChaCha20_Poly1305 - Modern alternative, excellent for mobile/embedded
             * 3. AES              - Industry standard, widely supported
             *
             * For .NET Framework 4.x:
             * 1. AES              - Best available option, industry standard, secure
             * 2. Rijndael         - Legacy name for AES, use AES instead
             *
             * LEGACY (Use only for backward compatibility):
             * - Rijndael          - Same as AES but with flexible block size
             * - Triple_Des        - Outdated, slow, smaller key space
             *
             * NEVER USE:
             * - DES               - Cryptographically broken, insecure
             *
             * HASH ALGORITHM RECOMMENDATIONS:
             * - SHA256            - Good balance of security and performance (default)
             * - SHA512            - Maximum security for critical applications
             * - SHA1              - Legacy only, deprecated
             * - MD5               - Broken, use only for checksums, not security
             *
             * KEY DERIVATION:
             * - Always use UseModernKdf = true (default)
             * - Minimum 100,000 iterations (default)
             * - Consider 200,000+ for high-security applications
             * - Use unique salts per application/user when possible
             *
             * TO USE MODERN AUTHENTICATED ENCRYPTION:
             * - Upgrade your project to .NET 6 or .NET 8 (recommended)
             * - Change target framework in your .csproj file
             * - Enjoy AES_GCM and ChaCha20_Poly1305!
             */
        }
    }
}
