using EasyProduct.Common.Base;
using EasyProduct.Common.Error;
using EasyProduct.Models.Dto.Basic.Dept;
using EasyProduct.Models.Entitys.Basic;
using Mapster;
using Microsoft.Extensions.Logging;
using SqlSugar;

namespace EasyProduct.Business.Basic;

/// <summary>
/// 部门服务实现
/// </summary>
/// <remarks>
/// 提供部门的增删改查、树形结构查询等功能
/// </remarks>
public class DeptService : BaseService, IDeptService
{
    private readonly ILogger<DeptService> _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="logger">日志服务</param>
    public DeptService(ILogger<DeptService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// 获取部门树形结构
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>部门树形列表</returns>
    public async Task<List<DeptTreeDto>> GetDeptTreeAsync(DeptTreeQueryDto query)
    {
        // 查询所有部门（根据条件过滤）
        var queryable = _db.Queryable<basic_dept>()
            .Where(x => x.IsDeleted == 0);

        if (query.OnlyEnabled == true)
        {
            queryable = queryable.Where(x => x.Status == Models.Enums.Status.Enabled);
        }

        if (!string.IsNullOrEmpty(query.ParentId))
        {
            queryable = queryable.Where(x => x.ParentId == query.ParentId);
        }

        var depts = await queryable
            .OrderBy(x => x.Sort)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync();

        // 转换为 DTO
        var deptDtos = depts.Adapt<List<DeptTreeDto>>();

        // 构建树形结构
        return BuildDeptTree(deptDtos, "0");
    }

    /// <summary>
    /// 获取部门列表（扁平）
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>部门列表</returns>
    public async Task<List<DeptDto>> GetDeptListAsync(DeptQueryDto query)
    {
        var queryable = _db.Queryable<basic_dept>()
            .Where(x => x.IsDeleted == 0);

        if (!string.IsNullOrEmpty(query.Name))
        {
            queryable = queryable.Where(x => x.DeptName.Contains(query.Name));
        }

        if (!string.IsNullOrEmpty(query.Code))
        {
            queryable = queryable.Where(x => x.DeptCode != null && x.DeptCode.Contains(query.Code));
        }

        if (query.Status.HasValue)
        {
            queryable = queryable.Where(x => (int)x.Status == query.Status.Value);
        }

        var depts = await queryable
            .OrderBy(x => x.Sort)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync();

        return depts.Adapt<List<DeptDto>>();
    }

    /// <summary>
    /// 获取部门详情
    /// </summary>
    /// <param name="id">部门ID</param>
    /// <returns>部门详情</returns>
    public async Task<DeptDto> GetDeptByIdAsync(string id)
    {
        var dept = await _db.Queryable<basic_dept>()
            .Where(x => x.Id.ToString() == id && x.IsDeleted == 0)
            .FirstAsync();

        if (dept == null)
        {
            throw new BusinessException("部门不存在", 404);
        }

        return dept.Adapt<DeptDto>();
    }

    /// <summary>
    /// 创建部门
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新部门ID</returns>
    public async Task<string> CreateDeptAsync(CreateDeptDto dto)
    {
        // 检查部门编码是否重复
        if (!string.IsNullOrEmpty(dto.DeptCode))
        {
            var exists = await _db.Queryable<basic_dept>()
                .Where(x => x.DeptCode == dto.DeptCode && x.IsDeleted == 0)
                .AnyAsync();

            if (exists)
            {
                throw new BusinessException($"部门编码 {dto.DeptCode} 已存在");
            }
        }

        // 创建部门实体
        var dept = dto.Adapt<basic_dept>();
        dept.Id = Guid.NewGuid();
        dept.Status = Models.Enums.Status.Enabled;
        dept.CreatedAt = DateTime.UtcNow;
        dept.UpdatedAt = DateTime.UtcNow;

        // 计算部门层级和完整路径
        await CalculateDeptLevelAndPath(dept);

        // 插入数据库
        await _db.Insertable(dept).ExecuteCommandAsync();

        _logger.LogInformation("创建部门成功：{DeptName}, ID: {Id}", dept.DeptName, dept.Id);

        return dept.Id.ToString();
    }

    /// <summary>
    /// 更新部门
    /// </summary>
    /// <param name="dto">更新参数</param>
    /// <returns>是否成功</returns>
    public async Task<bool> UpdateDeptAsync(UpdateDeptDto dto)
    {
        // 检查部门是否存在
        var dept = await _db.Queryable<basic_dept>()
            .Where(x => x.Id.ToString() == dto.Id && x.IsDeleted == 0)
            .FirstAsync();

        if (dept == null)
        {
            throw new BusinessException("部门不存在", 404);
        }

        // 检查部门编码是否重复（排除自己）
        if (!string.IsNullOrEmpty(dto.DeptCode))
        {
            var exists = await _db.Queryable<basic_dept>()
                .Where(x => x.DeptCode == dto.DeptCode && x.Id.ToString() != dto.Id && x.IsDeleted == 0)
                .AnyAsync();

            if (exists)
            {
                throw new BusinessException($"部门编码 {dto.DeptCode} 已存在");
            }
        }

        // 检查是否将部门设置为自己的子部门
        if (dto.ParentId == dto.Id)
        {
            throw new BusinessException("不能将部门的上级设置为自己");
        }

        // 更新部门信息
        dept.DeptName = dto.DeptName;
        dept.ParentId = dto.ParentId;
        dept.DeptCode = dto.DeptCode;
        dept.LeaderId = dto.LeaderId;
        dept.LeaderName = dto.LeaderName;
        dept.Phone = dto.Phone;
        dept.Email = dto.Email;
        dept.Sort = dto.Sort;
        dept.Status = (Models.Enums.Status)dto.Status;
        dept.Description = dto.Description;
        dept.UpdatedAt = DateTime.UtcNow;

        // 重新计算部门层级和完整路径
        await CalculateDeptLevelAndPath(dept);

        // 更新数据库
        await _db.Updateable(dept).ExecuteCommandAsync();

        _logger.LogInformation("更新部门成功：{DeptName}, ID: {Id}", dept.DeptName, dept.Id);

        return true;
    }

    /// <summary>
    /// 删除部门
    /// </summary>
    /// <param name="id">部门ID</param>
    /// <returns>是否成功</returns>
    public async Task<bool> DeleteDeptAsync(string id)
    {
        // 检查部门是否存在
        var dept = await _db.Queryable<basic_dept>()
            .Where(x => x.Id.ToString() == id && x.IsDeleted == 0)
            .FirstAsync();

        if (dept == null)
        {
            throw new BusinessException("部门不存在", 404);
        }

        // 检查是否有子部门
        var hasChildren = await _db.Queryable<basic_dept>()
            .Where(x => x.ParentId == id && x.IsDeleted == 0)
            .AnyAsync();

        if (hasChildren)
        {
            throw new BusinessException("该部门下存在子部门，不能删除");
        }

        // 检查部门下是否有用户
        var hasUsers = await _db.Queryable<basic_user>()
            .Where(x => x.DeptId.ToString() == id && x.IsDeleted == 0)
            .AnyAsync();

        if (hasUsers)
        {
            throw new BusinessException("该部门下存在用户，不能删除");
        }

        // 软删除
        dept.IsDeleted = 1;
        dept.UpdatedAt = DateTime.UtcNow;

        await _db.Updateable(dept).ExecuteCommandAsync();

        _logger.LogInformation("删除部门成功：{DeptName}, ID: {Id}", dept.DeptName, dept.Id);

        return true;
    }

    /// <summary>
    /// 更新部门状态
    /// </summary>
    /// <param name="id">部门ID</param>
    /// <param name="status">状态：0=禁用，1=启用</param>
    /// <returns>是否成功</returns>
    public async Task<bool> UpdateDeptStatusAsync(string id, int status)
    {
        var dept = await _db.Queryable<basic_dept>()
            .Where(x => x.Id.ToString() == id && x.IsDeleted == 0)
            .FirstAsync();

        if (dept == null)
        {
            throw new BusinessException("部门不存在", 404);
        }

        dept.Status = (Models.Enums.Status)status;
        dept.UpdatedAt = DateTime.UtcNow;

        await _db.Updateable(dept).ExecuteCommandAsync();

        _logger.LogInformation("更新部门状态成功：{DeptName}, 状态: {Status}", dept.DeptName, status);

        return true;
    }

    /// <summary>
    /// 获取部门下的用户列表
    /// </summary>
    /// <param name="deptId">部门ID</param>
    /// <returns>用户列表</returns>
    public async Task<List<DeptUserDto>> GetDeptUsersAsync(string deptId)
    {
        var users = await _db.Queryable<basic_user>()
            .Where(x => x.DeptId.ToString() == deptId && x.IsDeleted == 0)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync();

        return users.Adapt<List<DeptUserDto>>();
    }

    /// <summary>
    /// 构建部门树形结构
    /// </summary>
    /// <param name="allDepts">所有部门列表</param>
    /// <param name="parentId">父部门ID</param>
    /// <returns>树形结构列表</returns>
    private List<DeptTreeDto> BuildDeptTree(List<DeptTreeDto> allDepts, string parentId)
    {
        var children = allDepts
            .Where(x => x.ParentId == parentId || (string.IsNullOrEmpty(x.ParentId) && parentId == "0"))
            .OrderBy(x => x.Sort)
            .ToList();

        foreach (var child in children)
        {
            child.Children = BuildDeptTree(allDepts, child.Id);
        }

        return children;
    }

    /// <summary>
    /// 计算部门层级和完整路径
    /// </summary>
    /// <param name="dept">部门实体</param>
    private async Task CalculateDeptLevelAndPath(basic_dept dept)
    {
        if (string.IsNullOrEmpty(dept.ParentId) || dept.ParentId == "0")
        {
            // 根部门
            dept.Level = 1;
            dept.FullPath = dept.DeptName;
        }
        else
        {
            // 查询父部门
            var parent = await _db.Queryable<basic_dept>()
                .Where(x => x.Id.ToString() == dept.ParentId && x.IsDeleted == 0)
                .FirstAsync();

            if (parent == null)
            {
                throw new BusinessException("父部门不存在");
            }

            dept.Level = parent.Level + 1;
            dept.FullPath = $"{parent.FullPath}/{dept.DeptName}";
        }
    }
}