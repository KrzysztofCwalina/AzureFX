using Azure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace Azure.FX.Core
{
    public abstract class ProcessorOperation // TODO: should this inherit from Azure.Operation?
    {
        protected CancellationTokenSource _ctSource = new CancellationTokenSource();
        protected int _numberOfTries;
        internal Task _task;
        public bool IsCompleted { get; protected set; }
        public DateTime CreatedTime { get; internal set; }
        public int NumberOfRetries => _numberOfTries < 2 ? 0 : _numberOfTries - 1;
        public object Tag { get; internal set; }
        public TimeSpan Age { get; set; }

        public async Task WaitForCompletionAsync() => await _task;
        
        public bool WaitForCompletion(CancellationToken cancellationToken = default) { 
            while(!IsCompleted)
            {
                Thread.Sleep(50);
                if (cancellationToken.IsCancellationRequested) return false;
            }
            return true;
        }

        public virtual void Complete() { } // override if anything needs to be commited

        public virtual void CompleteWhen(ProcessorOperation operation, OperationState state) { } // override if Complete is no a noop

        protected internal abstract Task DoAsync();

        protected ProcessorOperation()
        {
            CreatedTime = DateTime.UtcNow;
        }
        public void Cancel()
        {
            _ctSource.Cancel();
            OnCancelled();
        }

        protected internal void OnCompleted()
        {
            IsCompleted = true;
            Completed?.Invoke();
        }
        protected void OnCancelled() => Cancelled?.Invoke();
        public event Action Completed;
        public event Action Cancelled;
    }

    public class OperationProcessor
    {
        private OperationProcessor() { }
        public static OperationProcessor Shared { get; } = new OperationProcessor();
        public event Action<ProcessorOperation> Completed;

        List<ProcessorOperation> _operations = new List<ProcessorOperation>(); // TODO: this is a really bad datastructure for this scenario

        public void ExecuteOperation(ProcessorOperation operation)
        {
            lock (_operations) _operations.Add(operation);
            var task = operation.DoAsync();
            if (task.IsCompleted) OnCompleted(task, operation);
            else task.ContinueWith(OnCompleted, operation);
            operation._task = task;
        }

        private void OnCompleted(Task task, object operation)
        {
            var op = operation as ProcessorOperation;
            op._task = null;
            op.OnCompleted();
            Completed?.Invoke(op);
            lock(_operations) _operations.Remove(op);
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

        public static bool WaitForCompletion(ProcessorOperation[] operations, CancellationToken cancellationToken)
        {
            foreach(var operation in operations)
            {
                if (!operation.WaitForCompletion(cancellationToken))
                {
                    return false;
                }
            }
            return true;
        }
        public static bool WaitForCompletion(params ProcessorOperation[] operations)
        {
            foreach (var operation in operations)
            {
                if (!operation.WaitForCompletion())
                {
                    return false;
                }
            }
            return true;
        }
    }
}

namespace Azure
{
    public enum OperationState
    {
        Failed,
        Completed,
        InProgress,
    }
}
