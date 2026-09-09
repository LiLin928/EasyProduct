using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Mall.PointsRecord;

namespace EasyProduct.Business.Mall;

/// <summary>
/// 积分记录服务接口
/// </summary>
/// <remarks>
/// 提供积分记录的查询、积分变动、余额查询等功能
/// 积分变动操作必须在事务中执行，确保数据一致性
/// </remarks>
public interface IPointsRecordService
{
    /// <summary>
    /// 获取积分记录分页列表
    /// </summary>
    /// <param name="query">查询参数，包含会员ID、积分类型、时间范围、分页信息</param>
    /// <returns>积分记录分页结果</returns>
    /// <remarks>
    /// 支持按会员ID、积分类型、时间范围筛选
    /// 返回结果包含会员昵称和积分类型名称，方便前端展示
    /// </remarks>
    Task<PageResponse<PointsRecordDto>> GetListAsync(PointsRecordQuery query);

    /// <summary>
    /// 积分变动
    /// </summary>
    /// <param name="dto">积分变动参数，包含会员ID、积分类型、变动数量、备注</param>
    /// <returns>变动后的积分余额</returns>
    /// <remarks>
    /// 此操作涉及资金流水，必须在事务中执行
    /// 正数为增加积分，负数为减少积分
    /// 减少积分时，会检查余额是否充足
    /// 变动后会自动更新会员的当前积分和累计积分
    /// 变动后会自动更新会员等级
    /// </remarks>
    Task<int> ChangePointsAsync(PointsChangeDto dto);

    /// <summary>
    /// 获取会员当前积分余额
    /// </summary>
    /// <param name="memberId">会员ID</param>
    /// <returns>当前积分余额</returns>
    /// <remarks>
    /// 从会员主表读取当前积分，不包含已使用的积分
    /// </remarks>
    Task<int> GetMemberPointsBalanceAsync(Guid memberId);
}