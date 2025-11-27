using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ZidUtilities.CommonCode;

namespace Tester
{
    /// <summary>
    /// Comprehensive test suite for the SimpleDictionaryPersister class.
    /// Tests serialization, deserialization, encryption, and various storage formats.
    /// </summary>
    public class SimpleDictionaryPersisterTests
    {
        private int _testsRun = 0;
        private int _testsPassed = 0;
        private int _testsFailed = 0;
        private List<string> _failureMessages = new List<string>();

        /// <summary>
        /// Runs all tests for SimpleDictionaryPersister
        /// </summary>
        public void RunAllTests()
        {
            Console.WriteLine("==============================================");
            Console.WriteLine("SimpleDictionaryPersister - Test Suite");
            Console.WriteLine("==============================================");
            Console.WriteLine();

            // Basic functionality tests
            Test_Constructor_SetsDefaultValues();
            Test_AddOrUpdateSetting_AddsNewSetting();
            Test_AddOrUpdateSetting_UpdatesExistingSetting();
            Test_GetSetting_ReturnsCorrectValue();
            Test_GetSetting_ReturnsNullForNonExistentKey();
            Test_Indexer_GetAndSet();
            Test_ContainsSetting_ReturnsTrueForExistingKey();
            Test_ContainsSetting_ReturnsFalseForNonExistentKey();
            Test_RemoveSetting_RemovesExistingKey();
            Test_RemoveSetting_IgnoresNonExistentKey();
            Test_SettingsCount_ReturnsCorrectCount();
            Test_Clear_RemovesAllSettings();
            Test_CaseInsensitiveKeys();

            // Serialization tests - PlainText
            Test_Serialize_PlainText_NoEncryption();
            Test_Deserialize_PlainText_NoEncryption();
            Test_Serialize_PlainText_WithEncryption();
            Test_Deserialize_PlainText_WithEncryption();
            Test_PlainText_CustomSeparator();

            // Serialization tests - JSON
            Test_Serialize_JSON_NoEncryption();
            Test_Deserialize_JSON_NoEncryption();
            Test_Serialize_JSON_WithEncryption();
            Test_Deserialize_JSON_WithEncryption();

            // Serialization tests - XML
            Test_Serialize_XML_NoEncryption();
            Test_Deserialize_XML_NoEncryption();
            Test_Serialize_XML_WithEncryption();
            Test_Deserialize_XML_WithEncryption();

            // File location tests
            Test_SaveLocation_ApplicationFolder();
            Test_SaveLocation_CustomFileName();

            // Edge cases
            Test_EmptyDictionary_Serialization();
            Test_SpecialCharacters_InValues();
            Test_MultilineValues();
            Test_Enumerator_IteratesAllItems();

            // Print summary
            PrintTestSummary();
        }

        #region Basic Functionality Tests

        private void Test_Constructor_SetsDefaultValues()
        {
            RunTest("Constructor sets default values", () =>
            {
                var persister = new SimpleDictionaryPersister();

                Assert(persister.EncryptionKey == "@DefaultEncryptionKeyValue@123", "Default encryption key should be set");
                Assert(persister.IsDataEncrypted == false, "IsDataEncrypted should be false by default");
                Assert(persister.TypeOfSerialization == SerializationType.PlainText, "Default serialization type should be PlainText");
                Assert(persister.Separator == ":=", "Default separator should be ':='");
                Assert(persister.FileLocation == SaveLocation.UserAppDataFolder, "Default location should be UserAppDataFolder");
                Assert(persister.SettingsCount == 0, "Settings count should be 0 initially");
            });
        }

        private void Test_AddOrUpdateSetting_AddsNewSetting()
        {
            RunTest("AddOrUpdateSetting adds new setting", () =>
            {
                var persister = new SimpleDictionaryPersister();
                persister.AddOrUpdateSetting("TestKey", "TestValue");

                Assert(persister.SettingsCount == 1, "Settings count should be 1");
                Assert(persister.GetSetting("TestKey") == "TestValue", "Value should match");
            });
        }

