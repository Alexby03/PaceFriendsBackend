namespace PaceFriendsBackend.Core.Models;

public class ServiceResult<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? ErrorMessage { get; set; }
    public int ErrorCode { get; set; }

    public static ServiceResult<T> Ok(T data) => new() { Success = true, Data = data };
    public static ServiceResult<T> Fail(string message, int code = 400) => new() { Success = false, ErrorMessage = message, ErrorCode = code };
}
