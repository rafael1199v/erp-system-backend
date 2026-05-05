namespace Erp.Inventory.Application.Result;

public class Result<T>
{
    public T? Value { get; set; }
    public bool IsSuccess { get; set; }
    public string? Error { get; set; }

    public Result(T? value, bool isSuccess, string? error)
    {
        Value = value;
        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result<T> Success(T value)
    {
        return new Result<T>(value, true, null);
    }

    public static Result<T> Failure(string error)
    {
        return new Result<T>(default, false, error);
    }
}