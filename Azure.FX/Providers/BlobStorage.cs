using Azure.FX.Core;
using Azure.Identity;
using Azure.Storage.Blobs;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;

namespace Azure.FX
{
    public class BlobStorage : StorageServices
    {
        readonly BlobServiceClient _client;

        public BlobStorage(string connectionString)
            : this(new BlobServiceClient(connectionString)) { }

        public BlobStorage(Uri serviceUri)
            : this(new BlobServiceClient(serviceUri, new DefaultAzureCredential())) { }

        public BlobStorage(BlobServiceClient serviceClient)
            => _client = serviceClient;

        public event Action<DownloadOperation> Downloaded;
        public event Action<UploadOperation> Uploaded;
        public event Action<StorageProcessorError> Error;

        public UploadOperation UploadDirectory(string sourceDirectory, string destinationContainer)
            => throw new NotImplementedException();

        public BlobServiceClient Client => _client;

        public override UploadOperation Upload(BinaryData data, string destinationContainer, string destinationBlob)
        {
            var operation = new UploadBlobOperation(destinationContainer, destinationBlob, data);
            operation.Run(this);
            OperationProcessor.Shared.ExecuteOperation(operation);
            return operation;
        }

        public DownloadOperation DownloadToDirectory(string sourceContainer, string destinationDirectory)
            => throw new NotImplementedException();

        public BinaryData Download(string sourceContainer, string sourceBlob)
            => throw new NotImplementedException();

        public override DownloadOperation DownloadToFile(string sourceContainer, string sourceBlob, string destinationPath)
            => throw new NotImplementedException();

        public override DownloadOperation DownloadToStream(string sourceContainer, string sourceBlob, Stream destinationStream, bool closeStreamWhenDone = true)
            => throw new NotImplementedException();

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object obj) => base.Equals(obj);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode() => base.GetHashCode();

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string ToString() => base.ToString();

    }

    public class DownloadBlobOperation : DownloadOperation
    {
        public int ConcurrencyLevel { get; set; }

        public DownloadBlobOperation(string sourceContainer, string sourceBlob) : base(sourceContainer, sourceBlob)
        {}

        protected internal override Task DoAsync()
            => throw new NotImplementedException();

        public override void Complete()
            => throw new NotImplementedException();

        public override void CompleteWhen(ProcessorOperation operation, OperationState state)
            => throw new NotImplementedException();

        public void Run(BlobStorage storage)
            => throw new NotImplementedException();
    }

    public class UploadBlobOperation : UploadOperation
    {
        BinaryData _data;
        public UploadBlobOperation(string destinationContainer, string destinationBlob, BinaryData data)
            : base(destinationContainer, destinationBlob)
        {
            _data = data;
        }

        protected internal override Task DoAsync() 
            => throw new NotImplementedException();

        public override void Complete() 
            => throw new NotImplementedException();

        public override void CompleteWhen(ProcessorOperation operation, OperationState state)
            => throw new NotImplementedException();

        public void Run(BlobStorage storage)
            => throw new NotImplementedException();
    }

    public class StorageProcessorError
    {
        public UploadOperation Operation { get; }
    }
}
