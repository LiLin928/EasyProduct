using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Report.ColumnTemplate;

namespace EasyProduct.Business.Report;

/// <summary>
/// 报表列模板服务接口
/// </summary>
public interface IRptColumnTemplateService
{
    /// <summary>
    /// 获取列模板分页列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>列模板分页列表</returns>
    Task<PageResponse<RptColumnTemplateDto>> GetListAsync(RptColumnTemplateQuery query);

    /// <summary>
    /// 获取列模板详情
    /// </summary>
    /// <param name="id">列模板ID</param>
    /// <returns>列模板详情</returns>
    Task<RptColumnTemplateDto> GetByIdAsync(Guid id);

    /// <summary>
    /// 创建列模板
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新列模板ID</returns>
    Task<Guid> CreateAsync(RptColumnTemplateCreateDto dto);

    /// <summary>
    /// 更新列模板
    /// </summary>
    /// <param name="id">列模板ID</param>
    /// <param name="dto">更新参数</param>
    /// <returns>是否成功</returns>
    Task<bool> UpdateAsync(Guid id, RptColumnTemplateUpdateDto dto);

    /// <summary>
    /// 删除列模板
    /// </summary>
    /// <param name="id">列模板ID</param>
    /// <returns>是否成功</returns>
    Task<bool> DeleteAsync(Guid id);

    /// <summary>
    /// 获取所有列模板列表（不分页）
    /// </summary>
    /// <returns>列模板列表</returns>
    Task<List<RptColumnTemplateDto>> GetAllAsync();
}