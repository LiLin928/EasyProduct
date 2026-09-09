using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Mall.Member;

namespace EasyProduct.Business.Mall;

/// <summary>
/// 会员服务接口
/// </summary>
/// <remarks>
/// 提供会员的增删改查、微信登录、等级更新等功能
/// </remarks>
public interface IMemberService
{
    /// <summary>
    /// 获取会员分页列表
    /// </summary>
    /// <param name="query">查询参数，包含关键词、会员等级、状态、分页信息</param>
    /// <returns>会员分页结果</returns>
    Task<PageResponse<MemberDto>> GetListAsync(MemberQuery query);

    /// <summary>
    /// 获取会员详情
    /// </summary>
    /// <param name="id">会员ID</param>
    /// <returns>会员详情</returns>
    Task<MemberDto> GetByIdAsync(Guid id);

    /// <summary>
    /// 创建会员
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新创建的会员ID</returns>
    /// <remarks>
    /// 创建会员时会根据累计积分自动计算会员等级
    /// </remarks>
    Task<Guid> CreateAsync(MemberCreateDto dto);

    /// <summary>
    /// 更新会员
    /// </summary>
    /// <param name="id">会员ID</param>
    /// <param name="dto">更新参数</param>
    /// <returns>更新是否成功</returns>
    /// <remarks>
    /// 更新会员等级或累计积分时，会自动重新计算会员等级
    /// </remarks>
    Task<bool> UpdateAsync(Guid id, MemberUpdateDto dto);

    /// <summary>
    /// 删除会员
    /// </summary>
    /// <param name="id">会员ID</param>
    /// <returns>删除是否成功</returns>
    /// <remarks>
    /// 执行软删除，删除前会检查会员是否有未完成订单
    /// </remarks>
    Task<bool> DeleteAsync(Guid id);

    /// <summary>
    /// 微信登录
    /// </summary>
    /// <param name="dto">微信登录参数，包含 code、加密数据等</param>
    /// <returns>登录结果，包含 token、refreshToken、会员信息</returns>
    /// <remarks>
    /// 通过微信 code 换取 openid，实现自动注册或登录
    /// 返回的 token 用于后续 API 认证
    /// </remarks>
    Task<(string token, string refreshToken, MemberInfoDto member)> WechatLoginAsync(WechatLoginDto dto);

    /// <summary>
    /// 获取当前会员信息
    /// </summary>
    /// <param name="memberId">会员ID</param>
    /// <returns>会员信息，包含等级、积分、余额等</returns>
    /// <remarks>
    /// 用于小程序端展示会员中心信息
    /// </remarks>
    Task<MemberInfoDto> GetCurrentMemberInfoAsync(Guid memberId);

    /// <summary>
    /// 更新会员等级
    /// </summary>
    /// <param name="memberId">会员ID</param>
    /// <returns>更新是否成功</returns>
    /// <remarks>
    /// 根据会员累计积分重新计算并更新会员等级
    /// 通常在积分变更后调用
    /// </remarks>
    Task<bool> UpdateMemberLevelAsync(Guid memberId);
}