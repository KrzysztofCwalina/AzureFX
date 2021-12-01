using Azure;
using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace Azure
{
    public enum OperationState
    {
        Failed,
        Completed,
        InProgress,
    }
}

namespace Azure.FX.Core
{
    public abstract class ProcessorOperation // TODO: should this inherit from Azure.Operation?
    {
        protected CancellationTokenSource _ctSource = new CancellationTokenSource();
        protected int _numberOfTries;
        internal Task _task;

        public DateTime CreatedTime { get; internal set; }
        public int NumberOfRetries => _numberOfTries < 2 ? 0 : _numberOfTries - 1;
        public object Tag { get; internal set; }
        public TimeSpan Age { get; set; }

        public Task WaitForCompletionAsync(CancellationToken cancellationToken = default) { throw new NotImplementedException(); }
        public void WaitForCompletion(CancellationToken cancellationToken = default) { throw new NotImplementedException(); }

        public abstract void Complete();

        public abstract void CompleteWhen(ProcessorOperation operation, OperationState state);

        protected internal abstract Task DoAsync();

        protected ProcessorOperation()
        {
            CreatedTime = DateTime.UtcNow;
        }
        public void Cancel()
        {
            _ctSource.Cancel();
            Cancelled.Invoke();
        }

        public event Action Completed;
        public event Action Cancelled;
    }

    public class OperationProcessor
    {
        private OperationProcessor() { }
        public static OperationProcessor Shared { get; } = new OperationProcessor();

        ConcurrentQueue<ProcessorOperation> _operations = new ConcurrentQueue<ProcessorOperation>();

        public void ExecuteOperation(ProcessorOperation operation)
        {
            _operations.Enqueue(operation);
            var task = operation.DoAsync();
            task.ContinueWith(Completed);

            operation._task = task;
        }

        private void Completed(Task tak)
        {

        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object obj) => base.Equals(obj);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode() => base.GetHashCode();

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string ToString() => base.ToString();

        public void WaitForDone(CancellationToken cancellationToken = default)
        {
            while (_operations.Count != 0 && !cancellationToken.IsCancellationRequested) Thread.Sleep(50);
        }

        public static void WaitForCompletion(params ProcessorOperation[] operations)
        {
            throw new NotImplementedException();
        }
    }
}
