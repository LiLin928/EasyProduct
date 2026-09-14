using EasyProduct.Common.Base;
using EasyProduct.Common.Error;
using EasyProduct.Models.Dto.Crm;
using EasyProduct.Models.Entitys.Basic;
using EasyProduct.Models.Entitys.Crm;
using EasyProduct.Models.Entitys.Site;
using EasyProduct.Models.Enums;
using EasyProduct.Models.Enums.Crm;
using Mapster;
using SqlSugar;

namespace EasyProduct.Business.Crm;

/// <summary>
/// 客户服务实现
/// </summary>
/// <remarks>
/// 提供客户的增删改查、联系人管理、地址管理等功能
/// </remarks>
public class CustomerService : BaseService, ICustomerService
{
    /// <summary>
    /// 获取客户分页列表
    /// </summary>
    /// <param name="query">查询参数，包含关键词、客户类型、客户来源、状态、分页信息</param>
    /// <returns>客户分页结果</returns>
    /// <remarks>
    /// 1. 支持按客户名称、编码、联系人模糊搜索
    /// 2. 支持按客户类型、客户来源、状态筛选
    /// 3. 默认按创建时间倒序排列
    /// 4. 包含业务员姓名的关联查询
    /// </remarks>
    public async Task<PageResponse<CustomerDto>> GetListAsync(CustomerQueryDto query)
    {
        // 构建查询（关联用户表获取业务员姓名）
        var queryable = _db.Queryable<Customer, basic_user>(
            (c, u) => new JoinQueryInfos(
                JoinType.Left, c.BusinessUserId == u.Id.ToString()
            ))
            .Where((c, u) => c.IsDeleted == 0);

        // 关键词搜索（客户名称、编码、联系人）
        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            queryable = queryable.Where((c, u) =>
                c.Name.Contains(query.Keyword) ||
                c.Code.Contains(query.Keyword) ||
                (c.ContactName != null && c.ContactName.Contains(query.Keyword)));
        }

        // 客户名称搜索
        if (!string.IsNullOrWhiteSpace(query.Name))
        {
            queryable = queryable.Where((c, u) => c.Name.Contains(query.Name));
        }

        // 客户编码搜索
        if (!string.IsNullOrWhiteSpace(query.Code))
        {
            queryable = queryable.Where((c, u) => c.Code.Contains(query.Code));
        }

        // 客户类型筛选
        if (query.Type.HasValue)
        {
            queryable = queryable.Where((c, u) => c.Type == query.Type);
        }

        // 客户来源筛选
        if (query.Source.HasValue)
        {
            queryable = queryable.Where((c, u) => c.Source == query.Source);
        }

        // 状态筛选
        if (query.Status.HasValue)
        {
            queryable = queryable.Where((c, u) => c.Status == (Status)query.Status);
        }

        // 业务员筛选
        if (!string.IsNullOrWhiteSpace(query.BusinessUserId))
        {
            queryable = queryable.Where((c, u) => c.BusinessUserId == query.BusinessUserId);
        }

        // 按创建时间倒序
        queryable = queryable.OrderBy((c, u) => c.CreatedAt, OrderByType.Desc);

        // 分页查询
        RefAsync<int> total = 0;
        var result = await queryable
            .Select((c, u) => new CustomerDto
            {
                Id = c.Id.ToString(),
                Code = c.Code,
                Name = c.Name,
                Type = c.Type,
                TypeName = c.Type == 1 ? "B2B" : "零售",
                Source = c.Source,
                SourceName = c.Source == 1 ? "询价转化" : c.Source == 2 ? "注册建档" : "手动建档",
                Status = (int)c.Status,
                StatusName = c.Status == Status.Enabled ? "启用" : "禁用",
                ContactName = c.ContactName,
                ContactPhone = c.ContactPhone,
                ContactEmail = c.ContactEmail,
                Address = c.Address,
                BusinessUserId = c.BusinessUserId,
                BusinessUserName = u.UserName,
                Remark = c.Remark,
                CreatedAt = c.CreatedAt
            })
            .ToPageListAsync(query.PageIndex, query.PageSize, total);

