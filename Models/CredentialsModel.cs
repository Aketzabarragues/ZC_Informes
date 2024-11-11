using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZC_Informes.Helpers;

namespace ZC_Informes.Models
{
    public class CredentialsModel
    {

        public string EncryptedUserPassword { get; set; }
        public string EncryptedMasterPassword { get; set; }

        public CredentialsModel(string userPassword, string masterPassword)
        {
            EncryptedUserPassword = ConfigEncryptorHelper.Encrypt(userPassword);
            EncryptedMasterPassword = ConfigEncryptorHelper.Encrypt(masterPassword);
        }

        public string GetDecryptedUserPassword() => ConfigEncryptorHelper.Decrypt(EncryptedUserPassword);
        public string GetDecryptedMasterPassword() => ConfigEncryptorHelper.Decrypt(EncryptedMasterPassword);

    }
}
