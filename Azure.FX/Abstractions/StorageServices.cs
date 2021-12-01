using Azure.FX.Core;
using System;
using System.IO;

namespace Azure.FX
{
    public abstract class StorageServices
    {
        public Action<DownloadOperation> BlobCreated { get; set; }

        public abstract UploadOperation Upload(BinaryData data, string containerName, string blobName);

        public abstract DownloadOperation DownloadToFile(string sourceContainer, string sourceBlob, string destinationPath);

        public abstract DownloadOperation DownloadToStream(string sourceContainer, string sourceBlob, Stream destinationStream, bool closeStreamWhenDone = true);
    }

    public abstract class BlobOperation : ProcessorOperation
    {
        public string Blob { get; protected set; }
        public string Container { get; protected set; }

        protected BlobOperation(string container, string blob)
        {
            Container = container;
            Blob = blob;
        }
    }

    public abstract class DownloadOperation : BlobOperation
    {
        public BinaryData Data { get; protected set; }

        protected DownloadOperation(string sourceContainer, string sourceBlob)
            : base(sourceContainer, sourceBlob) {}
    }

    public abstract class UploadOperation : BlobOperation
    {
        protected UploadOperation(string destinationContainer, string destinationBlob)
            : base(destinationContainer, destinationBlob) {}
    }
}