        private void Test_AddOrUpdateSetting_UpdatesExistingSetting()
        {
            RunTest("AddOrUpdateSetting updates existing setting", () =>
            {
                var persister = new SimpleDictionaryPersister();
                persister.AddOrUpdateSetting("TestKey", "OldValue");
                persister.AddOrUpdateSetting("TestKey", "NewValue");

                Assert(persister.SettingsCount == 1, "Settings count should still be 1");
                Assert(persister.GetSetting("TestKey") == "NewValue", "Value should be updated");
            });
        }

        private void Test_GetSetting_ReturnsCorrectValue()
        {
            RunTest("GetSetting returns correct value", () =>
            {
                var persister = new SimpleDictionaryPersister();
                persister["Key1"] = "Value1";

                Assert(persister.GetSetting("Key1") == "Value1", "Should return correct value");
            });
        }

        private void Test_GetSetting_ReturnsNullForNonExistentKey()
        {
            RunTest("GetSetting returns null for non-existent key", () =>
            {
                var persister = new SimpleDictionaryPersister();

                Assert(persister.GetSetting("NonExistent") == null, "Should return null for non-existent key");
            });
        }

        private void Test_Indexer_GetAndSet()
        {
            RunTest("Indexer get and set work correctly", () =>
            {
                var persister = new SimpleDictionaryPersister();
                persister["IndexKey"] = "IndexValue";

                Assert(persister["IndexKey"] == "IndexValue", "Indexer should work for get and set");
            });
        }

        private void Test_ContainsSetting_ReturnsTrueForExistingKey()
        {
            RunTest("ContainsSetting returns true for existing key", () =>
            {
                var persister = new SimpleDictionaryPersister();
                persister["ExistingKey"] = "Value";

                Assert(persister.ContainsSetting("ExistingKey") == true, "Should return true for existing key");
            });
        }

        private void Test_ContainsSetting_ReturnsFalseForNonExistentKey()
        {
            RunTest("ContainsSetting returns false for non-existent key", () =>
            {
                var persister = new SimpleDictionaryPersister();

                Assert(persister.ContainsSetting("NonExistent") == false, "Should return false for non-existent key");
            });
        }

        private void Test_RemoveSetting_RemovesExistingKey()
        {
            RunTest("RemoveSetting removes existing key", () =>
            {
                var persister = new SimpleDictionaryPersister();
                persister["KeyToRemove"] = "Value";
                persister.RemoveSetting("KeyToRemove");

                Assert(persister.ContainsSetting("KeyToRemove") == false, "Key should be removed");
                Assert(persister.SettingsCount == 0, "Settings count should be 0");
            });
        }

        private void Test_RemoveSetting_IgnoresNonExistentKey()
        {
            RunTest("RemoveSetting ignores non-existent key", () =>
            {
                var persister = new SimpleDictionaryPersister();
                persister.RemoveSetting("NonExistent"); // Should not throw

                Assert(persister.SettingsCount == 0, "Settings count should remain 0");
            });
        }

        private void Test_SettingsCount_ReturnsCorrectCount()
        {
            RunTest("SettingsCount returns correct count", () =>
            {
                var persister = new SimpleDictionaryPersister();
                persister["Key1"] = "Value1";
                persister["Key2"] = "Value2";
                persister["Key3"] = "Value3";

                Assert(persister.SettingsCount == 3, "Settings count should be 3");
            });
        }

        private void Test_Clear_RemovesAllSettings()
        {
            RunTest("Clear removes all settings", () =>
            {
                var persister = new SimpleDictionaryPersister();
                persister["Key1"] = "Value1";
                persister["Key2"] = "Value2";
                persister.Clear();

                Assert(persister.SettingsCount == 0, "Settings count should be 0 after Clear");
            });
        }

