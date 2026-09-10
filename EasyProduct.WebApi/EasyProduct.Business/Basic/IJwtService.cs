namespace EasyProduct.Business.Basic;

/// <summary>
/// JWT 服务接口
/// </summary>
/// <remarks>
/// 提供会员 JWT 生成功能。
/// </remarks>
public interface IJwtService
{
    /// <summary>
    /// 生成会员 Token
    /// </summary>
    /// <param name="memberId">会员 ID</param>
    /// <param name="openid">微信 openid</param>
    /// <returns>JWT Token</returns>
    string GenerateMemberToken(string memberId, string openid);
}