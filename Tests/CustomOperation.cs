using Azure.FX.Core;
using NUnit.Framework;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

public class OperationTests
{
    class AddOperation : ProcessorOperation
    {
        internal int _x;
        internal int _y;
        public long Result { get; private set; }

        public AddOperation(int x, int y)
        {
            _x = x;
            _y = y;
        }

        Random rand = new Random(100);
        protected async override Task DoAsync()
        {
            await Task.Delay(rand.Next(1000));
            Result = _x + _y;
            OnCompleted();
        }
    }

    [Test]
    public void Add()
    {
        var operation = new AddOperation(5, 2);
        OperationProcessor.Shared.ExecuteOperation(operation);
        operation.WaitForCompletion();
        Assert.AreEqual(7, operation.Result);
    }

    [Test]
    public void ManyAdds()
    {
        OperationProcessor.Shared.Completed += (operation) =>
        {
            var add = operation as AddOperation;
            Assert.AreEqual(add._x + add._y, add.Result);
            Debug.WriteLine(add.Result);
        };

        var op1 = new AddOperation(5, 1);
        var op2 = new AddOperation(5, 2);
        var op3 = new AddOperation(5, 3);
        OperationProcessor.Shared.ExecuteOperation(op1);
        OperationProcessor.Shared.ExecuteOperation(op2);
        OperationProcessor.Shared.ExecuteOperation(op3);

        OperationProcessor.WaitForCompletion(op1, op2, op3);
    }
}
