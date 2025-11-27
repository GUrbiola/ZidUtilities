using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZidUtilities.CommonCode
{
    public enum SerializationType { XML, JSON, PlainText }
    public enum SaveLocation { ApplicationFolder, UserAppDataFolder, CommonAppDataFolder, CustomFolder }

    /// <summary>
    /// Simple helper that persists a dictionary of string key/value pairs to disk using
    /// different serialization formats and optional encryption.
    /// </summary>
    public class SimpleDictionaryPersister
    {
        private Crypter Encrypter;

        private Dictionary<string, string> SettingsDictionary { get; set; }

        /// <summary>
        /// Encryption key used when <see cref="IsDataEncrypted"/> is true.
        /// </summary>
        public string EncryptionKey { get; set; }

        /// <summary>
        /// When true, values will be encrypted using <see cref="Encrypter"/> during serialization
        /// and decrypted during deserialization.
        /// </summary>
        public bool IsDataEncrypted { get; set; }

        /// <summary>
        /// Controls which format is used when serializing/deserializing the dictionary.
        /// </summary>
        public SerializationType TypeOfSerialization { get; set; }

        /// <summary>
        /// File name used when saving/loading the serialized data.
        /// If empty or blank a default of "Settings.cid" is used.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Separator string used for the PlainText serialization format between key and value.
        /// </summary>
        public string Separator { get; set; }

        /// <summary>
        /// Provides an enumerator over the internal settings dictionary.
        /// </summary>
        /// <remarks>
        /// Use this to iterate the stored key/value pairs without exposing the dictionary directly.
        /// </remarks>
        public Dictionary<string, string>.Enumerator Enumerator { get { return SettingsDictionary.GetEnumerator(); } }

        /// <summary>
        /// Where the file will be written/read from when saving/loading.
        /// </summary>
        public SaveLocation FileLocation { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleDictionaryPersister"/> class
        /// with sensible defaults for encryption, serialization and storage location.
        /// </summary>
        /// <remarks>
        /// Default values:
        /// - <see cref="EncryptionKey"/> = "@DefaultEncryptionKeyValue@123"
        /// - <see cref="IsDataEncrypted"/> = false
        /// - <see cref="TypeOfSerialization"/> = <see cref="SerializationType.PlainText"/>
        /// - <see cref="Separator"/> = ":="
        /// - <see cref="FileLocation"/> = <see cref="SaveLocation.UserAppDataFolder"/>
        /// </remarks>
        public SimpleDictionaryPersister()
        {
            EncryptionKey = "@DefaultEncryptionKeyValue@123";
            IsDataEncrypted = false;
            Encrypter = new Crypter();
            TypeOfSerialization = SerializationType.PlainText;
            SettingsDictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            Separator = ":=";
            FileLocation = SaveLocation.UserAppDataFolder;
        }

        /// <summary>
        /// Indexer to get or set a setting value by key.
        /// </summary>
        /// <param name="KeyName">The case-insensitive key name.</param>
        /// <returns>
        /// The value associated with <paramref name="KeyName"/> or null if the key does not exist.
        /// When setting, the value is added or updated.
        /// </returns>
        public string this[string KeyName]
        {
            get
            {
                string back = null;
                if (SettingsDictionary != null || SettingsDictionary.Count > 0)
                {
                    if (SettingsDictionary.ContainsKey(KeyName))
                    {
                        back = SettingsDictionary[KeyName];
                    }
                }
                return back;
            }
            set
            {
                if (SettingsDictionary.ContainsKey(KeyName))
                {
                    SettingsDictionary[KeyName] = value;
                }
                else
                {
                    SettingsDictionary.Add(KeyName, value);
                }
            }
        }

        /// <summary>
        /// Number of settings currently stored in the dictionary.
        /// </summary>
        public int SettingsCount { get { return SettingsDictionary.Count; } }

        /// <summary>
        /// Adds a new setting or updates the value of an existing setting.
        /// </summary>
        /// <param name="KeyName">The key name to add or update.</param>
        /// <param name="KeyValue">The value to set for the key.</param>
        public void AddOrUpdateSetting(string KeyName, string KeyValue)
        {
            this[KeyName] = KeyValue;
        }

        /// <summary>
        /// Retrieves the value for a given setting key.
        /// </summary>
        /// <param name="KeyName">The key whose value should be returned.</param>
        /// <returns>The value associated with <paramref name="KeyName"/> or null if not found.</returns>
        public string GetSetting(string KeyName)
        {
            return this[KeyName];
        }

        /// <summary>
        /// Determines whether a setting with the specified key exists.
        /// </summary>
        /// <param name="KeyName">The key to look for.</param>
        /// <returns>True if the key exists; otherwise false.</returns>
        public bool ContainsSetting(string KeyName)
        {
            return SettingsDictionary.ContainsKey(KeyName);
        }

        /// <summary>
        /// Removes a setting by key if it exists.
        /// </summary>
        /// <param name="KeyName">The key to remove.</param>
        public void RemoveSetting(string KeyName)
        {
            if (SettingsDictionary.ContainsKey(KeyName))
                SettingsDictionary.Remove(KeyName);
        }

        /// <summary>
        /// Serializes the settings dictionary to disk using the configured
        /// <see cref="TypeOfSerialization"/> and optionally encrypts values when
        /// <see cref="IsDataEncrypted"/> is true.
        /// </summary>
        /// <remarks>
        /// The method chooses the target folder based on <see cref="FileLocation"/> and uses
        /// <see cref="FileName"/> (or "Settings.cid" if not set). Throws an exception if writing fails.
        /// </remarks>
        public void Serialize()
        {
            string SerializableString;
            try
            {
                if (SettingsDictionary.Count > 0)
                {
                    switch (TypeOfSerialization)
                    {
                        case SerializationType.XML:

                            if (IsDataEncrypted)
                            {
                                Dictionary<string, string> EncryptedSettings = new Dictionary<string, string>();
                                foreach (KeyValuePair<string, string> setting in SettingsDictionary)
                                {
                                    EncryptedSettings.Add(setting.Key, Encrypter.Encrypt(setting.Value, EncryptionKey));
                                }
                                SerializableString = EncryptedSettings.SerializeToXmlString();
                            }
                            else
                            {
                                SerializableString = SettingsDictionary.SerializeToXmlString();
                            }
                            break;
                        case SerializationType.JSON:
                            SerializableString = "{" + Environment.NewLine;
                            KeyValuePair<string, string> lastValue = SettingsDictionary.Last();
                            foreach (KeyValuePair<string, string> setting in SettingsDictionary)
                            {
                                string value = IsDataEncrypted ? Encrypter.Encrypt(setting.Value, EncryptionKey) : setting.Value;
                                if (setting.Equals(lastValue))
                                {
                                    SerializableString += String.Format("{0}:{1}{2}", setting.Key.EnsureQuotedString(), value.EnsureQuotedString(), Environment.NewLine);
                                }
                                else
                                {
                                    SerializableString += String.Format("{0}:{1},{2}", setting.Key.EnsureQuotedString(), value.EnsureQuotedString(), Environment.NewLine);
                                }
                            }
                            SerializableString += "}" + Environment.NewLine;
                            break;
                        case SerializationType.PlainText:
                            SerializableString = "";
                            foreach (KeyValuePair<string, string> setting in SettingsDictionary)
                            {
                                string value = IsDataEncrypted ? Encrypter.Encrypt(setting.Value, EncryptionKey) : setting.Value;
                                SerializableString += String.Format("{0}{1}{2}{3}", setting.Key, Separator, value, Environment.NewLine);
                            }
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                else
                {
                    SerializableString = "";
                }

                string filePath = "";
                switch (FileLocation)
                {
                    case SaveLocation.ApplicationFolder:
                        filePath = AppDomain.CurrentDomain.BaseDirectory + Path.DirectorySeparatorChar;
                        break;
                    case SaveLocation.UserAppDataFolder:
                        filePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + Path.DirectorySeparatorChar;
                        break;
                    case SaveLocation.CommonAppDataFolder:
                        filePath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + Path.DirectorySeparatorChar;
                        break;
                    case SaveLocation.CustomFolder:
                    default:
                        filePath = "";
                        break;
                }

                if (!String.IsNullOrEmpty(filePath) && !Directory.Exists(filePath))
                    Directory.CreateDirectory(filePath);

                FileName = FileName.IsNullEmptyOrBlank() ? "Settings.cid" : FileName;

                using (StreamWriter sw = new StreamWriter(Path.Combine(filePath, FileName)))
                {
                    sw.Write(SerializableString);
                    sw.Flush();
                    sw.Close();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error while trying to write the setting file.", ex);
            }
        }

        /// <summary>
        /// Deserializes the settings dictionary from disk using the configured
        /// <see cref="TypeOfSerialization"/> and optionally decrypts values when
        /// <see cref="IsDataEncrypted"/> is true.
        /// </summary>
        /// <remarks>
        /// The method expects the file at <see cref="FileName"/> (or "Settings.cid" if not set).
        /// Throws <see cref="FileNotFoundException"/> if the file does not exist.
        /// After successful execution, <see cref="SettingsDictionary"/> contains the loaded key/value pairs.
        /// </remarks>
        public void Deserialize()
        {
            string SerializedString;

            if (File.Exists(FileName.IsNullEmptyOrBlank() ? "Settings.cid" : FileName))
            {
                using (StreamReader sr = new StreamReader(FileName.IsNullEmptyOrBlank() ? "Settings.cid" : FileName))
                {
                    SerializedString = sr.ReadToEnd();
                }
            }
            else
            {
                throw new FileNotFoundException("The file the settings serialized was not found.");
            }

            SettingsDictionary = new Dictionary<string, string>();
            if (SerializedString.IsNullEmptyOrBlank())
                return;

            switch (TypeOfSerialization)
            {
                case SerializationType.XML:
                    SettingsDictionary = SerializedString.DeserializeFromXmlString() as Dictionary<string, string>;
                    if (IsDataEncrypted)
                    {
                        List<string> keys = new List<string>();
                        List<string> values = new List<string>();
                        foreach (KeyValuePair<string, string> setting in SettingsDictionary)
                        {
                            keys.Add(setting.Key);
                            values.Add(setting.Value);
                        }
                        for (int i = 0; i < keys.Count; i++)
                        {
                            SettingsDictionary[keys[i]] = Encrypter.Decrypt(values[i], EncryptionKey);
                        }
                    }
                    break;
                case SerializationType.JSON:
                    TextReader trJSON = new StringReader(SerializedString);
                    string curLineJSON;
                    while (!(curLineJSON = trJSON.ReadLine()).IsNullEmptyOrBlank())
                    {
                        if (curLineJSON.StartsWith("{"))
                            continue;
                        if (curLineJSON.StartsWith("}"))
                            break;

                        string[] data = curLineJSON.Split(new string[] { "\":\"" }, StringSplitOptions.None);
                        string value;
                        if (data[1].Contains(","))
                        {
                            value = data[1].TruncateLastChars(data[1].Length - data[1].LastIndexOf(',')).Trim('"');
                        }
                        else
                        {
                            value = data[1].Trim().Trim('"');
                        }
                        SettingsDictionary.Add(data[0].Trim('"'), IsDataEncrypted ? Encrypter.Decrypt(value, EncryptionKey) : value);
                    }
                    break;
                case SerializationType.PlainText:
                    TextReader trTXT = new StringReader(SerializedString);
                    string curLineTXT;
                    while (!(curLineTXT = trTXT.ReadLine()).IsNullEmptyOrBlank())
                    {
                        string[] data = curLineTXT.Split(new string[] { Separator }, StringSplitOptions.None);
                        SettingsDictionary.Add(data[0], IsDataEncrypted ? Encrypter.Decrypt(data[1], EncryptionKey) : data[1]);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Clears all stored settings from the internal dictionary.
        /// </summary>
        public void Clear()
        {
            SettingsDictionary.Clear();
        }
    }
}
