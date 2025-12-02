# CommonCode

Core utility library providing common functionality used across the ZidUtilities solution.

## Features

### Security & Cryptography
- **Crypter**: Encryption and decryption utilities supporting multiple algorithms
- **PasswordGenerator**: Secure password generation with customizable complexity rules
- **Check**: Input validation and security checks

### Data Management
- **Extensions**: Extension methods for common .NET types
- **GenericTokenList & TokenList**: Token-based list management
- **SimpleDictionaryPersister**: Dictionary persistence utilities
- **XmlSerialization**: XML serialization helpers

### Data Comparison
- **DifferenceEngine**: Advanced data comparison utilities for detecting changes between datasets
- **DataComparison**: Data comparison and difference detection tools

### Image Processing
- **ImageHelper**: Image manipulation and conversion utilities

### Server Utilities
- **ServerFiltering**: Server-side filtering and data processing

### Localization
- **LanguageConfig**: Language configuration and localization support

## Dependencies

This is a core library with minimal external dependencies, designed to be lightweight and reusable.

## Target Framework

.NET Framework 4.8

## Usage Examples

### Crypter - String Encryption and Decryption

#### Basic Encryption with Default Settings (AES-256)

```csharp
using ZidUtilities.CommonCode;

var crypter = new Crypter();

// Encrypt a string
string plainText = "Sensitive data that needs protection";
string encryptionKey = "MySecurePassword123";
string encrypted = crypter.Encrypt(plainText, encryptionKey);

Console.WriteLine($"Encrypted: {encrypted}");

// Decrypt the string
string decrypted = crypter.Decrypt(encrypted, encryptionKey);
Console.WriteLine($"Decrypted: {decrypted}");
```

#### Using Different Encryption Algorithms

```csharp
var crypter = new Crypter();

// Using AES (default and recommended)
string aesEncrypted = crypter.Encrypt("Secret data", "key", Crypter.Algorithm.AES);

// Using Rijndael
string rijndaelEncrypted = crypter.Encrypt("Secret data", "key", Crypter.Algorithm.Rijndael);

// For legacy compatibility (not recommended for new applications)
string tripleDesEncrypted = crypter.Encrypt("Secret data", "key", Crypter.Algorithm.Triple_Des);
```

#### Custom Configuration for Enhanced Security

```csharp
var crypter = new Crypter
{
    SaltValue = "MyCustomSalt",
    PasswordIterations = 100000, // NIST recommended
    HashAlgorithm = "SHA256",
    KeySize = 256,
    UseModernKdf = true
};

string encrypted = crypter.Encrypt("High security data", "StrongPassword!");
string decrypted = crypter.Decrypt(encrypted, "StrongPassword!");
```

#### Legacy Data Compatibility

```csharp
// If you need to decrypt data encrypted with old settings
var crypter = new Crypter();
crypter.SetLegacyValues(); // Uses SHA1, 2 iterations, 128-bit keys

string legacyDecrypted = crypter.Decrypt(oldEncryptedData, "OldPassword");
```

### Crypter - File Encryption

#### Encrypting and Decrypting Files

```csharp
var crypter = new Crypter();

// Encrypt a file
int result = crypter.EncryptFile(
    inputFile: @"C:\docs\confidential.pdf",
    outputFile: @"C:\docs\confidential.pdf.encrypted",
    encryptionKey: "FileProtectionKey2024"
);

if (result == 1)
{
    Console.WriteLine("File encrypted successfully!");

    // Decrypt the file
    result = crypter.DecryptFile(
        inputFile: @"C:\docs\confidential.pdf.encrypted",
        outputFile: @"C:\docs\confidential_decrypted.pdf",
        encryptionKey: "FileProtectionKey2024"
    );

    if (result == 1)
        Console.WriteLine("File decrypted successfully!");
}
```

### Crypter - Hash Functions

#### Computing and Verifying Hashes

```csharp
// MD5 Hash (use only for legacy compatibility)
string md5Hash = Crypter.GetMd5Sum("Hello World");
bool md5Valid = Crypter.VerifyMd5Sum("Hello World", md5Hash);

// SHA256 Hash (recommended for modern applications)
string sha256Hash = Crypter.GetSha256Sum("Hello World");
bool sha256Valid = Crypter.VerifySha256Sum("Hello World", sha256Hash);

// SHA512 Hash (maximum security)
string sha512Hash = Crypter.GetSha512Sum("Hello World");
bool sha512Valid = Crypter.VerifySha512Sum("Hello World", sha512Hash);

// File hashing
string fileHash = Crypter.GetSha256Sum(@"C:\docs\document.pdf");
Console.WriteLine($"File SHA256: {fileHash}");
```

