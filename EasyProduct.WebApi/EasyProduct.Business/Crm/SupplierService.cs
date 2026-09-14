using EasyProduct.Common.Base;
using EasyProduct.Common.Error;
using EasyProduct.Models.Dto.Crm;
using EasyProduct.Models.Entitys.Crm;
using EasyProduct.Models.Enums;
using Mapster;
using SqlSugar;

namespace EasyProduct.Business.Crm;

/// <summary>
/// 供应商服务实现
/// </summary>
/// <remarks>
/// 提供供应商的增删改查、资质管理等功能
/// </remarks>
public class SupplierService : BaseService, ISupplierService
{
    /// <summary>
    /// 获取供应商分页列表
    /// </summary>
    /// <param name="query">查询参数，包含关键词、状态、分页信息</param>
    /// <returns>供应商分页结果</returns>
    /// <remarks>
    /// 1. 支持按供应商名称、编码、联系人模糊搜索
    /// 2. 支持按状态筛选
    /// 3. 默认按创建时间倒序排列
    /// </remarks>
    public async Task<PageResponse<SupplierDto>> GetListAsync(SupplierQueryDto query)
    {
        var queryable = _db.Queryable<Supplier>()
            .Where(s => s.IsDeleted == 0);

        // 关键词搜索（供应商名称、编码、联系人）
        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            queryable = queryable.Where(s =>
                s.Name.Contains(query.Keyword) ||
                s.Code.Contains(query.Keyword) ||
                (s.ContactName != null && s.ContactName.Contains(query.Keyword)));
        }

        // 供应商名称搜索
        if (!string.IsNullOrWhiteSpace(query.Name))
        {
            queryable = queryable.Where(s => s.Name.Contains(query.Name));
        }

        // 供应商编码搜索
        if (!string.IsNullOrWhiteSpace(query.Code))
        {
            queryable = queryable.Where(s => s.Code.Contains(query.Code));
        }

        // 状态筛选
        if (query.Status.HasValue)
        {
            queryable = queryable.Where(s => s.Status == (Status)query.Status);
        }

        // 按创建时间倒序
        queryable = queryable.OrderBy(s => s.CreatedAt, OrderByType.Desc);

        // 分页查询
        RefAsync<int> total = 0;
        var result = await queryable
            .Select(s => new SupplierDto
            {
                Id = s.Id.ToString(),
                Code = s.Code,
                Name = s.Name,
                Status = (int)s.Status,
                StatusName = s.Status == Status.Enabled ? "启用" : "禁用",
                ContactName = s.ContactName,
                ContactPhone = s.ContactPhone,
                ContactEmail = s.ContactEmail,
                Address = s.Address,
                BankName = s.BankName,
                BankAccount = s.BankAccount,
                Remark = s.Remark,
                CreatedAt = s.CreatedAt
            })
            .ToPageListAsync(query.PageIndex, query.PageSize, total);