        private void Test_CaseInsensitiveKeys()
        {
            RunTest("Keys are case-insensitive", () =>
            {
                var persister = new SimpleDictionaryPersister();
                persister["TestKey"] = "Value1";
                persister["testkey"] = "Value2"; // Should update, not add

                Assert(persister.SettingsCount == 1, "Should only have 1 setting (case-insensitive)");
                Assert(persister["TESTKEY"] == "Value2", "Should retrieve with different case");
            });
        }

        #endregion

        #region PlainText Serialization Tests

        private void Test_Serialize_PlainText_NoEncryption()
        {
            RunTest("Serialize PlainText without encryption", () =>
            {
                var persister = new SimpleDictionaryPersister
                {
                    TypeOfSerialization = SerializationType.PlainText,
                    IsDataEncrypted = false,
                    FileName = "test_plaintext.txt",
                    FileLocation = SaveLocation.ApplicationFolder
                };

                persister["Setting1"] = "Value1";
                persister["Setting2"] = "Value2";
                persister.Serialize();

                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test_plaintext.txt");
                Assert(File.Exists(filePath), "File should be created");

                string content = File.ReadAllText(filePath);
                Assert(content.Contains("Setting1:=Value1"), "Content should contain Setting1");
                Assert(content.Contains("Setting2:=Value2"), "Content should contain Setting2");

                // Cleanup
                if (File.Exists(filePath)) File.Delete(filePath);
            });
        }

        private void Test_Deserialize_PlainText_NoEncryption()
        {
            RunTest("Deserialize PlainText without encryption", () =>
            {
                // Setup - create a file
                var persister1 = new SimpleDictionaryPersister
                {
                    TypeOfSerialization = SerializationType.PlainText,
                    IsDataEncrypted = false,
                    FileName = "test_deserialize_plaintext.txt",
                    FileLocation = SaveLocation.ApplicationFolder
                };

                persister1["Key1"] = "Value1";
                persister1["Key2"] = "Value2";
                persister1.Serialize();

                // Test - deserialize
                var persister2 = new SimpleDictionaryPersister
                {
                    TypeOfSerialization = SerializationType.PlainText,
                    IsDataEncrypted = false,
                    FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test_deserialize_plaintext.txt")
                };

                persister2.Deserialize();

                Assert(persister2.SettingsCount == 2, "Should have 2 settings");
                Assert(persister2["Key1"] == "Value1", "Key1 should match");
                Assert(persister2["Key2"] == "Value2", "Key2 should match");

                // Cleanup
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test_deserialize_plaintext.txt");
                if (File.Exists(filePath)) File.Delete(filePath);
            });
        }

        private void Test_Serialize_PlainText_WithEncryption()
        {
            RunTest("Serialize PlainText with encryption", () =>
            {
                var persister = new SimpleDictionaryPersister
                {
                    TypeOfSerialization = SerializationType.PlainText,
                    IsDataEncrypted = true,
                    EncryptionKey = "TestPassword123",
                    FileName = "test_encrypted_plaintext.txt",
                    FileLocation = SaveLocation.ApplicationFolder
                };

                persister["SecretKey"] = "SecretValue";
                persister.Serialize();

                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test_encrypted_plaintext.txt");
                Assert(File.Exists(filePath), "File should be created");

                string content = File.ReadAllText(filePath);
                Assert(!content.Contains("SecretValue"), "Content should be encrypted (not contain plain value)");
                Assert(content.Contains("SecretKey"), "Content should contain the key (not encrypted)");

                // Cleanup
                if (File.Exists(filePath)) File.Delete(filePath);
            });
        }

