using System;
using System.Collections.Generic;
using System.Threading;

public enum ExecutionType
{
    Post,
    Send
}

public struct SendOrPostCallbackItem
{
    object _state;
    private ExecutionType _exeType;
    SendOrPostCallback _method;

    public SendOrPostCallbackItem(SendOrPostCallback callback,
        object state, ExecutionType type)
    {
        _method = callback;
        _state = state;
        _exeType = type;
    }

    public void Execute()
    {
        if (_method == ExecutionType.Send)
            Send();
        else
            Post();
    }

    public void Send()
    {
        _method(mState);
    }

    public void Post()
    {
        _method(mState);
    }
}

public class UnitySynchronizationContext : SynchronizationContext, IDisposable
{
    public Queue<SendOrPostCallbackItem> Queue1;
    public Queue<SendOrPostCallbackItem> Queue2;
    public UnitySynchronizationContext()
        : base()
    {
        Queue1 = new Queue<SendOrPostCallbackItem>();
        Queue2 = new Queue<SendOrPostCallbackItem>();
    }

    public override void Send(SendOrPostCallback d, object state)
    {
        SendOrPostCallbackItem item = new SendOrPostCallbackItem(d, state,
                                          ExecutionType.Send);
        Queue2.Enqueue(item);
    }

    public override void Post(SendOrPostCallback d, object state)
    {
        SendOrPostCallbackItem item = new SendOrPostCallbackItem(d, state,
                                          ExecutionType.Post);
        Queue2.Enqueue(item);
    }

    public void Run()
    {
        while (Queue1.Count > 0)
        {
            SendOrPostCallbackItem workItem = Queue1.Dequeue();
            workItem.Execute();
        }
        var tmpQ = Queue1;
        Queue1 = Queue2;
        Queue2 = tmpQ;

    }

    public void Dispose()
    {
    }

    public override SynchronizationContext CreateCopy()
    {
        return this;
    }
}
