using Azure;
using Azure.Core;
using Azure.FX;
using Azure.FX.Core;
using Azure.FX.Providers;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Azure
{
    public abstract class AzureApplication 
    {
        string _vault;

        protected AzureApplication(string vault)
        {
            _vault = vault;
        }

        public StorageServices Storage { get; internal set; } 
        public MessagingServices Messaging { get; internal set; } 
        public SettingsServices Settings { get; internal set; } 

        public async Task RunAsync()
        {
            await OnIniitalizeAsyncCore().ConfigureAwait(false);   
        }
        public void Run() => RunAsync().GetAwaiter().GetResult();

        private void OnErrorCore(OperationError e)
        {
            OnError(e);
        }
        protected internal abstract void OnError(OperationError e);

        private async Task OnIniitalizeAsyncCore()
        {
            Settings = new KeyVaultProvider(_vault);

            var messagingConnection = Settings.GetSecret("messaging");
            Messaging = new ServiceBusProvider(messagingConnection, "queue");

            var storageConnection = Settings.GetSecret("storage");
            Storage = new BlobStorage(storageConnection);

            OnInitialize();
            await OnInitializeAsync().ConfigureAwait(false);
        }
        protected internal virtual Task OnInitializeAsync() => Task.CompletedTask;
        protected internal virtual void OnInitialize() { }

        #region Plumbing
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object obj) => base.Equals(obj);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode() => base.GetHashCode();

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string ToString() => base.ToString();
        #endregion
    }

    public class OperationError
    {
        public ProcessorOperation Operation { get; internal set; }

        public void Handled()
        {
            throw new NotImplementedException();
        }
    }
}