        private void Test_Deserialize_PlainText_WithEncryption()
        {
            RunTest("Deserialize PlainText with encryption", () =>
            {
                string testPassword = "MyTestPassword456";

                // Setup - serialize with encryption
                var persister1 = new SimpleDictionaryPersister
                {
                    TypeOfSerialization = SerializationType.PlainText,
                    IsDataEncrypted = true,
                    EncryptionKey = testPassword,
                    FileName = "test_decrypt_plaintext.txt",
                    FileLocation = SaveLocation.ApplicationFolder
                };

                persister1["EncryptedKey"] = "EncryptedValue";
                persister1.Serialize();

                // Test - deserialize with same password
                var persister2 = new SimpleDictionaryPersister
                {
                    TypeOfSerialization = SerializationType.PlainText,
                    IsDataEncrypted = true,
                    EncryptionKey = testPassword,
                    FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test_decrypt_plaintext.txt")
                };

                persister2.Deserialize();

                Assert(persister2["EncryptedKey"] == "EncryptedValue", "Decrypted value should match original");

                // Cleanup
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test_decrypt_plaintext.txt");
                if (File.Exists(filePath)) File.Delete(filePath);
            });
        }

        private void Test_PlainText_CustomSeparator()
        {
            RunTest("PlainText with custom separator", () =>
            {
                var persister = new SimpleDictionaryPersister
                {
                    TypeOfSerialization = SerializationType.PlainText,
                    Separator = "|||",
                    FileName = "test_custom_separator.txt",
                    FileLocation = SaveLocation.ApplicationFolder
                };

                persister["Key1"] = "Value1";
                persister.Serialize();

                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test_custom_separator.txt");
                string content = File.ReadAllText(filePath);

                Assert(content.Contains("Key1|||Value1"), "Should use custom separator");

                // Cleanup
                if (File.Exists(filePath)) File.Delete(filePath);
            });
        }

        #endregion

        #region JSON Serialization Tests

        private void Test_Serialize_JSON_NoEncryption()
        {
            RunTest("Serialize JSON without encryption", () =>
            {
                var persister = new SimpleDictionaryPersister
                {
                    TypeOfSerialization = SerializationType.JSON,
                    IsDataEncrypted = false,
                    FileName = "test_json.json",
                    FileLocation = SaveLocation.ApplicationFolder
                };

                persister["JsonKey1"] = "JsonValue1";
                persister["JsonKey2"] = "JsonValue2";
                persister.Serialize();

                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test_json.json");
                Assert(File.Exists(filePath), "JSON file should be created");

                string content = File.ReadAllText(filePath);
                Assert(content.Contains("JsonKey1"), "Should contain JsonKey1");
                Assert(content.Contains("JsonValue1"), "Should contain JsonValue1");
                Assert(content.StartsWith("{"), "Should start with {");
                Assert(content.TrimEnd().EndsWith("}"), "Should end with }");

                // Cleanup
                if (File.Exists(filePath)) File.Delete(filePath);
            });
        }

        private void Test_Deserialize_JSON_NoEncryption()
        {
            RunTest("Deserialize JSON without encryption", () =>
            {
                // Setup
                var persister1 = new SimpleDictionaryPersister
                {
                    TypeOfSerialization = SerializationType.JSON,
                    IsDataEncrypted = false,
                    FileName = "test_deserialize_json.json",
                    FileLocation = SaveLocation.ApplicationFolder
                };

                persister1["JsonKey1"] = "JsonValue1";
                persister1["JsonKey2"] = "JsonValue2";
                persister1.Serialize();

                // Test
                var persister2 = new SimpleDictionaryPersister
                {
                    TypeOfSerialization = SerializationType.JSON,
                    IsDataEncrypted = false,
                    FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test_deserialize_json.json")
                };

                persister2.Deserialize();

                Assert(persister2.SettingsCount == 2, "Should have 2 settings");
                Assert(persister2["JsonKey1"] == "JsonValue1", "JsonKey1 should match");
                Assert(persister2["JsonKey2"] == "JsonValue2", "JsonKey2 should match");

                // Cleanup
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test_deserialize_json.json");
                if (File.Exists(filePath)) File.Delete(filePath);
            });
        }

