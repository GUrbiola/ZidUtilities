using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ZidUtilities.CommonCode
{
    /// <summary>
    /// Provides utilities for symmetric encryption/decryption of strings and files,
    /// plus cryptographic hash utilities. Supports multiple algorithms including modern options.
    /// </summary>
    /// <remarks>
    /// <para><b>SECURITY NOTICE:</b></para>
    /// <list type="bullet">
    /// <item><description><b>DEPRECATED:</b> DES is cryptographically broken. Do not use for new applications.</description></item>
    /// <item><description><b>LEGACY:</b> Triple_Des is outdated. Use AES or newer algorithms for new projects.</description></item>
    /// <item><description><b>RECOMMENDED:</b> AES for .NET Framework 4.x; AES_GCM or ChaCha20_Poly1305 for .NET Core 3.0+/.NET 5+.</description></item>
    /// </list>
    /// <para><b>FRAMEWORK COMPATIBILITY:</b></para>
    /// <list type="bullet">
    /// <item><description>.NET Framework 4.x: DES, Triple_Des, Rijndael, AES</description></item>
    /// <item><description>.NET Core 3.0+ / .NET 5+: All algorithms including AES_GCM and ChaCha20_Poly1305</description></item>
    /// </list>
    /// </remarks>
    public class Crypter
    {
        /// <summary>
        /// Supported symmetric encryption algorithms.
        /// </summary>
        /// <remarks>
        /// - <b>DES:</b> INSECURE - Deprecated, do not use for new code
        /// - <b>Triple_Des:</b> Legacy algorithm, consider upgrading to AES
        /// - <b>Rijndael:</b> AES with flexible block sizes (legacy name)
        /// - <b>AES:</b> Industry standard symmetric encryption (recommended)
        /// - <b>AES_GCM:</b> AES with Galois/Counter Mode - provides authenticated encryption (requires .NET Core 3.0+/.NET 5+)
        /// - <b>ChaCha20_Poly1305:</b> Modern stream cipher with authentication (requires .NET Core 3.0+/.NET 5+)
        /// </remarks>
        public enum Algorithm
        {
            DES,
            Triple_Des,
            Rijndael,
            AES
#if NETCOREAPP3_0_OR_GREATER || NET5_0_OR_GREATER
            ,
            AES_GCM,
            ChaCha20_Poly1305
#endif
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Crypter"/> class with default configuration values.
        /// </summary>
        /// <remarks>
        /// Modern defaults use SHA256 with 100,000 iterations for strong key derivation.
        /// For backward compatibility with old encrypted data, you may need to adjust these values.
        /// </remarks>
        public Crypter()
        {
            _saltValue = "CidSaltValue"; // can be any string
            _passwordIterations = 100000; // Modern secure default (NIST recommendation)
            _initVector = "Cid Crypter 1234"; //must be 16 bytes for AES, 8 bytes for DES/3DES
            _hashAlgorithm = "SHA256"; // Modern default, can be SHA1, MD5, SHA256, SHA384, or SHA512
            _keySize = 256; // 256-bit keys provide strong security for AES
            _useModernKdf = true; // Use Rfc2898DeriveBytes instead of legacy PasswordDeriveBytes
        }

        private bool _useModernKdf = true;
        /// <summary>
        /// When true, uses Rfc2898DeriveBytes (PBKDF2) for key derivation.
        /// When false, uses legacy PasswordDeriveBytes for backward compatibility.
        /// </summary>
        /// <remarks>
        /// - Set to <c>true</c> (default) for new applications.
        /// - Set to <c>false</c> only when you need to decrypt data encrypted with the old method.
        /// </remarks>
        public bool UseModernKdf
        {
            get { return _useModernKdf; }
            set { _useModernKdf = value; }
        }

        private string _saltValue = "CidSaltValue"; // can be any string
        /// <summary>
        /// Salt value used when deriving cryptographic keys from a passphrase.
        /// </summary>
        /// <remarks>
        /// - This is an arbitrary string that is combined with the passphrase to produce a unique derived key.
        /// - It should be different per application (or per user/data) to prevent reuse of precomputed attacks
        ///   (rainbow-tables) against the passphrase alone.
        /// - The salt does not need to be secret, but it must be stable (persisted) for decryption to succeed.
        /// - Recommended: use a cryptographically-random byte sequence encoded as Base64 or hex for production,
        ///   and a minimum length of 8 bytes (but longer is better).
        /// - Default: "CidSaltValue"
        /// </remarks>
        public string SaltValue
        {
            get { return _saltValue; }
            set { _saltValue = value; }
        }

        private int _passwordIterations = 100000; // Modern secure default
        /// <summary>
        /// Number of iterations used by the password-based key derivation function.
        /// </summary>
        /// <remarks>
        /// - This increases the computational cost of deriving keys from a passphrase, slowing brute-force attacks.
        /// - Higher values are more secure but increase CPU cost at key derivation time.
        /// - Choose the highest value that is acceptable for your application's performance budget.
        /// - Modern recommendations (NIST 2023): minimum 100,000 iterations for PBKDF2-HMAC-SHA256.
        /// - When <see cref="UseModernKdf"/> is true, uses <see cref="Rfc2898DeriveBytes"/> (PBKDF2).
        /// - When <see cref="UseModernKdf"/> is false, uses legacy <see cref="PasswordDeriveBytes"/> for backward compatibility.
        /// - Default: 100,000 (for new applications; use 2 for legacy compatibility).
        /// </remarks>
        public int PasswordIterations
        {
            get { return _passwordIterations; }
            set { _passwordIterations = value; }
        }

        private string _initVector = "Cid Crypter 1234"; //must be 16 bytes
        /// <summary>
        /// Initialization Vector (IV) used by block cipher modes (e.g. CBC).
        /// </summary>
        /// <remarks>
        /// - The IV must be the correct byte length for the selected algorithm:
        ///   - DES / TripleDES: 8 bytes
        ///   - Rijndael / AES: 16 bytes (128 bits)
        /// - This value should be unpredictable and ideally unique per encryption operation.
        ///   Reusing a fixed IV across many encryptions weakens security for some modes.
        /// - The IV does not need to be secret, but it must be the same for encryption and decryption of a given ciphertext.
        /// - If you change algorithms or key sizes, ensure this string's byte-length (ASCII) matches the required IV length.
        /// - Default: "Cid Crypter 1234" (16 ASCII characters => 16 bytes; suitable for Rijndael/AES but not for DES/TripleDES).
        /// </remarks>
        public string InitVector
        {
            get { return _initVector; }
            set { _initVector = value; }
        }


        private string _hashAlgorithm = "SHA256"; // Modern default
        /// <summary>
        /// Name of the hash algorithm used for password-based key derivation.
        /// </summary>
        /// <remarks>
        /// <para><b>Supported values:</b> "MD5", "SHA1", "SHA256", "SHA384", "SHA512"</para>
        /// <para><b>Security recommendations:</b></para>
        /// <list type="bullet">
        /// <item><description><b>NOT RECOMMENDED:</b> "MD5" - Cryptographically broken, use only for legacy compatibility</description></item>
        /// <item><description><b>LEGACY:</b> "SHA1" - Deprecated for cryptographic use, consider upgrading</description></item>
        /// <item><description><b>RECOMMENDED:</b> "SHA256" - Industry standard, good balance of security and performance</description></item>
        /// <item><description><b>HIGH SECURITY:</b> "SHA384" or "SHA512" - For applications requiring maximum security</description></item>
        /// </list>
        /// <para>When <see cref="UseModernKdf"/> is true, this is used with PBKDF2 (Rfc2898DeriveBytes).</para>
        /// <para>Default: "SHA256"</para>
        /// </remarks>
        public string HashAlgorithm
        {
            get { return _hashAlgorithm; }
            set
            {
                switch (value?.ToUpperInvariant())
                {
                    case "MD5":
                    case "SHA1":
                    case "SHA256":
                    case "SHA384":
                    case "SHA512":
                        _hashAlgorithm = value;
                        break;
                    default:
                        _hashAlgorithm = "SHA256";
                        break;
                }
            }
        }

        private int _keySize = 256; // Modern secure default
        /// <summary>
        /// Key size in bits used when generating symmetric keys.
        /// </summary>
        /// <remarks>
        /// <para><b>Supported values:</b> 128, 192, 256 (bits)</para>
        /// <para><b>Algorithm compatibility:</b></para>
        /// <list type="bullet">
        /// <item><description>AES/Rijndael: 128, 192, or 256 bits</description></item>
        /// <item><description>TripleDES: 192 bits (24 bytes)</description></item>
        /// <item><description>DES: 64 bits (8 bytes, effectively 56 bits)</description></item>
        /// <item><description>ChaCha20-Poly1305: 256 bits only</description></item>
        /// </list>
        /// <para>Larger key sizes provide stronger security. 256-bit keys are recommended for new applications.</para>
        /// <para>Default: 256</para>
        /// </remarks>
        public int KeySize
        {
            get { return _keySize; }
            set
            {
                switch (value)
                {
                    case 128:
                    case 192:
                    case 256:
                        _keySize = value;
                        break;
                    default:
                        _keySize = 256;
                        break;
                }
            }
        }


        /// <summary>
        /// Resets configuration values to their modern secure defaults.
        /// </summary>
        /// <remarks>
        /// Sets: SHA256 hash, 100,000 iterations, 256-bit keys, modern KDF enabled.
        /// For legacy compatibility, use <see cref="SetLegacyValues"/> instead.
        /// </remarks>
        public void SetDefaultValues()
        {
            _saltValue = "CidSaltValue";
            _passwordIterations = 100000;
            _initVector = "Cid Crypter 1234";
            _hashAlgorithm = "SHA256";
            _keySize = 256;
            _useModernKdf = true;
        }

        /// <summary>
        /// Resets configuration values to legacy defaults for backward compatibility.
        /// </summary>
        /// <remarks>
        /// Sets: SHA1 hash, 2 iterations, 128-bit keys, legacy KDF enabled.
        /// Use this only when you need to decrypt data encrypted with old settings.
        /// </remarks>
        public void SetLegacyValues()
        {
            _saltValue = "CidSaltValue";
            _passwordIterations = 2;
            _initVector = "Cid Crypter 1234";
            _hashAlgorithm = "SHA1";
            _keySize = 128;
            _useModernKdf = false;
        }

        /// <summary>
        /// Derives cryptographic key bytes from a passphrase using either modern PBKDF2 or legacy method.
        /// </summary>
        private byte[] DeriveKeyBytes(string passphrase, byte[] saltBytes, int keyLengthBytes)
        {
            if (UseModernKdf)
            {
                // Modern approach: Use Rfc2898DeriveBytes (PBKDF2) with selected hash algorithm
                HashAlgorithmName hashName;
                var algo = HashAlgorithm.ToUpperInvariant();
                if (algo == "SHA256")
                    hashName = HashAlgorithmName.SHA256;
                else if (algo == "SHA384")
                    hashName = HashAlgorithmName.SHA384;
                else if (algo == "SHA512")
                    hashName = HashAlgorithmName.SHA512;
                else if (algo == "SHA1")
                    hashName = HashAlgorithmName.SHA1;
                else
                    hashName = HashAlgorithmName.SHA256;

                using (var rfc2898 = new Rfc2898DeriveBytes(passphrase, saltBytes, PasswordIterations, hashName))
                {
                    return rfc2898.GetBytes(keyLengthBytes);
                }
            }
            else
            {
                // Legacy approach: Use PasswordDeriveBytes for backward compatibility
#pragma warning disable SYSLIB0041 // PasswordDeriveBytes is obsolete
                using (var password = new PasswordDeriveBytes(passphrase, saltBytes, HashAlgorithm, PasswordIterations))
                {
                    return password.GetBytes(keyLengthBytes);
                }
#pragma warning restore SYSLIB0041
            }
        }

        /// <summary>
        /// Encrypts the specified plaintext string using the current instance configuration.
        /// </summary>
        /// <param name="clearText">Plaintext string to encrypt. Expected to be UTF-8 encoded text.</param>
        /// <param name="encryptionKey">Passphrase used to derive the encryption key. If not provided, a default is used.</param>
        /// <param name="algorithm">Algorithm to use for encryption. Defaults to AES.</param>
        /// <returns>Base64-encoded ciphertext string representing the encrypted data.</returns>
        /// <remarks>
        /// Uses Rijndael/AES in CBC mode with PKCS7 padding. For authenticated encryption, use <see cref="Algorithm.AES_GCM"/>.
        /// </remarks>
        public string Encrypt(string clearText, string encryptionKey = "Default Encryption Key 12344321", Algorithm algorithm = Algorithm.AES)
        {
            byte[] initVectorBytes = Encoding.ASCII.GetBytes(InitVector);
            byte[] saltValueBytes = Encoding.ASCII.GetBytes(SaltValue);
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(clearText);

            // Derive key bytes using modern or legacy KDF
            byte[] keyBytes = DeriveKeyBytes(encryptionKey, saltValueBytes, KeySize / 8);

#if NETCOREAPP3_0_OR_GREATER || NET5_0_OR_GREATER
            // Handle different algorithms
            if (algorithm == Algorithm.AES_GCM)
            {
                // For AES-GCM, use authenticated encryption
                using (var aesGcm = new AesGcm(keyBytes, AesGcm.TagByteSizes.MaxSize))
                {
                    byte[] nonce = new byte[AesGcm.NonceByteSizes.MaxSize];
                    Array.Copy(initVectorBytes, nonce, Math.Min(initVectorBytes.Length, nonce.Length));

                    byte[] cipherText = new byte[plainTextBytes.Length];
                    byte[] tag = new byte[AesGcm.TagByteSizes.MaxSize];

                    aesGcm.Encrypt(nonce, plainTextBytes, cipherText, tag);

                    // Combine nonce + tag + ciphertext for storage
                    byte[] result = new byte[nonce.Length + tag.Length + cipherText.Length];
                    Buffer.BlockCopy(nonce, 0, result, 0, nonce.Length);
                    Buffer.BlockCopy(tag, 0, result, nonce.Length, tag.Length);
                    Buffer.BlockCopy(cipherText, 0, result, nonce.Length + tag.Length, cipherText.Length);

                    return Convert.ToBase64String(result);
                }
            }
            else if (algorithm == Algorithm.ChaCha20_Poly1305)
            {
                // For ChaCha20-Poly1305
                using (var chacha = new ChaCha20Poly1305(keyBytes))
                {
                    byte[] nonce = new byte[ChaCha20Poly1305.NonceByteSizes.MaxSize];
                    Array.Copy(initVectorBytes, nonce, Math.Min(initVectorBytes.Length, nonce.Length));

                    byte[] cipherText = new byte[plainTextBytes.Length];
                    byte[] tag = new byte[ChaCha20Poly1305.TagByteSizes.MaxSize];

                    chacha.Encrypt(nonce, plainTextBytes, cipherText, tag);

                    // Combine nonce + tag + ciphertext for storage
                    byte[] result = new byte[nonce.Length + tag.Length + cipherText.Length];
                    Buffer.BlockCopy(nonce, 0, result, 0, nonce.Length);
                    Buffer.BlockCopy(tag, 0, result, nonce.Length, tag.Length);
                    Buffer.BlockCopy(cipherText, 0, result, nonce.Length + tag.Length, cipherText.Length);

                    return Convert.ToBase64String(result);
                }
            }
            else
#endif
            {
                // For traditional algorithms (Rijndael/AES)
                using (var symmetricKey = algorithm == Algorithm.Rijndael ?
                    (SymmetricAlgorithm)new RijndaelManaged() :
                    (SymmetricAlgorithm)Aes.Create())
                {
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;

                    using (ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes))
                    using (MemoryStream memoryStream = new MemoryStream())
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                        cryptoStream.FlushFinalBlock();

                        byte[] cipherTextBytes = memoryStream.ToArray();
                        return Convert.ToBase64String(cipherTextBytes);
                    }
                }
            }
        }

        /// <summary>
        /// Decrypts the specified base64-encoded ciphertext string using the current instance configuration.
        /// </summary>
        /// <param name="encryptedText">Base64-encoded ciphertext to decrypt.</param>
        /// <param name="encryptionKey">Passphrase used to derive the decryption key. If not provided, a default is used.</param>
        /// <param name="algorithm">Algorithm to use for decryption. Must match the algorithm used for encryption. Defaults to AES.</param>
        /// <returns>Decrypted plaintext string. Assumes the original plaintext was UTF-8 encoded.</returns>
        public string Decrypt(string encryptedText, string encryptionKey = "Default Encryption Key 12344321", Algorithm algorithm = Algorithm.AES)
        {
            byte[] initVectorBytes = Encoding.ASCII.GetBytes(InitVector);
            byte[] saltValueBytes = Encoding.ASCII.GetBytes(SaltValue);
            byte[] cipherTextBytes = Convert.FromBase64String(encryptedText);

            // Derive key bytes using modern or legacy KDF
            byte[] keyBytes = DeriveKeyBytes(encryptionKey, saltValueBytes, KeySize / 8);

#if NETCOREAPP3_0_OR_GREATER || NET5_0_OR_GREATER
            // Handle different algorithms
            if (algorithm == Algorithm.AES_GCM)
            {
                // For AES-GCM, use authenticated decryption
                using (var aesGcm = new AesGcm(keyBytes, AesGcm.TagByteSizes.MaxSize))
                {
                    int nonceSize = AesGcm.NonceByteSizes.MaxSize;
                    int tagSize = AesGcm.TagByteSizes.MaxSize;

                    // Extract nonce, tag, and ciphertext
                    byte[] nonce = new byte[nonceSize];
                    byte[] tag = new byte[tagSize];
                    byte[] cipherText = new byte[cipherTextBytes.Length - nonceSize - tagSize];

                    Buffer.BlockCopy(cipherTextBytes, 0, nonce, 0, nonceSize);
                    Buffer.BlockCopy(cipherTextBytes, nonceSize, tag, 0, tagSize);
                    Buffer.BlockCopy(cipherTextBytes, nonceSize + tagSize, cipherText, 0, cipherText.Length);

                    byte[] plainTextBytes = new byte[cipherText.Length];
                    aesGcm.Decrypt(nonce, cipherText, tag, plainTextBytes);

                    return Encoding.UTF8.GetString(plainTextBytes);
                }
            }
            else if (algorithm == Algorithm.ChaCha20_Poly1305)
            {
                // For ChaCha20-Poly1305
                using (var chacha = new ChaCha20Poly1305(keyBytes))
                {
                    int nonceSize = ChaCha20Poly1305.NonceByteSizes.MaxSize;
                    int tagSize = ChaCha20Poly1305.TagByteSizes.MaxSize;

                    // Extract nonce, tag, and ciphertext
                    byte[] nonce = new byte[nonceSize];
                    byte[] tag = new byte[tagSize];
                    byte[] cipherText = new byte[cipherTextBytes.Length - nonceSize - tagSize];

                    Buffer.BlockCopy(cipherTextBytes, 0, nonce, 0, nonceSize);
                    Buffer.BlockCopy(cipherTextBytes, nonceSize, tag, 0, tagSize);
                    Buffer.BlockCopy(cipherTextBytes, nonceSize + tagSize, cipherText, 0, cipherText.Length);

                    byte[] plainTextBytes = new byte[cipherText.Length];
                    chacha.Decrypt(nonce, cipherText, tag, plainTextBytes);

                    return Encoding.UTF8.GetString(plainTextBytes);
                }
            }
            else
#endif
            {
                // For traditional algorithms (Rijndael/AES)
                using (var symmetricKey = algorithm == Algorithm.Rijndael ?
                    (SymmetricAlgorithm)new RijndaelManaged() :
                    (SymmetricAlgorithm)Aes.Create())
                {
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;

                    using (ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes))
                    using (MemoryStream memoryStream = new MemoryStream(cipherTextBytes))
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        byte[] plainTextBytes = new byte[cipherTextBytes.Length];
                        int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);

                        return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                    }
                }
            }
        }

        /// <summary>
        /// Encrypts a file from disk using the specified symmetric algorithm and key.
        /// </summary>
        /// <param name="inputFile">Path to the input file to encrypt. Must exist.</param>
        /// <param name="outputFile">Path to the output file where encrypted data will be written. If exists it will be truncated.</param>
        /// <param name="encryptionKey">Passphrase or key material used to build the cipher key.</param>
        /// <param name="algorithm">Algorithm to use for file encryption. Defaults to AES.</param>
        /// <returns>
        /// Returns 1 on success. Returns -1 if input file does not exist or output file path is invalid.
        /// </returns>
        /// <remarks>
        /// For AES_GCM and ChaCha20_Poly1305, the nonce and authentication tag are prepended to the output file.
        /// For legacy algorithms (DES, Triple_Des, Rijndael), uses direct key derivation without PBKDF2.
        /// </remarks>
        public int EncryptFile(string inputFile, string outputFile, string encryptionKey, Algorithm algorithm = Algorithm.AES)
        {
            if (!File.Exists(inputFile) || outputFile == null || outputFile.Length == 0)
                return -1;

            byte[] saltValueBytes = Encoding.ASCII.GetBytes(SaltValue);
            byte[] initVectorBytes = Encoding.ASCII.GetBytes(InitVector);
            byte[] keyBytes;

            using (FileStream inStream = new FileStream(inputFile, FileMode.Open, FileAccess.Read))
            using (FileStream outStream = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
            {
                byte[] buffer = new byte[81920]; // 80KB buffer for better performance

#if NETCOREAPP3_0_OR_GREATER || NET5_0_OR_GREATER
                // For authenticated encryption algorithms
                if (algorithm == Algorithm.AES_GCM || algorithm == Algorithm.ChaCha20_Poly1305)
                {
                    // Read entire file into memory (limitation of current AEAD APIs)
                    byte[] fileData = new byte[inStream.Length];
                    inStream.Read(fileData, 0, fileData.Length);

                    keyBytes = DeriveKeyBytes(encryptionKey, saltValueBytes, KeySize / 8);

                    if (algorithm == Algorithm.AES_GCM)
                    {
                        using (var aesGcm = new AesGcm(keyBytes, AesGcm.TagByteSizes.MaxSize))
                        {
                            byte[] nonce = new byte[AesGcm.NonceByteSizes.MaxSize];
                            Array.Copy(initVectorBytes, nonce, Math.Min(initVectorBytes.Length, nonce.Length));

                            byte[] cipherText = new byte[fileData.Length];
                            byte[] tag = new byte[AesGcm.TagByteSizes.MaxSize];

                            aesGcm.Encrypt(nonce, fileData, cipherText, tag);

                            // Write nonce, tag, then ciphertext
                            outStream.Write(nonce, 0, nonce.Length);
                            outStream.Write(tag, 0, tag.Length);
                            outStream.Write(cipherText, 0, cipherText.Length);
                        }
                    }
                    else // ChaCha20_Poly1305
                    {
                        using (var chacha = new ChaCha20Poly1305(keyBytes))
                        {
                            byte[] nonce = new byte[ChaCha20Poly1305.NonceByteSizes.MaxSize];
                            Array.Copy(initVectorBytes, nonce, Math.Min(initVectorBytes.Length, nonce.Length));

                            byte[] cipherText = new byte[fileData.Length];
                            byte[] tag = new byte[ChaCha20Poly1305.TagByteSizes.MaxSize];

                            chacha.Encrypt(nonce, fileData, cipherText, tag);

                            // Write nonce, tag, then ciphertext
                            outStream.Write(nonce, 0, nonce.Length);
                            outStream.Write(tag, 0, tag.Length);
                            outStream.Write(cipherText, 0, cipherText.Length);
                        }
                    }
                }
                else
#endif
                {
                    // For traditional block cipher algorithms
                    SymmetricAlgorithm symmetricAlg;
                    int keySize;

                    switch (algorithm)
                    {
                        case Algorithm.DES:
                            symmetricAlg = new DESCryptoServiceProvider();
                            keySize = 8; // DES uses 64-bit (8-byte) keys
                            keyBytes = ValidateByteArraySize(encryptionKey, keySize);
                            break;
                        case Algorithm.Triple_Des:
                            symmetricAlg = new TripleDESCryptoServiceProvider();
                            keySize = 24; // 3DES uses 192-bit (24-byte) keys
                            keyBytes = ValidateByteArraySize(encryptionKey, keySize);
                            break;
                        case Algorithm.Rijndael:
                            symmetricAlg = new RijndaelManaged();
                            keyBytes = DeriveKeyBytes(encryptionKey, saltValueBytes, KeySize / 8);
                            break;
                        case Algorithm.AES:
                        default:
                            symmetricAlg = Aes.Create();
                            keyBytes = DeriveKeyBytes(encryptionKey, saltValueBytes, KeySize / 8);
                            break;
                    }

                    using (symmetricAlg)
                    using (ICryptoTransform encryptor = symmetricAlg.CreateEncryptor(keyBytes, initVectorBytes))
                    using (CryptoStream cryptoStream = new CryptoStream(outStream, encryptor, CryptoStreamMode.Write))
                    {
                        int bytesRead;
                        while ((bytesRead = inStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            cryptoStream.Write(buffer, 0, bytesRead);
                        }
                        cryptoStream.FlushFinalBlock();
                    }
                }
            }

            return 1;
        }

        /// <summary>
        /// Decrypts a file from disk using the specified symmetric algorithm and key.
        /// </summary>
        /// <param name="inputFile">Path to the input file (encrypted) to decrypt. Must exist.</param>
        /// <param name="outputFile">Path to the output file where decrypted data will be written. If exists it will be truncated.</param>
        /// <param name="encryptionKey">Passphrase or key material used to build the cipher key.</param>
        /// <param name="algorithm">Algorithm to use for file decryption. Must match the algorithm used for encryption. Defaults to AES.</param>
        /// <returns>
        /// Returns 1 on success. Returns -1 if input file does not exist or output file path is invalid.
        /// </returns>
        /// <remarks>
        /// For AES_GCM and ChaCha20_Poly1305, expects the nonce and authentication tag prepended to the file.
        /// </remarks>
        public int DecryptFile(string inputFile, string outputFile, string encryptionKey, Algorithm algorithm = Algorithm.AES)
        {
            if (!File.Exists(inputFile) || outputFile == null || outputFile.Length == 0)
                return -1;

            byte[] saltValueBytes = Encoding.ASCII.GetBytes(SaltValue);
            byte[] initVectorBytes = Encoding.ASCII.GetBytes(InitVector);
            byte[] keyBytes;

            using (FileStream inStream = new FileStream(inputFile, FileMode.Open, FileAccess.Read))
            using (FileStream outStream = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
            {
#if NETCOREAPP3_0_OR_GREATER || NET5_0_OR_GREATER
                // For authenticated encryption algorithms
                if (algorithm == Algorithm.AES_GCM || algorithm == Algorithm.ChaCha20_Poly1305)
                {
                    // Read entire file into memory (limitation of current AEAD APIs)
                    byte[] encryptedData = new byte[inStream.Length];
                    inStream.Read(encryptedData, 0, encryptedData.Length);

                    keyBytes = DeriveKeyBytes(encryptionKey, saltValueBytes, KeySize / 8);

                    if (algorithm == Algorithm.AES_GCM)
                    {
                        using (var aesGcm = new AesGcm(keyBytes, AesGcm.TagByteSizes.MaxSize))
                        {
                            int nonceSize = AesGcm.NonceByteSizes.MaxSize;
                            int tagSize = AesGcm.TagByteSizes.MaxSize;

                            // Extract nonce, tag, and ciphertext
                            byte[] nonce = new byte[nonceSize];
                            byte[] tag = new byte[tagSize];
                            byte[] cipherText = new byte[encryptedData.Length - nonceSize - tagSize];

                            Buffer.BlockCopy(encryptedData, 0, nonce, 0, nonceSize);
                            Buffer.BlockCopy(encryptedData, nonceSize, tag, 0, tagSize);
                            Buffer.BlockCopy(encryptedData, nonceSize + tagSize, cipherText, 0, cipherText.Length);

                            byte[] plainText = new byte[cipherText.Length];
                            aesGcm.Decrypt(nonce, cipherText, tag, plainText);

                            outStream.Write(plainText, 0, plainText.Length);
                        }
                    }
                    else // ChaCha20_Poly1305
                    {
                        using (var chacha = new ChaCha20Poly1305(keyBytes))
                        {
                            int nonceSize = ChaCha20Poly1305.NonceByteSizes.MaxSize;
                            int tagSize = ChaCha20Poly1305.TagByteSizes.MaxSize;

                            // Extract nonce, tag, and ciphertext
                            byte[] nonce = new byte[nonceSize];
                            byte[] tag = new byte[tagSize];
                            byte[] cipherText = new byte[encryptedData.Length - nonceSize - tagSize];

                            Buffer.BlockCopy(encryptedData, 0, nonce, 0, nonceSize);
                            Buffer.BlockCopy(encryptedData, nonceSize, tag, 0, tagSize);
                            Buffer.BlockCopy(encryptedData, nonceSize + tagSize, cipherText, 0, cipherText.Length);

                            byte[] plainText = new byte[cipherText.Length];
                            chacha.Decrypt(nonce, cipherText, tag, plainText);

                            outStream.Write(plainText, 0, plainText.Length);
                        }
                    }
                }
                else
#endif
                {
                    // For traditional block cipher algorithms
                    SymmetricAlgorithm symmetricAlg;
                    int keySize;

                    switch (algorithm)
                    {
                        case Algorithm.DES:
                            symmetricAlg = new DESCryptoServiceProvider();
                            keySize = 8; // DES uses 64-bit (8-byte) keys
                            keyBytes = ValidateByteArraySize(encryptionKey, keySize);
                            break;
                        case Algorithm.Triple_Des:
                            symmetricAlg = new TripleDESCryptoServiceProvider();
                            keySize = 24; // 3DES uses 192-bit (24-byte) keys
                            keyBytes = ValidateByteArraySize(encryptionKey, keySize);
                            break;
                        case Algorithm.Rijndael:
                            symmetricAlg = new RijndaelManaged();
                            keyBytes = DeriveKeyBytes(encryptionKey, saltValueBytes, KeySize / 8);
                            break;
                        case Algorithm.AES:
                        default:
                            symmetricAlg = Aes.Create();
                            keyBytes = DeriveKeyBytes(encryptionKey, saltValueBytes, KeySize / 8);
                            break;
                    }

                    using (symmetricAlg)
                    using (ICryptoTransform decryptor = symmetricAlg.CreateDecryptor(keyBytes, initVectorBytes))
                    using (CryptoStream cryptoStream = new CryptoStream(inStream, decryptor, CryptoStreamMode.Read))
                    {
                        byte[] buffer = new byte[81920]; // 80KB buffer
                        int bytesRead;
                        while ((bytesRead = cryptoStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            outStream.Write(buffer, 0, bytesRead);
                        }
                    }
                }
            }

            return 1;
        }

        /// <summary>
        /// Validates and pads or truncates a provided string into a fixed-size byte array.
        /// </summary>
        /// <param name="cad">Input string to be converted to a byte array (ASCII encoding).</param>
        /// <param name="lenght">Desired length of the resulting byte array.</param>
        /// <param name="filler">Byte value used to pad the array when the input is shorter than the desired length. Default is (byte)83.</param>
        /// <returns>
        /// A byte array of length <paramref name="lenght"/> containing ASCII bytes from <paramref name="cad"/>,
        /// padded with <paramref name="filler"/> if necessary.
        /// </returns>
        private byte[] ValidateByteArraySize(string cad, int lenght, byte filler = (byte)83)
        {
            byte[] back = new byte[lenght];
            byte[] temp = Encoding.ASCII.GetBytes(cad);

            for (int i = 0; i < lenght; i++)
            {
                back[i] = temp.Length > i ? temp[i] : filler;
            }

            return back;
        }

        /// <summary>
        /// Computes the MD5 hash of a file (if <paramref name="fileOrString"/> is an existing file path) or of the provided string.
        /// </summary>
        /// <param name="fileOrString">Either a path to a file to hash or a plain string to hash.</param>
        /// <returns>
        /// Hexadecimal uppercase MD5 hash string. Returns an empty string if input is null or empty.
        /// </returns>
        public static string GetMd5Sum(string fileOrString)
        {
            byte[] hash = null;

            if (!String.IsNullOrEmpty(fileOrString))
            {
                //Get the hash as a byte array
                if (File.Exists(fileOrString))
                {
                    using (var md5 = MD5.Create())
                    {
                        using (var stream = File.OpenRead(fileOrString))
                        {
                            hash = md5.ComputeHash(stream);
                        }
                    }
                }
                else
                {
                    byte[] inputBytes = Encoding.ASCII.GetBytes(fileOrString);

                    using (var md5 = MD5.Create())
                    {
                        hash = md5.ComputeHash(inputBytes);
                    }
                }

                //convert the byte array to an string(upper cased string)
                if (hash != null && hash.Length > 0)
                {
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < hash.Length; i++)
                    {
                        sb.Append(hash[i].ToString("X2"));
                    }
                    return sb.ToString();
                }
            }
            return "";

        }

        /// <summary>
        /// Verifies that the MD5 of the provided file path or string matches the supplied MD5 hex string.
        /// </summary>
        /// <param name="fileOrString">Path to a file or a plain string whose MD5 should be computed.</param>
        /// <param name="md5">Expected MD5 hash (hex string). Comparison is case-insensitive.</param>
        /// <returns>True if computed MD5 equals <paramref name="md5"/> (case-insensitive); otherwise false.</returns>
        public static bool VerifyMd5Sum(string fileOrString, string md5)
        {
            string buff = GetMd5Sum(fileOrString);
            return buff.Equals(md5, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Computes the SHA256 hash of a file (if <paramref name="fileOrString"/> is an existing file path) or of the provided string.
        /// </summary>
        /// <param name="fileOrString">Either a path to a file to hash or a plain string to hash.</param>
        /// <returns>
        /// Hexadecimal uppercase SHA256 hash string. Returns an empty string if input is null or empty.
        /// </returns>
        /// <remarks>
        /// SHA256 is cryptographically secure and recommended for modern applications.
        /// </remarks>
        public static string GetSha256Sum(string fileOrString)
        {
            byte[] hash = null;

            if (!String.IsNullOrEmpty(fileOrString))
            {
                if (File.Exists(fileOrString))
                {
                    using (var sha256 = SHA256.Create())
                    using (var stream = File.OpenRead(fileOrString))
                    {
                        hash = sha256.ComputeHash(stream);
                    }
                }
                else
                {
                    byte[] inputBytes = Encoding.UTF8.GetBytes(fileOrString);
                    using (var sha256 = SHA256.Create())
                    {
                        hash = sha256.ComputeHash(inputBytes);
                    }
                }

                if (hash != null && hash.Length > 0)
                {
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < hash.Length; i++)
                    {
                        sb.Append(hash[i].ToString("X2"));
                    }
                    return sb.ToString();
                }
            }
            return "";
        }

        /// <summary>
        /// Verifies that the SHA256 hash of the provided file path or string matches the supplied SHA256 hex string.
        /// </summary>
        /// <param name="fileOrString">Path to a file or a plain string whose SHA256 should be computed.</param>
        /// <param name="sha256">Expected SHA256 hash (hex string). Comparison is case-insensitive.</param>
        /// <returns>True if computed SHA256 equals <paramref name="sha256"/> (case-insensitive); otherwise false.</returns>
        public static bool VerifySha256Sum(string fileOrString, string sha256)
        {
            string buff = GetSha256Sum(fileOrString);
            return buff.Equals(sha256, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Computes the SHA512 hash of a file (if <paramref name="fileOrString"/> is an existing file path) or of the provided string.
        /// </summary>
        /// <param name="fileOrString">Either a path to a file to hash or a plain string to hash.</param>
        /// <returns>
        /// Hexadecimal uppercase SHA512 hash string. Returns an empty string if input is null or empty.
        /// </returns>
        /// <remarks>
        /// SHA512 provides the highest security level among common hash algorithms and is recommended for high-security applications.
        /// </remarks>
        public static string GetSha512Sum(string fileOrString)
        {
            byte[] hash = null;

            if (!String.IsNullOrEmpty(fileOrString))
            {
                if (File.Exists(fileOrString))
                {
                    using (var sha512 = SHA512.Create())
                    using (var stream = File.OpenRead(fileOrString))
                    {
                        hash = sha512.ComputeHash(stream);
                    }
                }
                else
                {
                    byte[] inputBytes = Encoding.UTF8.GetBytes(fileOrString);
                    using (var sha512 = SHA512.Create())
                    {
                        hash = sha512.ComputeHash(inputBytes);
                    }
                }

                if (hash != null && hash.Length > 0)
                {
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < hash.Length; i++)
                    {
                        sb.Append(hash[i].ToString("X2"));
                    }
                    return sb.ToString();
                }
            }
            return "";
        }

        /// <summary>
        /// Verifies that the SHA512 hash of the provided file path or string matches the supplied SHA512 hex string.
        /// </summary>
        /// <param name="fileOrString">Path to a file or a plain string whose SHA512 should be computed.</param>
        /// <param name="sha512">Expected SHA512 hash (hex string). Comparison is case-insensitive.</param>
        /// <returns>True if computed SHA512 equals <paramref name="sha512"/> (case-insensitive); otherwise false.</returns>
        public static bool VerifySha512Sum(string fileOrString, string sha512)
        {
            string buff = GetSha512Sum(fileOrString);
            return buff.Equals(sha512, StringComparison.OrdinalIgnoreCase);
        }
    }

}
