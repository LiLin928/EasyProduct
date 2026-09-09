using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Mall.MemberLevel;
using EasyProduct.Models.Entitys.Mall;

namespace EasyProduct.Business.Mall;

/// <summary>
/// 会员等级服务接口
/// </summary>
/// <remarks>
/// 提供会员等级的增删改查、等级计算等功能
/// </remarks>
public interface IMemberLevelService
{
    /// <summary>
    /// 获取会员等级分页列表
    /// </summary>
    /// <param name="query">查询参数，包含关键词、状态、分页信息</param>
    /// <returns>会员等级分页结果</returns>
    Task<PageResponse<MemberLevelDto>> GetListAsync(MemberLevelQuery query);

    /// <summary>
    /// 获取所有启用的会员等级
    /// </summary>
    /// <returns>启用的会员等级列表</returns>
    /// <remarks>
    /// 用于下拉选择、等级计算等场景，只返回启用状态的等级
    /// </remarks>
    Task<List<MemberLevelDto>> GetAllAsync();

    /// <summary>
    /// 获取会员等级详情
    /// </summary>
    /// <param name="id">会员等级ID</param>
    /// <returns>会员等级详情</returns>
    Task<MemberLevelDto> GetByIdAsync(Guid id);

    /// <summary>
    /// 创建会员等级
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新创建的会员等级ID</returns>
    /// <remarks>
    /// 创建前会校验等级编码和等级数值的唯一性
    /// </remarks>
    Task<Guid> CreateAsync(MemberLevelCreateDto dto);

    /// <summary>
    /// 更新会员等级
    /// </summary>
    /// <param name="id">会员等级ID</param>
    /// <param name="dto">更新参数</param>
    /// <returns>更新是否成功</returns>
    /// <remarks>
    /// 更新前会校验等级编码和等级数值的唯一性（排除自身）
    /// </remarks>
    Task<bool> UpdateAsync(Guid id, MemberLevelUpdateDto dto);

    /// <summary>
    /// 删除会员等级
    /// </summary>
    /// <param name="id">会员等级ID</param>
    /// <returns>删除是否成功</returns>
    /// <remarks>
    /// 执行软删除，删除前会检查是否有会员使用此等级
    /// </remarks>
    Task<bool> DeleteAsync(Guid id);

    /// <summary>
    /// 根据积分计算会员等级
    /// </summary>
    /// <param name="points">会员积分</param>
    /// <returns>会员等级实体，未找到返回 null</returns>
    /// <remarks>
    /// 根据积分范围（MinPoints ≤ points ≤ MaxPoints 或 MaxPoints=0）计算对应等级
    /// 返回等级数值最高的匹配等级
    /// </remarks>
    Task<MemberLevel?> CalculateLevelByPointsAsync(int points);
}