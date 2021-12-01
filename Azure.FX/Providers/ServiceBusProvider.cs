using Azure.Core;
using System;
using System.ComponentModel;
using Azure.Messaging.ServiceBus;
using System.Threading.Tasks;
using Azure.FX.Providers;
using Azure.FX.Core;

namespace Azure.FX.Providers
{
    public class ServiceBusProvider : MessagingServices
    {
        ServiceBusClient _client;
        internal ServiceBusSender _sender;

        public ServiceBusProvider(string connectionString, string queue)
        {
            _client = new ServiceBusClient(connectionString);
            _sender = _client.CreateSender(queue);
        }
        public event Action<SendOperation> Process;

        public override SendOperation Send(object message)
        {
            var operation = new SbSendOperation(this);
            operation.Data = BinaryData.FromObjectAsJson(message);
            OperationProcessor.Shared.ExecuteOperation(operation);
            return operation;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object obj) => base.Equals(obj);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode() => base.GetHashCode();

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string ToString() => base.ToString();
    }

    internal class SbSendOperation : SendOperation
    {
        ServiceBusProvider _service;
        public SbSendOperation(ServiceBusProvider processor)
        {
            _service = processor;
        }
        public BinaryData Data { get; internal set; }
        public override void Complete()
        {
            throw new NotImplementedException();
        }

        public override void CompleteWhen(ProcessorOperation operation, OperationState state)
        {
            throw new NotImplementedException();
        }

        protected internal override async Task DoAsync()
        {
            ServiceBusMessage sbm = new ServiceBusMessage();
            await _service._sender.SendMessageAsync(sbm);
        }
    }
}
