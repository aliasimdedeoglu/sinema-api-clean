namespace CinemaSystem.Application.Common.Models;
public sealed class Result<T>
{
    private Result(T value) { Value = value; IsSuccess = true; }
    private Result(string error, int statusCode) { Error = error; StatusCode = statusCode; IsSuccess = false; }
    public T? Value { get; }
    public string? Error { get; }
    public int StatusCode { get; }
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public static Result<T> Success(T value) => new(value);
    public static Result<T> Failure(string error, int statusCode = 400) => new(error, statusCode);
    public static Result<T> NotFound(string error) => new(error, 404);
    public static Result<T> Conflict(string error) => new(error, 409);
}