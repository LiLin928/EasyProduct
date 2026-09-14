using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Report.Definition;

namespace EasyProduct.Business.Report;

/// <summary>
/// 报表定义服务接口
/// </summary>
public interface IRptDefinitionService
{
    /// <summary>
    /// 获取报表定义分页列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>报表定义分页列表</returns>
    Task<PageResponse<RptDefinitionDto>> GetListAsync(RptDefinitionQuery query);

    /// <summary>
    /// 获取报表定义详情
    /// </summary>
    /// <param name="id">报表定义ID</param>
    /// <returns>报表定义详情</returns>
    Task<RptDefinitionDto> GetByIdAsync(Guid id);

    /// <summary>
    /// 创建报表定义
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新报表定义ID</returns>
    Task<Guid> CreateAsync(RptDefinitionCreateDto dto);

    /// <summary>
    /// 更新报表定义
    /// </summary>
    /// <param name="id">报表定义ID</param>
    /// <param name="dto">更新参数</param>
    /// <returns>是否成功</returns>
    Task<bool> UpdateAsync(Guid id, RptDefinitionUpdateDto dto);

    /// <summary>
    /// 删除报表定义
    /// </summary>
    /// <param name="id">报表定义ID</param>
    /// <returns>是否成功</returns>
    Task<bool> DeleteAsync(Guid id);

    /// <summary>
    /// 执行报表查询
    /// </summary>
    /// <param name="id">报表定义ID</param>
    /// <param name="pageIndex">页码</param>
    /// <param name="pageSize">每页条数</param>
    /// <returns>查询结果</returns>
    Task<RptExecuteResultDto> ExecuteAsync(Guid id, int pageIndex = 1, int pageSize = 100);

    /// <summary>
    /// 导出报表数据到 Excel
    /// </summary>
    /// <param name="id">报表定义ID</param>
    /// <returns>Excel 文件字节数组</returns>
    Task<byte[]> ExportToExcelAsync(Guid id);

    /// <summary>
    /// 发布报表
    /// </summary>
    /// <param name="id">报表定义ID</param>
    /// <returns>是否成功</returns>
    Task<bool> PublishAsync(Guid id);

    /// <summary>
    /// 归档报表
    /// </summary>
    /// <param name="id">报表定义ID</param>
    /// <returns>是否成功</returns>
    Task<bool> ArchiveAsync(Guid id);
}