using System;

namespace Azure.FX
{
    public abstract class SettingsServices
    {
        public abstract string GetSecret(string secretName);

        public abstract string GetString(string key);
    }
}
