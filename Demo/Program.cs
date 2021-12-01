using Azure;
using Azure.FX;

class MyApplication : AzureApplication
{
    static void Main()
    {
        // this sets up all the core services based on secretes stored in secrets service (i.e. key vault)
        var app = new MyApplication(vault: "https://albums.vault.azure.net/");

        // this just simulates the first message being sent into the system
        app.Messaging.Send("Hi!");

        app.Run();
    }

    public MyApplication(string vault) : base(vault) {}

    protected override void OnInitialize()
    {
        // This sets up resources being observed, i.e. triggers.
        Messaging.MessageReceived += MessageReceived;
        Storage.BlobCreated += BlobAdded;

        // Messaging, Storage, Configuration are just instances of a simple abstractions built into AzureApplication,
        // but additional services can be added as fields of MyApplication and initalized here.
    }

    void MessageReceived(ReceiveOperation received)
    {
        // configuration service is built in
        var containerName = Settings.GetString("container");

        // All service APIs fail only on user error (bugs). Runtime errors are passed to the Error handler farhter below
        // i.e. the following call will not fail.
        UploadOperation upload = Storage.Upload(received.Data, containerName, Guid.NewGuid().ToString());

        // service APIs return "operations", which can be chained
        // this completes the message (removes it from the queue) after the blob upload succeeds 
        received.CompleteWhen(upload, OperationState.Completed);
    }

    void BlobAdded(DownloadOperation downloaded)
    {
        SendOperation send = Messaging.Send("Hello!");
        downloaded.CompleteWhen(send, OperationState.Completed);
    }

    protected override void OnError(OperationError e)
    {
        // by default, the handler will just log the error and retry
        // but the error and operation might be inspected to take a different action
        if (e.Operation.NumberOfRetries > 100 || e.Operation.Age > TimeSpan.FromMinutes(30))
        {
            e.Operation.Cancel(); // cancelled operation are moved to a built-in dead letter queue
        }
        else e.Handled(); // supresses error logging
    }
}