        private void Test_Serialize_JSON_WithEncryption()
        {
            RunTest("Serialize JSON with encryption", () =>
            {
                var persister = new SimpleDictionaryPersister
                {
                    TypeOfSerialization = SerializationType.JSON,
                    IsDataEncrypted = true,
                    EncryptionKey = "JsonPassword789",
                    FileName = "test_encrypted_json.json",
                    FileLocation = SaveLocation.ApplicationFolder
                };

                persister["SecretJsonKey"] = "SecretJsonValue";
                persister.Serialize();

                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test_encrypted_json.json");
                string content = File.ReadAllText(filePath);

                Assert(!content.Contains("SecretJsonValue"), "Value should be encrypted");

                // Cleanup
                if (File.Exists(filePath)) File.Delete(filePath);
            });
        }

        private void Test_Deserialize_JSON_WithEncryption()
        {
            RunTest("Deserialize JSON with encryption", () =>
            {
                string password = "JsonEncryptPass";

                // Setup
                var persister1 = new SimpleDictionaryPersister
                {
                    TypeOfSerialization = SerializationType.JSON,
                    IsDataEncrypted = true,
                    EncryptionKey = password,
                    FileName = "test_decrypt_json.json",
                    FileLocation = SaveLocation.ApplicationFolder
                };

                persister1["EncryptedJsonKey"] = "EncryptedJsonValue";
                persister1.Serialize();

                // Test
                var persister2 = new SimpleDictionaryPersister
                {
                    TypeOfSerialization = SerializationType.JSON,
                    IsDataEncrypted = true,
                    EncryptionKey = password,
                    FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test_decrypt_json.json")
                };

                persister2.Deserialize();

                Assert(persister2["EncryptedJsonKey"] == "EncryptedJsonValue", "Decrypted JSON value should match");

                // Cleanup
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test_decrypt_json.json");
                if (File.Exists(filePath)) File.Delete(filePath);
            });
        }

        #endregion

        #region XML Serialization Tests

        private void Test_Serialize_XML_NoEncryption()
        {
            RunTest("Serialize XML without encryption", () =>
            {
                var persister = new SimpleDictionaryPersister
                {
                    TypeOfSerialization = SerializationType.XML,
                    IsDataEncrypted = false,
                    FileName = "test_xml.xml",
                    FileLocation = SaveLocation.ApplicationFolder
                };

                persister["XmlKey1"] = "XmlValue1";
                persister["XmlKey2"] = "XmlValue2";
                persister.Serialize();

                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test_xml.xml");
                Assert(File.Exists(filePath), "XML file should be created");

                string content = File.ReadAllText(filePath);
                Assert(content.Contains("XmlKey1"), "Should contain XmlKey1");
                Assert(content.Contains("XmlValue1"), "Should contain XmlValue1");

                // Cleanup
                if (File.Exists(filePath)) File.Delete(filePath);
            });
        }

        private void Test_Deserialize_XML_NoEncryption()
        {
            RunTest("Deserialize XML without encryption", () =>
            {
                // Setup
                var persister1 = new SimpleDictionaryPersister
                {
                    TypeOfSerialization = SerializationType.XML,
                    IsDataEncrypted = false,
                    FileName = "test_deserialize_xml.xml",
                    FileLocation = SaveLocation.ApplicationFolder
                };

                persister1["XmlKey1"] = "XmlValue1";
                persister1["XmlKey2"] = "XmlValue2";
                persister1.Serialize();

                // Test
                var persister2 = new SimpleDictionaryPersister
                {
                    TypeOfSerialization = SerializationType.XML,
                    IsDataEncrypted = false,
                    FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test_deserialize_xml.xml")
                };

                persister2.Deserialize();

                Assert(persister2.SettingsCount == 2, "Should have 2 settings");
                Assert(persister2["XmlKey1"] == "XmlValue1", "XmlKey1 should match");
                Assert(persister2["XmlKey2"] == "XmlValue2", "XmlKey2 should match");

                // Cleanup
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test_deserialize_xml.xml");
                if (File.Exists(filePath)) File.Delete(filePath);
            });
        }

