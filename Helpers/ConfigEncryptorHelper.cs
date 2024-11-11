using System.Configuration;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Text.Json;

namespace ZC_Informes.Helpers
{
    public class ConfigEncryptorHelper
    {

        private static readonly string jsonFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config", "Credentials", "Credentials.json");


        public static string GetMasterPassword()
        {
            return GetDecryptedPassword("MasterPassword");
        }

        public static string GetUserPassword()
        {
            return GetDecryptedPassword("UserPassword");
        }


        public static void UpdateUserPassword(string newUserPassword)
        {
            string encryptedPassword = Encrypt(newUserPassword);
            UpdatePasswordInJson("UserPassword", encryptedPassword);
        }


        public static void InitializeMasterPassword(string masterPassword)
        {

            

            if (File.Exists(jsonFilePath))
            {
                var jsonContent = File.ReadAllText(jsonFilePath);
                var passwordData = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonContent);

                // Si ya existe la clave 'MasterPassword', no sobrescribirla.
                //if (passwordData != null && passwordData.ContainsKey("MasterPassword"))
                //{
                //    throw new InvalidOperationException("La contraseña maestra ya está inicializada.");
                //}
            }

            string encryptedMasterPassword = Encrypt(masterPassword);
            UpdatePasswordInJson("MasterPassword", encryptedMasterPassword);
        }


        // =============== Cifrar texto con ProtectedData (sin necesidad de clave)
        public static string Encrypt(string plainText)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(plainText);
            byte[] encryptedData = ProtectedData.Protect(bytes, null, DataProtectionScope.CurrentUser);
            return Convert.ToBase64String(encryptedData);
        }

        // =============== Desencriptar texto con ProtectedData
        public static string Decrypt(string encryptedText)
        {
            byte[] bytes = Convert.FromBase64String(encryptedText);
            byte[] decryptedData = ProtectedData.Unprotect(bytes, null, DataProtectionScope.CurrentUser);
            return Encoding.UTF8.GetString(decryptedData);
        }


        // ================== MÉTODOS DE GESTIÓN DE CONTRASEÑAS ====================
        private static string GetDecryptedPassword(string passwordKey)
        {
            
            if (!File.Exists(jsonFilePath))
                throw new FileNotFoundException("El archivo de configuración JSON no se encontró.");

            var jsonContent = File.ReadAllText(jsonFilePath);
            var passwordData = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonContent);

            if (passwordData == null || !passwordData.ContainsKey(passwordKey))
                throw new KeyNotFoundException($"La clave '{passwordKey}' no se encontró en el archivo de configuración.");

            return Decrypt(passwordData[passwordKey]);
        }

        private static void UpdatePasswordInJson(string passwordKey, string encryptedPassword)
        {
            Dictionary<string, string> passwordData;

            
            if (File.Exists(jsonFilePath))
            {
                var jsonContent = File.ReadAllText(jsonFilePath);
                passwordData = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonContent)
                               ?? new Dictionary<string, string>();
            }
            else
            {
                passwordData = new Dictionary<string, string>();
            }

            passwordData[passwordKey] = encryptedPassword;

            var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
            File.WriteAllText(jsonFilePath, JsonSerializer.Serialize(passwordData, jsonOptions));
        }

    }
}