### PasswordGenerator - Creating Secure Passwords

#### Basic Password Generation

```csharp
using ZidUtilities.CommonCode;

var generator = new PasswordGenerator(length: 12);

// Add rules for password composition
generator.Rules.Add(new PasswordComponent
{
    CompType = PasswordComponentType.UpperCase,
    Quantity = 2
});

generator.Rules.Add(new PasswordComponent
{
    CompType = PasswordComponentType.LowerCase,
    Quantity = 6
});

generator.Rules.Add(new PasswordComponent
{
    CompType = PasswordComponentType.Digit,
    Quantity = 2
});

generator.Rules.Add(new PasswordComponent
{
    CompType = PasswordComponentType.SpecialChar,
    Quantity = 2
});

string password = generator.GetPassword();
Console.WriteLine($"Generated Password: {password}");
// Example output: "aB3dEf!2gH@i"
```

#### Generating Multiple Passwords

```csharp
var generator = new PasswordGenerator(16);

generator.Rules.Add(new PasswordComponent { CompType = PasswordComponentType.UpperCase, Quantity = 3 });
generator.Rules.Add(new PasswordComponent { CompType = PasswordComponentType.LowerCase, Quantity = 8 });
generator.Rules.Add(new PasswordComponent { CompType = PasswordComponentType.Digit, Quantity = 3 });
generator.Rules.Add(new PasswordComponent { CompType = PasswordComponentType.SpecialChar, Quantity = 2 });

// Generate 10 unique passwords
for (int i = 0; i < 10; i++)
{
    Console.WriteLine($"Password {i + 1}: {generator.GetPassword()}");
}
```

#### Password with Specific Security Requirements

```csharp
// Corporate password policy: 14 chars, 2 uppercase, 8 lowercase, 2 digits, 2 special
var corpGenerator = new PasswordGenerator(14);

corpGenerator.Rules.Add(new PasswordComponent { CompType = PasswordComponentType.UpperCase, Quantity = 2 });
corpGenerator.Rules.Add(new PasswordComponent { CompType = PasswordComponentType.LowerCase, Quantity = 8 });
corpGenerator.Rules.Add(new PasswordComponent { CompType = PasswordComponentType.Digit, Quantity = 2 });
corpGenerator.Rules.Add(new PasswordComponent { CompType = PasswordComponentType.SpecialChar, Quantity = 2 });

string corporatePassword = corpGenerator.GetPassword();
```

#### Simple Random Password

```csharp
// If no rules are specified, the generator fills with random letters
var simpleGenerator = new PasswordGenerator(10);
string simplePassword = simpleGenerator.GetPassword();
// Output will be random letters (upper and lowercase)
```

### Check - Input Validation

The `Check` class provides various validation utilities for securing user input and validating data formats.

```csharp
using ZidUtilities.CommonCode;

// Use Check class methods for validation
// (Specific examples depend on the methods available in Check.cs)
```

### Extensions - Useful Extension Methods

The Extensions class provides helpful extension methods for common .NET types including string manipulation, collection operations, and data transformations.

```csharp
using ZidUtilities.CommonCode;

// Example usage of extension methods
// (Specific examples depend on available extension methods)
```

## Common Use Cases

### Security & Encryption
- Encrypting sensitive configuration data
- Protecting user credentials before storage
- Securing file transfers and backups
- Computing file integrity checksums
- Generating secure temporary passwords

### Password Management
- User password generation during registration
- Temporary password creation for password resets
- API key generation
- Token generation for authentication

### Data Processing
- Comparing datasets for changes
- Validating and sanitizing user input
- Image format conversions
- Server-side data filtering

## Security Best Practices

1. **Always use AES for new applications** - It's the industry standard
2. **Use strong encryption keys** - At least 12 characters with mixed case, numbers, and symbols
3. **Use SHA256 or SHA512 for hashing** - MD5 and SHA1 are deprecated
4. **Store encryption keys securely** - Never hard-code keys in source code
5. **Use high iteration counts** - 100,000+ iterations for password-based key derivation
6. **Enable modern KDF** - Set `UseModernKdf = true` for PBKDF2 support

## Performance Considerations

- **File Encryption**: Large files are processed in 80KB chunks for optimal performance
- **Hash Functions**: SHA256 provides good balance of security and speed
- **Password Generation**: Minimal overhead, suitable for bulk generation
- **Key Derivation**: Higher iteration counts increase security but add computational cost
