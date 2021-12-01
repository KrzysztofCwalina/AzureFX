using Azure;
using Azure.Core;
using Azure.FX.Core;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

public class GenericOperationTests
{
    class MyOperation : ProcessorOperation
    {
        public override void Complete()
            => throw new NotImplementedException();

        public override void CompleteWhen(ProcessorOperation operation, OperationState state)
            => throw new NotImplementedException();

        protected override Task DoAsync()
        {
            Console.WriteLine("Hello World!");
            return Task.CompletedTask;
        }
    }

    [Test]
    public void HelloWorld()
    {
        OperationProcessor.Shared.ExecuteOperation(new MyOperation());
    }
}
