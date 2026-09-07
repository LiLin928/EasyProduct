namespace EasyProduct.Common.Error;

/// <summary>
/// 业务异常，用于封装业务逻辑错误
/// </summary>
/// <remarks>
/// 业务异常不会被记录为系统错误，而是返回给前端显示。
/// 适用于参数校验失败、业务规则不满足等场景。
/// </remarks>
public class BusinessException : Exception
{
    /// <summary>
    /// 错误码
    /// </summary>
    public int Code { get; }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="message">错误消息</param>
    /// <param name="code">错误码，默认为 400</param>
    public BusinessException(string message, int code = 400) : base(message)
    {
        Code = code;
    }
}
