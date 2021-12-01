using Azure.FX;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace Azure
{
    // https://github.com/Azure/azure-sdk-for-net/blob/main/sdk/keyvault/Azure.Security.KeyVault.Secrets/README.md

    public class KeyVaultProvider : SettingsServices
    {
        SecretClient _keyVault;
        public KeyVaultProvider(string uri){
            _keyVault = new SecretClient(new System.Uri(uri), new DefaultAzureCredential());
        }

        public override string GetSecret(string secretName)
            => _keyVault.GetSecret(secretName).Value.Value;

        public override string GetString(string key)
        {
            throw new System.NotImplementedException();
        }
    }
}