        return PageResponse<SupplierDto>.Create(result, total.Value, query.PageIndex, query.PageSize);
    }

    /// <summary>
    /// 获取供应商详情
    /// </summary>
    /// <param name="id">供应商ID</param>
    /// <returns>供应商详情</returns>
    public async Task<SupplierDto> GetByIdAsync(Guid id)
    {
        var supplier = await _db.Queryable<Supplier>()
            .Where(s => s.Id == id && s.IsDeleted == 0)
            .FirstAsync();

        if (supplier == null)
        {
            throw BusinessException.NotFound("供应商不存在");
        }

        return new SupplierDto
        {
            Id = supplier.Id.ToString(),
            Code = supplier.Code,
            Name = supplier.Name,
            Status = (int)supplier.Status,
            StatusName = supplier.Status == Status.Enabled ? "启用" : "禁用",
            ContactName = supplier.ContactName,
            ContactPhone = supplier.ContactPhone,
            ContactEmail = supplier.ContactEmail,
            Address = supplier.Address,
            BankName = supplier.BankName,
            BankAccount = supplier.BankAccount,
            Remark = supplier.Remark,
            CreatedAt = supplier.CreatedAt
        };
    }

    /// <summary>
    /// 创建供应商
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新创建的供应商ID</returns>
    /// <remarks>
    /// 创建供应商时会自动生成供应商编码（S + 年月 + 序号）
    /// </remarks>
    public async Task<Guid> CreateAsync(CreateSupplierDto dto)
    {
        // 生成供应商编码
        var code = await GenerateSupplierCodeAsync();

        var supplier = dto.Adapt<Supplier>();
        supplier.Id = Guid.NewGuid();
        supplier.Code = code;
        supplier.Status = Status.Enabled;

        await _db.Insertable(supplier).ExecuteCommandAsync();

        return supplier.Id;
    }

    /// <summary>
    /// 更新供应商
    /// </summary>
    /// <param name="id">供应商ID</param>
    /// <param name="dto">更新参数</param>
    /// <returns>更新是否成功</returns>
    public async Task<bool> UpdateAsync(Guid id, UpdateSupplierDto dto)
    {
        var supplier = await _db.Queryable<Supplier>()
            .Where(s => s.Id == id && s.IsDeleted == 0)
            .FirstAsync();

        if (supplier == null)
        {
            throw BusinessException.NotFound("供应商不存在");
        }

        // 更新字段
        supplier.Name = dto.Name;
        supplier.ContactName = dto.ContactName;
        supplier.ContactPhone = dto.ContactPhone;
        supplier.ContactEmail = dto.ContactEmail;
        supplier.Address = dto.Address;
        supplier.BankName = dto.BankName;
        supplier.BankAccount = dto.BankAccount;
        supplier.Remark = dto.Remark;
        supplier.UpdatedAt = DateTime.UtcNow;

        return await _db.Updateable(supplier).ExecuteCommandAsync() > 0;
    }

    /// <summary>
    /// 删除供应商
    /// </summary>
    /// <param name="id">供应商ID</param>
    /// <returns>删除是否成功</returns>
    /// <remarks>
    /// 使用软删除，将 IsDeleted 字段设置为 1
    /// </remarks>
    public async Task<bool> DeleteAsync(Guid id)
    {
        var supplier = await _db.Queryable<Supplier>()
            .Where(s => s.Id == id && s.IsDeleted == 0)
            .FirstAsync();

        if (supplier == null)
        {
            throw BusinessException.NotFound("供应商不存在");
        }

        // 软删除
        supplier.IsDeleted = 1;
        supplier.UpdatedAt = DateTime.UtcNow;

        return await _db.Updateable(supplier).ExecuteCommandAsync() > 0;
    }

    /// <summary>
    /// 启用/禁用供应商
    /// </summary>
    /// <param name="id">供应商ID</param>
    /// <param name="status">状态（0=禁用，1=启用）</param>
    /// <returns>操作是否成功</returns>
    public async Task<bool> UpdateStatusAsync(Guid id, int status)
    {
        var supplier = await _db.Queryable<Supplier>()
            .Where(s => s.Id == id && s.IsDeleted == 0)
            .FirstAsync();

        if (supplier == null)
        {
            throw BusinessException.NotFound("供应商不存在");
        }

        supplier.Status = (Status)status;
        supplier.UpdatedAt = DateTime.UtcNow;

        return await _db.Updateable(supplier).ExecuteCommandAsync() > 0;
    }

    /// <summary>
    /// 获取供应商资质列表
    /// </summary>
    /// <param name="supplierId">供应商ID</param>
    /// <returns>资质列表</returns>
    public async Task<List<SupplierQualificationDto>> GetQualificationsAsync(Guid supplierId)
    {
        var qualifications = await _db.Queryable<SupplierQualification>()
            .Where(q => q.SupplierId == supplierId.ToString() && q.IsDeleted == 0)
            .OrderBy(q => q.CreatedAt, OrderByType.Desc)
            .ToListAsync();

        return qualifications.Select(q => new SupplierQualificationDto
        {
            Id = q.Id.ToString(),
            SupplierId = q.SupplierId,
            Type = q.Type,
            TypeName = q.Type == 1 ? "营业执照" : q.Type == 2 ? "生产许可证" : "质量认证",
            Name = q.Name,
            CertificateNo = q.CertificateNo,
            IssueDate = q.IssueDate,
            ExpireDate = q.ExpireDate,
            ImageUrl = q.ImageUrl,
            QualificationStatus = q.QualificationStatus,
            QualificationStatusName = q.QualificationStatus == 1 ? "有效" : "过期",
            CreatedAt = q.CreatedAt
        }).ToList();
    }

    /// <summary>
    /// 创建供应商资质
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新创建的资质ID</returns>
    public async Task<Guid> CreateQualificationAsync(CreateSupplierQualificationDto dto)
    {
        // 判断资质状态
        int qualificationStatus = 1;
        if (dto.ExpireDate.HasValue && dto.ExpireDate < DateTime.Now)
        {
            qualificationStatus = 0; // 过期
        }

        var qualification = dto.Adapt<SupplierQualification>();
        qualification.Id = Guid.NewGuid();
        qualification.QualificationStatus = qualificationStatus;
        qualification.Status = Status.Enabled;

        await _db.Insertable(qualification).ExecuteCommandAsync();

        return qualification.Id;
    }

    /// <summary>
    /// 更新供应商资质
    /// </summary>
    /// <param name="id">资质ID</param>
    /// <param name="dto">更新参数</param>
    /// <returns>更新是否成功</returns>
    public async Task<bool> UpdateQualificationAsync(Guid id, UpdateSupplierQualificationDto dto)
    {
        var qualification = await _db.Queryable<SupplierQualification>()
            .Where(q => q.Id == id && q.IsDeleted == 0)
            .FirstAsync();

        if (qualification == null)
        {
            throw BusinessException.NotFound("资质不存在");
        }

        // 判断资质状态
        int qualificationStatus = 1;
        if (dto.ExpireDate.HasValue && dto.ExpireDate < DateTime.Now)
        {
            qualificationStatus = 0; // 过期
        }

        // 更新字段
        qualification.Type = dto.Type;
        qualification.Name = dto.Name;
        qualification.CertificateNo = dto.CertificateNo;
        qualification.IssueDate = dto.IssueDate;
        qualification.ExpireDate = dto.ExpireDate;
        qualification.ImageUrl = dto.ImageUrl;
        qualification.QualificationStatus = qualificationStatus;
        qualification.UpdatedAt = DateTime.UtcNow;

        return await _db.Updateable(qualification).ExecuteCommandAsync() > 0;
    }

    /// <summary>
    /// 删除供应商资质
    /// </summary>
    /// <param name="id">资质ID</param>
    /// <returns>删除是否成功</returns>
    public async Task<bool> DeleteQualificationAsync(Guid id)
    {
        var qualification = await _db.Queryable<SupplierQualification>()
            .Where(q => q.Id == id && q.IsDeleted == 0)
            .FirstAsync();

        if (qualification == null)
        {
            throw BusinessException.NotFound("资质不存在");
        }

        // 软删除
        qualification.IsDeleted = 1;
        qualification.UpdatedAt = DateTime.UtcNow;

        return await _db.Updateable(qualification).ExecuteCommandAsync() > 0;
    }

    /// <summary>
    /// 获取启用的供应商列表（下拉选择用）
    /// </summary>
    /// <param name="keyword">关键词（可选，用于搜索供应商名称或编码）</param>
    /// <returns>供应商列表</returns>
    public async Task<List<SupplierDto>> GetActiveListAsync(string? keyword = null)
    {
        var queryable = _db.Queryable<Supplier>()
            .Where(s => s.IsDeleted == 0 && s.Status == Status.Enabled);

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            queryable = queryable.Where(s =>
                s.Name.Contains(keyword) || s.Code.Contains(keyword));
        }

        var suppliers = await queryable
            .OrderBy(s => s.CreatedAt, OrderByType.Desc)
            .Take(50) // 限制返回50条
            .ToListAsync();

        return suppliers.Select(s => new SupplierDto
        {
            Id = s.Id.ToString(),
            Code = s.Code,
            Name = s.Name,
            ContactName = s.ContactName,
            ContactPhone = s.ContactPhone
        }).ToList();
    }

    #region 私有方法

    /// <summary>
    /// 生成供应商编码
    /// </summary>
    /// <returns>供应商编码（格式：S + 年月 + 序号）</returns>
    private async Task<string> GenerateSupplierCodeAsync()
    {
        var now = DateTime.Now;
        var prefix = $"S{now:yyyyMM}";

        // 查询当月最大序号
        var lastSupplier = await _db.Queryable<Supplier>()
            .Where(s => s.Code.StartsWith(prefix))
            .OrderBy(s => s.Code, OrderByType.Desc)
            .FirstAsync();

        int sequence = 1;
        if (lastSupplier != null)
        {
            var lastCode = lastSupplier.Code;
            var lastSequence = int.Parse(lastCode.Substring(prefix.Length));
            sequence = lastSequence + 1;
        }

        return $"{prefix}{sequence:D4}";
    }

    #endregion
}