        private void Test_Serialize_XML_WithEncryption()
        {
            RunTest("Serialize XML with encryption", () =>
            {
                var persister = new SimpleDictionaryPersister
                {
                    TypeOfSerialization = SerializationType.XML,
                    IsDataEncrypted = true,
                    EncryptionKey = "XmlPassword123",
                    FileName = "test_encrypted_xml.xml",
                    FileLocation = SaveLocation.ApplicationFolder
                };

                persister["SecretXmlKey"] = "SecretXmlValue";
                persister.Serialize();

                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test_encrypted_xml.xml");
                string content = File.ReadAllText(filePath);

                Assert(!content.Contains("SecretXmlValue"), "Value should be encrypted");

                // Cleanup
                if (File.Exists(filePath)) File.Delete(filePath);
            });
        }

        private void Test_Deserialize_XML_WithEncryption()
        {
            RunTest("Deserialize XML with encryption", () =>
            {
                string password = "XmlEncryptPass";

                // Setup
                var persister1 = new SimpleDictionaryPersister
                {
                    TypeOfSerialization = SerializationType.XML,
                    IsDataEncrypted = true,
                    EncryptionKey = password,
                    FileName = "test_decrypt_xml.xml",
                    FileLocation = SaveLocation.ApplicationFolder
                };

                persister1["EncryptedXmlKey"] = "EncryptedXmlValue";
                persister1.Serialize();

                // Test
                var persister2 = new SimpleDictionaryPersister
                {
                    TypeOfSerialization = SerializationType.XML,
                    IsDataEncrypted = true,
                    EncryptionKey = password,
                    FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test_decrypt_xml.xml")
                };

                persister2.Deserialize();

                Assert(persister2["EncryptedXmlKey"] == "EncryptedXmlValue", "Decrypted XML value should match");

                // Cleanup
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test_decrypt_xml.xml");
                if (File.Exists(filePath)) File.Delete(filePath);
            });
        }

        #endregion

        #region File Location Tests

        private void Test_SaveLocation_ApplicationFolder()
        {
            RunTest("Save to Application Folder", () =>
            {
                var persister = new SimpleDictionaryPersister
                {
                    FileLocation = SaveLocation.ApplicationFolder,
                    FileName = "test_app_folder.txt"
                };

                persister["TestKey"] = "TestValue";
                persister.Serialize();

                string expectedPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test_app_folder.txt");
                Assert(File.Exists(expectedPath), "File should exist in application folder");

                // Cleanup
                if (File.Exists(expectedPath)) File.Delete(expectedPath);
            });
        }

        private void Test_SaveLocation_CustomFileName()
        {
            RunTest("Custom file name", () =>
            {
                var persister = new SimpleDictionaryPersister
                {
                    FileLocation = SaveLocation.ApplicationFolder,
                    FileName = "MyCustomSettings.config"
                };

                persister["Key"] = "Value";
                persister.Serialize();

                string expectedPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MyCustomSettings.config");
                Assert(File.Exists(expectedPath), "File with custom name should exist");

                // Cleanup
                if (File.Exists(expectedPath)) File.Delete(expectedPath);
            });
        }

        #endregion

        #region Edge Case Tests

        private void Test_EmptyDictionary_Serialization()
        {
            RunTest("Serialize empty dictionary", () =>
            {
                var persister = new SimpleDictionaryPersister
                {
                    FileName = "test_empty.txt",
                    FileLocation = SaveLocation.ApplicationFolder
                };

                persister.Serialize(); // Should not throw

                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test_empty.txt");
                Assert(File.Exists(filePath), "File should be created even for empty dictionary");

                // Cleanup
                if (File.Exists(filePath)) File.Delete(filePath);
            });
        }

