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

    /// <summary>
    /// 创建参数错误异常
    /// </summary>
    /// <param name="message">错误消息</param>
    /// <returns>业务异常实例</returns>
    public static BusinessException BadRequest(string message)
    {
        return new BusinessException(message, 400);
    }

    /// <summary>
    /// 创建资源不存在异常
    /// </summary>
    /// <param name="message">错误消息</param>
    /// <returns>业务异常实例</returns>
    public static BusinessException NotFound(string message)
    {
        return new BusinessException(message, 404);
    }

    /// <summary>
    /// 创建未授权异常
    /// </summary>
    /// <param name="message">错误消息</param>
    /// <returns>业务异常实例</returns>
    public static BusinessException Unauthorized(string message)
    {
        return new BusinessException(message, 401);
    }

    /// <summary>
    /// 创建无权限异常
    /// </summary>
    /// <param name="message">错误消息</param>
    /// <returns>业务异常实例</returns>
    public static BusinessException Forbidden(string message)
    {
        return new BusinessException(message, 403);
    }
}