        return PageResponse<CustomerDto>.Create(result, total.Value, query.PageIndex, query.PageSize);
    }

    /// <summary>
    /// 获取客户详情
    /// </summary>
    /// <param name="id">客户ID</param>
    /// <returns>客户详情</returns>
    /// <remarks>
    /// 包含客户基本信息和业务员姓名
    /// </remarks>
    public async Task<CustomerDto> GetByIdAsync(Guid id)
    {
        var customer = await _db.Queryable<Customer>()
            .Where(c => c.Id == id && c.IsDeleted == 0)
            .FirstAsync();

        if (customer == null)
        {
            throw BusinessException.NotFound("客户不存在");
        }

        // 获取业务员姓名
        string? businessUserName = null;
        if (!string.IsNullOrWhiteSpace(customer.BusinessUserId))
        {
            var user = await _db.Queryable<basic_user>()
                .Where(u => u.Id.ToString() == customer.BusinessUserId)
                .FirstAsync();
            businessUserName = user?.UserName;
        }

        return new CustomerDto
        {
            Id = customer.Id.ToString(),
            Code = customer.Code,
            Name = customer.Name,
            Type = customer.Type,
            TypeName = customer.Type == 1 ? "B2B" : "零售",
            Source = customer.Source,
            SourceName = customer.Source == 1 ? "询价转化" : customer.Source == 2 ? "注册建档" : "手动建档",
            Status = (int)customer.Status,
            StatusName = customer.Status == Status.Enabled ? "启用" : "禁用",
            ContactName = customer.ContactName,
            ContactPhone = customer.ContactPhone,
            ContactEmail = customer.ContactEmail,
            Address = customer.Address,
            BusinessUserId = customer.BusinessUserId,
            BusinessUserName = businessUserName,
            Remark = customer.Remark,
            CreatedAt = customer.CreatedAt
        };
    }

    /// <summary>
    /// 创建客户
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新创建的客户ID</returns>
    /// <remarks>
    /// 创建客户时会自动生成客户编码（C + 年月 + 序号）
    /// </remarks>
    public async Task<Guid> CreateAsync(CreateCustomerDto dto)
    {
        // 生成客户编码
        var code = await GenerateCustomerCodeAsync();

        var customer = dto.Adapt<Customer>();
        customer.Id = Guid.NewGuid();
        customer.Code = code;
        customer.Status = Status.Enabled;

        await _db.Insertable(customer).ExecuteCommandAsync();

        return customer.Id;
    }

    /// <summary>
    /// 更新客户
    /// </summary>
    /// <param name="id">客户ID</param>
    /// <param name="dto">更新参数</param>
    /// <returns>更新是否成功</returns>
    public async Task<bool> UpdateAsync(Guid id, UpdateCustomerDto dto)
    {
        var customer = await _db.Queryable<Customer>()
            .Where(c => c.Id == id && c.IsDeleted == 0)
            .FirstAsync();

        if (customer == null)
        {
            throw BusinessException.NotFound("客户不存在");
        }

        // 更新字段
        customer.Name = dto.Name;
        customer.Type = dto.Type;
        customer.ContactName = dto.ContactName;
        customer.ContactPhone = dto.ContactPhone;
        customer.ContactEmail = dto.ContactEmail;
        customer.Address = dto.Address;
        customer.BusinessUserId = dto.BusinessUserId;
        customer.Remark = dto.Remark;
        customer.UpdatedAt = DateTime.UtcNow;

        return await _db.Updateable(customer).ExecuteCommandAsync() > 0;
    }

    /// <summary>
    /// 删除客户
    /// </summary>
    /// <param name="id">客户ID</param>
    /// <returns>删除是否成功</returns>
    /// <remarks>
    /// 使用软删除，将 IsDeleted 字段设置为 1
    /// </remarks>
    public async Task<bool> DeleteAsync(Guid id)
    {
        var customer = await _db.Queryable<Customer>()
            .Where(c => c.Id == id && c.IsDeleted == 0)
            .FirstAsync();

        if (customer == null)
        {
            throw BusinessException.NotFound("客户不存在");
        }

        // 软删除
        customer.IsDeleted = 1;
        customer.UpdatedAt = DateTime.UtcNow;

        return await _db.Updateable(customer).ExecuteCommandAsync() > 0;
    }

    /// <summary>
    /// 启用/禁用客户
    /// </summary>
    /// <param name="id">客户ID</param>
    /// <param name="status">状态（0=禁用，1=启用）</param>
    /// <returns>操作是否成功</returns>
    public async Task<bool> UpdateStatusAsync(Guid id, int status)
    {
        var customer = await _db.Queryable<Customer>()
            .Where(c => c.Id == id && c.IsDeleted == 0)
            .FirstAsync();

        if (customer == null)
        {
            throw BusinessException.NotFound("客户不存在");
        }

        customer.Status = (Status)status;
        customer.UpdatedAt = DateTime.UtcNow;

        return await _db.Updateable(customer).ExecuteCommandAsync() > 0;
    }

    #region 联系人管理

    /// <summary>
    /// 获取客户联系人列表
    /// </summary>
    /// <param name="customerId">客户ID</param>
    /// <returns>联系人列表</returns>
    public async Task<List<CustomerContactDto>> GetContactsAsync(Guid customerId)
    {
        var contacts = await _db.Queryable<CustomerContact>()
            .Where(c => c.CustomerId == customerId.ToString() && c.IsDeleted == 0)
            .OrderBy(c => c.IsPrimary, OrderByType.Desc)
            .OrderBy(c => c.CreatedAt, OrderByType.Desc)
            .ToListAsync();

        return contacts.Select(c => new CustomerContactDto
        {
            Id = c.Id.ToString(),
            CustomerId = c.CustomerId,
            Name = c.Name,
            Phone = c.Phone,
            Email = c.Email,
            Position = c.Position,
            IsPrimary = c.IsPrimary,
            IsPrimaryName = c.IsPrimary == 1 ? "是" : "否",
            CreatedAt = c.CreatedAt
        }).ToList();
    }

    /// <summary>
    /// 创建客户联系人
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新创建的联系人ID</returns>
    /// <remarks>
    /// 如果设置为主要联系人，会自动取消其他联系人的主要标记
    /// </remarks>
    public async Task<Guid> CreateContactAsync(CreateCustomerContactDto dto)
    {
        // 如果设置为主要联系人，取消其他联系人的主要标记
        if (dto.IsPrimary == 1)
        {
            await _db.Updateable<CustomerContact>()
                .SetColumns(c => c.IsPrimary == 0)
                .Where(c => c.CustomerId == dto.CustomerId && c.IsPrimary == 1)
                .ExecuteCommandAsync();
        }

        var contact = dto.Adapt<CustomerContact>();
        contact.Id = Guid.NewGuid();
        contact.Status = Status.Enabled;

        await _db.Insertable(contact).ExecuteCommandAsync();

        return contact.Id;
    }

    /// <summary>
    /// 更新客户联系人
    /// </summary>
    /// <param name="id">联系人ID</param>
    /// <param name="dto">更新参数</param>
    /// <returns>更新是否成功</returns>
    public async Task<bool> UpdateContactAsync(Guid id, UpdateCustomerContactDto dto)
    {
        var contact = await _db.Queryable<CustomerContact>()
            .Where(c => c.Id == id && c.IsDeleted == 0)
            .FirstAsync();

        if (contact == null)
        {
            throw BusinessException.NotFound("联系人不存在");
        }

        // 如果设置为主要联系人，取消其他联系人的主要标记
        if (dto.IsPrimary == 1 && contact.IsPrimary == 0)
        {
            await _db.Updateable<CustomerContact>()
                .SetColumns(c => c.IsPrimary == 0)
                .Where(c => c.CustomerId == contact.CustomerId && c.IsPrimary == 1)
                .ExecuteCommandAsync();
        }

        // 更新字段
        contact.Name = dto.Name;
        contact.Phone = dto.Phone;
        contact.Email = dto.Email;
        contact.Position = dto.Position;
        contact.IsPrimary = dto.IsPrimary;
        contact.UpdatedAt = DateTime.UtcNow;

        return await _db.Updateable(contact).ExecuteCommandAsync() > 0;
    }

    /// <summary>
    /// 删除客户联系人
    /// </summary>
    /// <param name="id">联系人ID</param>
    /// <returns>删除是否成功</returns>
    public async Task<bool> DeleteContactAsync(Guid id)
    {
        var contact = await _db.Queryable<CustomerContact>()
            .Where(c => c.Id == id && c.IsDeleted == 0)
            .FirstAsync();

        if (contact == null)
        {
            throw BusinessException.NotFound("联系人不存在");
        }

        // 软删除
        contact.IsDeleted = 1;
        contact.UpdatedAt = DateTime.UtcNow;

        return await _db.Updateable(contact).ExecuteCommandAsync() > 0;
    }

    #endregion

    #region 地址管理

    /// <summary>
    /// 获取客户地址列表
    /// </summary>
    /// <param name="customerId">客户ID</param>
    /// <returns>地址列表</returns>
    public async Task<List<CustomerAddressDto>> GetAddressesAsync(Guid customerId)
    {
        var addresses = await _db.Queryable<CustomerAddress>()
            .Where(a => a.CustomerId == customerId.ToString() && a.IsDeleted == 0)
            .OrderBy(a => a.IsDefault, OrderByType.Desc)
            .OrderBy(a => a.CreatedAt, OrderByType.Desc)
            .ToListAsync();

        return addresses.Select(a => new CustomerAddressDto
        {
            Id = a.Id.ToString(),
            CustomerId = a.CustomerId,
            ReceiverName = a.ReceiverName,
            Phone = a.Phone,
            Province = a.Province,
            City = a.City,
            District = a.District,
            DetailAddress = a.DetailAddress,
            FullAddress = $"{a.Province}{a.City}{a.District}{a.DetailAddress}",
            IsDefault = a.IsDefault,
            IsDefaultName = a.IsDefault == 1 ? "是" : "否",
            CreatedAt = a.CreatedAt
        }).ToList();
    }

    /// <summary>
    /// 创建客户地址
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新创建的地址ID</returns>
    /// <remarks>
    /// 如果设置为默认地址，会自动取消其他地址的默认标记
    /// </remarks>
    public async Task<Guid> CreateAddressAsync(CreateCustomerAddressDto dto)
    {
        // 如果设置为默认地址，取消其他地址的默认标记
        if (dto.IsDefault == 1)
        {
            await _db.Updateable<CustomerAddress>()
                .SetColumns(a => a.IsDefault == 0)
                .Where(a => a.CustomerId == dto.CustomerId && a.IsDefault == 1)
                .ExecuteCommandAsync();
        }

        var address = dto.Adapt<CustomerAddress>();
        address.Id = Guid.NewGuid();
        address.Status = Status.Enabled;

        await _db.Insertable(address).ExecuteCommandAsync();

        return address.Id;
    }

    /// <summary>
    /// 更新客户地址
    /// </summary>
    /// <param name="id">地址ID</param>
    /// <param name="dto">更新参数</param>
    /// <returns>更新是否成功</returns>
    public async Task<bool> UpdateAddressAsync(Guid id, UpdateCustomerAddressDto dto)
    {
        var address = await _db.Queryable<CustomerAddress>()
            .Where(a => a.Id == id && a.IsDeleted == 0)
            .FirstAsync();

        if (address == null)
        {
            throw BusinessException.NotFound("地址不存在");
        }

        // 如果设置为默认地址，取消其他地址的默认标记
        if (dto.IsDefault == 1 && address.IsDefault == 0)
        {
            await _db.Updateable<CustomerAddress>()
                .SetColumns(a => a.IsDefault == 0)
                .Where(a => a.CustomerId == address.CustomerId && a.IsDefault == 1)
                .ExecuteCommandAsync();
        }

        // 更新字段
        address.ReceiverName = dto.ReceiverName;
        address.Phone = dto.Phone;
        address.Province = dto.Province;
        address.City = dto.City;
        address.District = dto.District;
        address.DetailAddress = dto.DetailAddress;
        address.IsDefault = dto.IsDefault;
        address.UpdatedAt = DateTime.UtcNow;

        return await _db.Updateable(address).ExecuteCommandAsync() > 0;
    }

    /// <summary>
    /// 删除客户地址
    /// </summary>
    /// <param name="id">地址ID</param>
    /// <returns>删除是否成功</returns>
    public async Task<bool> DeleteAddressAsync(Guid id)
    {
        var address = await _db.Queryable<CustomerAddress>()
            .Where(a => a.Id == id && a.IsDeleted == 0)
            .FirstAsync();

        if (address == null)
        {
            throw BusinessException.NotFound("地址不存在");
        }

        // 软删除
        address.IsDeleted = 1;
        address.UpdatedAt = DateTime.UtcNow;

        return await _db.Updateable(address).ExecuteCommandAsync() > 0;
    }

    #endregion

    #region 特殊功能

    /// <summary>
    /// 询价转客户
    /// </summary>
    /// <param name="inquiryId">询价单ID</param>
    /// <returns>新创建的客户ID</returns>
    /// <remarks>
    /// 从询价单创建客户档案：
    /// - 客户类型：B2B
    /// - 客户来源：询价
    /// - 自动填充联系人、联系方式等信息
    /// </remarks>
    public async Task<Guid> CreateFromInquiryAsync(Guid inquiryId)
    {
        // 查询问价单
        var inquiry = await _db.Queryable<EasyProduct.Models.Entitys.Site.site_inquiry>()
            .Where(i => i.Id == inquiryId && i.IsDeleted == 0)
            .FirstAsync();

        if (inquiry == null)
        {
            throw BusinessException.NotFound("询价单不存在");
        }

        // 检查是否已转客户
        if (!string.IsNullOrWhiteSpace(inquiry.CustomerId))
        {
            throw BusinessException.BadRequest("该询价单已转客户");
        }

        // 生成客户编码
        var code = await GenerateCustomerCodeAsync();

        // 创建客户
        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            Code = code,
            Name = inquiry.CompanyName ?? inquiry.ContactName,
            Type = (int)CustomerType.B2B,
            Source = (int)CustomerSource.Inquiry,
            ContactName = inquiry.ContactName,
            ContactPhone = inquiry.Phone,
            ContactEmail = inquiry.Email,
            Address = inquiry.Address,
            Status = Status.Enabled
        };

        await _db.Insertable(customer).ExecuteCommandAsync();

        // 更新询价单的客户ID
        inquiry.CustomerId = customer.Id.ToString();
        inquiry.UpdatedAt = DateTime.UtcNow;
        await _db.Updateable(inquiry).ExecuteCommandAsync();

        return customer.Id;
    }

    /// <summary>
    /// 获取启用的客户列表（下拉选择用）
    /// </summary>
    /// <param name="keyword">关键词（可选，用于搜索客户名称或编码）</param>
    /// <returns>客户列表</returns>
    public async Task<List<CustomerDto>> GetActiveListAsync(string? keyword = null)
    {
        var queryable = _db.Queryable<Customer>()
            .Where(c => c.IsDeleted == 0 && c.Status == Status.Enabled);

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            queryable = queryable.Where(c =>
                c.Name.Contains(keyword) || c.Code.Contains(keyword));
        }

        var customers = await queryable
            .OrderBy(c => c.CreatedAt, OrderByType.Desc)
            .Take(50) // 限制返回50条
            .ToListAsync();

        return customers.Select(c => new CustomerDto
        {
            Id = c.Id.ToString(),
            Code = c.Code,
            Name = c.Name,
            Type = c.Type,
            TypeName = c.Type == 1 ? "B2B" : "零售",
            ContactName = c.ContactName,
            ContactPhone = c.ContactPhone
        }).ToList();
    }

    #endregion

    #region 私有方法

    /// <summary>
    /// 生成客户编码
    /// </summary>
    /// <returns>客户编码（格式：C + 年月 + 序号）</returns>
    private async Task<string> GenerateCustomerCodeAsync()
    {
        var now = DateTime.Now;
        var prefix = $"C{now:yyyyMM}";

        // 查询当月最大序号
        var lastCustomer = await _db.Queryable<Customer>()
            .Where(c => c.Code.StartsWith(prefix))
            .OrderBy(c => c.Code, OrderByType.Desc)
            .FirstAsync();

        int sequence = 1;
        if (lastCustomer != null)
        {
            var lastCode = lastCustomer.Code;
            var lastSequence = int.Parse(lastCode.Substring(prefix.Length));
            sequence = lastSequence + 1;
        }

        return $"{prefix}{sequence:D4}";
    }

    #endregion
}