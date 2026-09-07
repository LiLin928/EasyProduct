namespace EasyProduct.Common.Base;

/// <summary>
/// 统一 API 响应格式，用于封装所有 API 接口的返回结果
/// </summary>
/// <typeparam name="T">响应数据的类型</typeparam>
public class ApiResponse<T>
{
    /// <summary>
    /// 状态码：200 表示成功，其他值表示失败
    /// </summary>
    public int Code { get; set; }

    /// <summary>
    /// 提示信息，用于向用户显示操作结果说明
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// 业务数据，包含实际的响应内容
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// 时间戳（毫秒），记录响应生成的时间
    /// </summary>
    public long Timestamp { get; set; }

    /// <summary>
    /// 返回成功响应
    /// </summary>
    public static ApiResponse<T> Success(T? data, string message = "操作成功")
    {
        return new ApiResponse<T>
        {
            Code = 200,
            Message = message,
            Data = data,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        };
    }

    /// <summary>
    /// 返回失败响应
    /// </summary>
    public static ApiResponse<T> Error(string message, int code = 500)
    {
        return new ApiResponse<T>
        {
            Code = code,
            Message = message,
            Data = default,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        };
    }

    /// <summary>
    /// 返回无数据的成功响应
    /// </summary>
    public static ApiResponse<T> Success(string message = "操作成功")
    {
        return Success(default, message);
    }
}