        private void Test_SpecialCharacters_InValues()
        {
            RunTest("Special characters in values", () =>
            {
                var persister1 = new SimpleDictionaryPersister
                {
                    TypeOfSerialization = SerializationType.PlainText,
                    FileName = "test_special_chars.txt",
                    FileLocation = SaveLocation.ApplicationFolder
                };

                persister1["SpecialKey"] = "Value with @#$%^&*() special chars!";
                persister1.Serialize();

                var persister2 = new SimpleDictionaryPersister
                {
                    TypeOfSerialization = SerializationType.PlainText,
                    FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test_special_chars.txt")
                };

                persister2.Deserialize();

                Assert(persister2["SpecialKey"] == "Value with @#$%^&*() special chars!", "Special characters should be preserved");

                // Cleanup
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test_special_chars.txt");
                if (File.Exists(filePath)) File.Delete(filePath);
            });
        }

        private void Test_MultilineValues()
        {
            RunTest("Multiline values", () =>
            {
                var persister1 = new SimpleDictionaryPersister
                {
                    TypeOfSerialization = SerializationType.XML,
                    FileName = "test_multiline.xml",
                    FileLocation = SaveLocation.ApplicationFolder
                };

                persister1["MultilineKey"] = "Line1\r\nLine2\r\nLine3";
                persister1.Serialize();

                var persister2 = new SimpleDictionaryPersister
                {
                    TypeOfSerialization = SerializationType.XML,
                    FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test_multiline.xml")
                };

                persister2.Deserialize();

                Assert(persister2["MultilineKey"].Contains("Line1") &&
                       persister2["MultilineKey"].Contains("Line2") &&
                       persister2["MultilineKey"].Contains("Line3"),
                       "Multiline values should be preserved");

                // Cleanup
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test_multiline.xml");
                if (File.Exists(filePath)) File.Delete(filePath);
            });
        }

        private void Test_Enumerator_IteratesAllItems()
        {
            RunTest("Enumerator iterates all items", () =>
            {
                var persister = new SimpleDictionaryPersister();
                persister["Key1"] = "Value1";
                persister["Key2"] = "Value2";
                persister["Key3"] = "Value3";

                int count = 0;
                var enumerator = persister.Enumerator;
                while (enumerator.MoveNext())
                {
                    count++;
                    var current = enumerator.Current;
                    Assert(!string.IsNullOrEmpty(current.Key), "Key should not be empty");
                    Assert(!string.IsNullOrEmpty(current.Value), "Value should not be empty");
                }

                Assert(count == 3, "Enumerator should iterate all 3 items");
            });
        }

        #endregion

        #region Test Infrastructure

        private void RunTest(string testName, Action testAction)
        {
            _testsRun++;
            try
            {
                testAction();
                _testsPassed++;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"[PASS] {testName}");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                _testsFailed++;
                _failureMessages.Add($"{testName}: {ex.Message}");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[FAIL] {testName}");
                Console.WriteLine($"       {ex.Message}");
                Console.ResetColor();
            }
        }

        private void Assert(bool condition, string message)
        {
            if (!condition)
            {
                throw new Exception($"Assertion failed: {message}");
            }
        }

        private void PrintTestSummary()
        {
            Console.WriteLine();
            Console.WriteLine("==============================================");
            Console.WriteLine("Test Summary");
            Console.WriteLine("==============================================");
            Console.WriteLine($"Total Tests Run:    {_testsRun}");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Tests Passed:       {_testsPassed}");
            Console.ResetColor();

            if (_testsFailed > 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Tests Failed:       {_testsFailed}");
                Console.ResetColor();

                Console.WriteLine();
                Console.WriteLine("Failed Tests:");
                foreach (string failure in _failureMessages)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"  - {failure}");
                    Console.ResetColor();
                }
            }

            Console.WriteLine();
            double passRate = (_testsPassed / (double)_testsRun) * 100;
            Console.WriteLine($"Pass Rate: {passRate:F1}%");
            Console.WriteLine("==============================================");
        }

        #endregion
    }
}
