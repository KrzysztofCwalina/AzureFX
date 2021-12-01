using Azure.Core;
using Azure.FX.Core;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Azure.FX
{
    public abstract class StorageServices
    {
        public Action<DownloadOperation> BlobCreated { get; set; }

        public abstract UploadOperation Upload(BinaryData data, string containerName, string blobName);

        public abstract DownloadOperation DownloadToFile(string sourceContainer, string sourceBlob, string destinationPath);

        public abstract DownloadOperation DownloadToStream(string sourceContainer, string sourceBlob, Stream destinationStream, bool closeStreamWhenDone = true);
    }

    public abstract class DownloadOperation : ProcessorOperation
    {
        public string Blob { get; protected set; }
        public string Container { get; protected set; }
        public BinaryData Data { get; protected set; }

        protected DownloadOperation(string container, string blob)
        {
            Container = container;
            Blob = blob;
        }
    }

    public abstract class UploadOperation : ProcessorOperation
    {
        public string Blob { get; protected set; }
        public string Container { get; protected set; }
        protected BinaryData Data { get; set; }
    }
}
