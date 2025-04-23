namespace CleanArchitecture.Application.Common.Models;

public class Result
{
    internal Result(bool succeeded, IEnumerable<string> errors)
    {
        Succeeded = succeeded;
        Errors = errors.ToArray();
    }

    internal Result(bool succeeded, IEnumerable<string> errors, object data)
    {
        Succeeded = succeeded;
        Errors = errors.ToArray();
        Data = data;
    }

    public bool Succeeded { get; init; }

    public string[] Errors { get; init; }

    public object? Data { get; init; }

    public static Result Success()
    {
        return new Result(true, Array.Empty<string>());
    }

    public static Result Success(object data)
    {
        return new Result(true, Array.Empty<string>(), data);
    }

    public static Result Failure(IEnumerable<string> errors)
    {
        return new Result(false, errors);
    }

    public static Result Failure(string error) => new Result(false, new[] { error });
}
