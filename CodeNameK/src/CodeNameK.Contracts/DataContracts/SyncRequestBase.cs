namespace CodeNameK.Contracts;

public abstract record SyncRequestBase<T>
{
    public abstract T Payload { get; }
}