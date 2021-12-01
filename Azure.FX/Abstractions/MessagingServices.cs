using Azure.FX.Core;
using System;

namespace Azure.FX
{
    public abstract class MessagingServices
    {
        public Action<ReceiveOperation> MessageReceived { get; set; }

        public abstract SendOperation Send(object message);
    }

    public abstract class ReceiveOperation : ProcessorOperation
    {
        public BinaryData Data { get; internal set; }
    }

    public abstract class SendOperation : ProcessorOperation
    {
        public BinaryData Data { get; internal set; }
    }
}
