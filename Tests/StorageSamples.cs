using Azure;
using Azure.FX;
using Azure.FX.Core;
using NUnit.Framework;
using System;
using System.IO;

public class StorageSamples
{
    [Test]
    public void HelloWorld()
    {
        var client = new BlobStorage(connectionString: "");

        // This method is similar to azcopy,
        // i.e. almost never throws and provides bulk data transfer,
        // which corresponds more closely to what users are trying to acomplish.
        client.UploadDirectory(".\\data", "test_Container");
    }

    [Test]
    public void ErrorBasics()
    {
        // Azure.FX operations try very hard to acomplish their tasks, e.g. upload a set of files.
        // If an app tries to upload a file, it does not make sense to throw if the network is temporary down.
        // In most cases, the app has no choice but wait for the network to come up again.
        // This can be seen as turning exception reporting on it's head: from eager to conservative.

        // But of course, the app probably wants to know when the network is down,
        // and it can cancel the operation if it's tired of waiting.
        client.Error += (StorageProcessorError e) =>
        {
            if (e.Operation.NumberOfRetries > 100 || e.Operation.Age > TimeSpan.FromMinutes(10))
            {
                e.Operation.Cancel(); // this will automatically log.
            }
        };

        client.UploadDirectory(".\\data", "test_Container");
    }

    [Test]
    public void CommonOperations()
    {
        // BlobStorage has mainline scenario APIs for uploading and downloading files, folders. streams.  
        // It's similar in complexity and structure to .NET file I/O APIs.
        // Users familiar with .NET file I/O will feel at home with these APIs.
        client.Upload(BinaryData.FromString("Hello World!"), "test_container", "test_blob1");
        client.UploadDirectory(".\\data", "test_Container");

        client.DownloadToFile("test_container", "test_blob1", destinationPath: ".\\blob1.bin");
        client.DownloadToDirectory("test_container", ".\\destination");
    }

    [Test]
    public void ExecutionModel()
    {
        // Operations are also all synchronous, yet they don't block threads.
        // They simply queue workitems that will be happening in the background.
        // It's very similar to how LROs work.

        // Operation status changes can be observed
        client.Uploaded += (operation) => Console.WriteLine($"{operation.Blob} uploaded");
        client.Downloaded += (operation) => Console.WriteLine($"{operation.Blob}: {operation.Data}");

        UploadOperation upload = client.UploadDirectory(".\\source", "test_container1");
        DownloadOperation downloadFolder = client.DownloadToDirectory("test_container2", ".\\destination");
        DownloadOperation downloadFile = client.DownloadToFile("test_container1", "test_blob1", destinationPath: ".\\blob1.bin");

        // Operations can be waited on
        upload.WaitForCompletion(); // this will block the thread.
        OperationProcessor.WaitForCompletion(downloadFolder, downloadFile);
    }

    [Test]
    public void DownloadAdvanced()
    {
        // BlobStorage ("client") will expose a smal subset of the blob storage APIs.
        // More knobs can be exposed on operations, i.e. no need to polute the "client" APIs.
        // Having said that, even operations won't expose the full surface area of the service.
        // If the user wants to access more advanced features, they can use low-level Azure SDK APIs.
        var operation = new DownloadBlobOperation("test_container", "test_blob1");
        operation.ConcurrencyLevel = 6;
        operation.Run(client);

        client.DownloadToFile("test_container", "test_blob1", destinationPath: ".\\blob1.bin");

        using var stream = new MemoryStream();
        client.DownloadToStream("test_container", "test_blob1", stream);
    }

    BlobStorage client = new BlobStorage(connectionString: "");
}
