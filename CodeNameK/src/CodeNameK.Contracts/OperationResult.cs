namespace CodeNameK.Contracts;
public record class OperationResult<T>
{
    public T? Entity { get; init; }
    public bool IsSuccess { get; init; } = false;
    public string Reason { get; init; } = string.Empty;

    public static implicit operator T?(OperationResult<T> item)
    {
        return item.Entity;
    }
}