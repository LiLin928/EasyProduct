using EasyProduct.Common.Base;
using EasyProduct.Common.Error;
using EasyProduct.Models.Dto.Report.ColumnTemplate;
using EasyProduct.Models.Entitys.Report;
using Mapster;
using Microsoft.Extensions.Logging;
using SqlSugar;

namespace EasyProduct.Business.Report;

/// <summary>
/// 报表列模板服务实现
/// </summary>
public class RptColumnTemplateService : BaseService, IRptColumnTemplateService
{
    private readonly ILogger<RptColumnTemplateService> _logger;

    public RptColumnTemplateService(ILogger<RptColumnTemplateService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// 获取列模板分页列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>列模板分页列表</returns>
    /// <remarks>
    /// 1. 支持按模板名称模糊搜索
    /// 2. 支持按字段类型筛选
    /// 3. 默认按创建时间倒序排列
    /// </remarks>
    public async Task<PageResponse<RptColumnTemplateDto>> GetListAsync(RptColumnTemplateQuery query)
    {
        // 1. 构建查询条件
        var whereExpr = Expressionable.Create<RptColumnTemplate>()
            .AndIF(!string.IsNullOrEmpty(query.Name), x => x.Name.Contains(query.Name!))
            .AndIF(!string.IsNullOrEmpty(query.Type), x => x.Type == query.Type)
            .ToExpression();

        // 2. 分页查询
        var queryable = _db.Queryable<RptColumnTemplate>()
            .Where(whereExpr)
            .OrderByDescending(x => x.CreatedAt);

        RefAsync<int> total = 0;
        var list = await queryable
            .ToPageListAsync(query.PageIndex, query.PageSize, total);

        // 3. 转换为 DTO
        var dtos = list.Adapt<List<RptColumnTemplateDto>>();
        for (int i = 0; i < dtos.Count; i++)
        {
            dtos[i].CreateTime = list[i].CreatedAt;
            dtos[i].UpdateTime = list[i].UpdatedAt ?? DateTime.MinValue;
        }

        // 4. 返回分页结果
        return PageResponse<RptColumnTemplateDto>.Create(dtos, total.Value, query.PageIndex, query.PageSize);
    }

    /// <summary>
    /// 获取列模板详情
    /// </summary>
    /// <param name="id">列模板ID</param>
    /// <returns>列模板详情</returns>
    public async Task<RptColumnTemplateDto> GetByIdAsync(Guid id)
    {
        var entity = await _db.Queryable<RptColumnTemplate>()
            .FirstAsync(x => x.Id == id);

        if (entity == null)
        {
            throw BusinessException.NotFound("列模板不存在");
        }

        return entity.Adapt<RptColumnTemplateDto>();
    }

    /// <summary>
    /// 创建列模板
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新列模板ID</returns>
    public async Task<Guid> CreateAsync(RptColumnTemplateCreateDto dto)
    {
        var entity = dto.Adapt<RptColumnTemplate>();
        entity.Sortable = dto.Sortable ? 1 : 0;

        await _db.Insertable(entity).ExecuteCommandAsync();
        return entity.Id;
    }

    /// <summary>
    /// 更新列模板
    /// </summary>
    /// <param name="id">列模板ID</param>
    /// <param name="dto">更新参数</param>
    /// <returns>是否成功</returns>
    public async Task<bool> UpdateAsync(Guid id, RptColumnTemplateUpdateDto dto)
    {
        var entity = await _db.Queryable<RptColumnTemplate>()
            .FirstAsync(x => x.Id == id);

        if (entity == null)
        {
            throw BusinessException.NotFound("列模板不存在");
        }

        entity.Name = dto.Name ?? entity.Name;
        entity.Field = dto.Field ?? entity.Field;
        entity.Type = dto.Type ?? entity.Type;
        entity.Width = dto.Width ?? entity.Width;
        entity.Format = dto.Format;
        entity.Sortable = dto.Sortable.HasValue ? (dto.Sortable.Value ? 1 : 0) : entity.Sortable;
        entity.Remark = dto.Remark;

        await _db.Updateable(entity).ExecuteCommandAsync();
        return true;
    }

    /// <summary>
    /// 删除列模板
    /// </summary>
    /// <param name="id">列模板ID</param>
    /// <returns>是否成功</returns>
    public async Task<bool> DeleteAsync(Guid id)
    {
        await _db.Deleteable<RptColumnTemplate>()
            .Where(x => x.Id == id)
            .ExecuteCommandAsync();

        return true;
    }

    /// <summary>
    /// 获取所有列模板列表（不分页）
    /// </summary>
    /// <returns>列模板列表</returns>
    public async Task<List<RptColumnTemplateDto>> GetAllAsync()
    {
        var list = await _db.Queryable<RptColumnTemplate>()
            .OrderBy(x => x.Name)
            .ToListAsync();

        return list.Adapt<List<RptColumnTemplateDto>>();
    }
}