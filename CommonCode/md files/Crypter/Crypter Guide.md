# Crypter Class - Quick Start Guide

## Simple Usage (Works on Your Current .NET Framework 4.8)

### Basic Text Encryption
```csharp
using ZidUtilities.CommonCode;

var crypter = new Crypter();

// Encrypt
string encrypted = crypter.Encrypt("My secret data", "MyPassword123");

// Decrypt
string decrypted = crypter.Decrypt(encrypted, "MyPassword123");
```

### File Encryption
```csharp
var crypter = new Crypter();

// Encrypt a file
crypter.EncryptFile("document.txt", "document.enc", "MyPassword123", Crypter.Algorithm.AES);

// Decrypt a file
crypter.DecryptFile("document.enc", "document_decrypted.txt", "MyPassword123", Crypter.Algorithm.AES);
```

### Hash Files or Strings
```csharp
// SHA256 (recommended)
string sha256 = Crypter.GetSha256Sum("myfile.pdf");
bool valid = Crypter.VerifySha256Sum("myfile.pdf", sha256);

// SHA512 (highest security)
string sha512 = Crypter.GetSha512Sum("Important data");

// MD5 (legacy, for compatibility)
string md5 = Crypter.GetMd5Sum("myfile.zip");
```

## What Changed?

### ✅ New Defaults (More Secure)
- **Algorithm**: AES-256 (was: Rijndael with smaller key)
- **Hash**: SHA256 (was: SHA1)
- **Iterations**: 100,000 (was: 2)
- **Key Size**: 256-bit (was: 128-bit)

### ⚠️ Decrypting Old Data?
If you have data encrypted with the old settings:

```csharp
var crypter = new Crypter();
crypter.SetLegacyValues(); // Use old settings

string decrypted = crypter.Decrypt(oldEncryptedData, "password", Crypter.Algorithm.Rijndael);
```

## Available Algorithms

### For .NET Framework 4.8 (Your Current Version):
- **AES** ✅ - RECOMMENDED - Modern, secure, fast
- **Rijndael** - Legacy name for AES
- **Triple_Des** - Outdated, use only for compatibility
- **DES** - INSECURE, never use for new data

### For .NET Core 3.0+ / .NET 5+ / .NET 6+ (If You Upgrade):
- **AES_GCM** ✅✅ - BEST - Authenticated encryption
- **ChaCha20_Poly1305** ✅✅ - BEST - Modern authenticated encryption
- All above algorithms

## Want AES-GCM or ChaCha20-Poly1305?

These algorithms provide **authenticated encryption** (detects tampering) but require upgrading:

1. Open `CommonCode.csproj`
2. Change:
   ```xml
   <TargetFrameworkVersion>v4.8</TargetFrameworkVersion>
   ```
   To:
   ```xml
   <TargetFramework>net6.0</TargetFramework>
   ```
3. Rebuild and use:
   ```csharp
   var crypter = new Crypter();
   string encrypted = crypter.Encrypt("data", "password", Crypter.Algorithm.AES_GCM);
   ```

## Common Scenarios

### Scenario 1: New Application
```csharp
// Just use defaults - they're secure!
var crypter = new Crypter();
string encrypted = crypter.Encrypt("data", "password");
```

### Scenario 2: High Security Requirements
```csharp
var crypter = new Crypter
{
    HashAlgorithm = "SHA512",
    PasswordIterations = 200000,  // Extra slow = extra secure
    KeySize = 256
};
string encrypted = crypter.Encrypt("sensitive data", "strong password");
```

### Scenario 3: Decrypt Old Data, Encrypt New Data
```csharp
// Decrypt old data
var oldCrypter = new Crypter();
oldCrypter.SetLegacyValues();
string plainText = oldCrypter.Decrypt(oldData, "password", Crypter.Algorithm.Rijndael);

// Re-encrypt with modern settings
var newCrypter = new Crypter();
string newData = newCrypter.Encrypt(plainText, "password"); // Uses AES-256
```

### Scenario 4: Custom Salt & IV (For Multiple Apps)
```csharp
var crypter = new Crypter
{
    SaltValue = "MyApp2024Salt",      // Unique per app
    InitVector = "MyAppIV12345678",   // Must be 16 bytes for AES
};
string encrypted = crypter.Encrypt("data", "password");
```

## Security Best Practices

1. ✅ **Use strong passwords** - The encryption is only as strong as your password
2. ✅ **Use default settings** - They're already secure (SHA256, 100k iterations, AES-256)
3. ✅ **Store salt values securely** - Don't hardcode them in production
4. ✅ **Use unique IVs** - Don't reuse the same IV for different encryption operations
5. ✅ **Upgrade to .NET 6+** - If possible, to get authenticated encryption (AES-GCM)
6. ❌ **Don't use DES** - It's cryptographically broken
7. ❌ **Don't use low iterations** - Keep the default 100,000 or higher

## Hash Algorithm Comparison

| Algorithm | Security | Speed | Use Case |
|-----------|----------|-------|----------|
| **SHA-256** | ⭐⭐⭐⭐⭐ | ⭐⭐⭐⭐ | **Recommended** for most uses |
| **SHA-512** | ⭐⭐⭐⭐⭐ | ⭐⭐⭐ | High security applications |
| SHA-1 | ⭐⭐ | ⭐⭐⭐⭐ | Legacy only, deprecated |
| MD5 | ⭐ | ⭐⭐⭐⭐⭐ | Checksums only, not security |

## Encryption Algorithm Comparison (Available Now)

| Algorithm | Security | Speed | Notes |
|-----------|----------|-------|-------|
| **AES** | ⭐⭐⭐⭐⭐ | ⭐⭐⭐⭐ | **Best choice for .NET Framework 4.8** |
| Rijndael | ⭐⭐⭐⭐ | ⭐⭐⭐⭐ | Legacy, use AES instead |
| Triple DES | ⭐⭐ | ⭐⭐ | Slow, outdated |
| DES | ☠️ INSECURE | ⭐⭐⭐ | Never use |

## Need Help?

- See `CRYPTER_MODERNIZATION_SUMMARY.md` for full details
- See `CrypterUsageExample.cs` for comprehensive code examples
- All methods have detailed XML documentation

## Summary

**For your current .NET Framework 4.8 project:**
- ✅ Use `Crypter.Algorithm.AES` (default, most secure available)
- ✅ Keep default settings (SHA256, 100k iterations, 256-bit key)
- ✅ Old data can still be decrypted with `SetLegacyValues()`
- ✅ New SHA256/SHA512 hash methods available

**Want even better security?**
- Upgrade to .NET 6 or .NET 8
- Get access to AES-GCM and ChaCha20-Poly1305 (authenticated encryption)
