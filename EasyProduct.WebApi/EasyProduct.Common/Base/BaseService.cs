using System.Linq.Expressions;
using SqlSugar;

namespace EasyProduct.Common.Base;

/// <summary>
/// 非泛型服务基类，提供数据库上下文注入
/// </summary>
/// <remarks>
/// 适用于不需要泛型 CRUD 操作的服务，如多表查询、复杂业务逻辑等。
/// 数据库上下文 _db 通过 Autofac 属性注入方式自动赋值。
/// </remarks>
/// <example>
/// <code>
/// public class MyService : BaseService, IMyService
/// {
///     // _db 自动注入，无需构造函数
///     public async Task&lt;List&lt;MyDto&gt;&gt; GetComplexDataAsync()
///     {
///         // 多表关联查询
///         return await _db.Queryable&lt;TableA, TableB&gt;((a, b) => new JoinQueryInfos(
///             JoinType.Left, a.Id == b.AId
///         ))
///         .Select((a, b) => new MyDto { ... })
///         .ToListAsync();
///     }
/// }
/// </code>
/// </example>
public class BaseService
{
    /// <summary>
    /// 数据库上下文（通过 Autofac 属性注入）
    /// </summary>
    /// <remarks>
    /// 使用 SqlSugar 的 ISqlSugarClient 接口，支持多种数据库操作。
    /// 通过 Autofac 的属性注入功能自动赋值，无需构造函数。
    /// 属性命名规则：以 "_" 开头的公共属性会自动注入。
    /// </remarks>
    public ISqlSugarClient _db { get; set; } = null!;
}

/// <summary>
/// 泛型仓储基类，提供通用的 CRUD 操作方法
/// </summary>
/// <typeparam name="T">实体类型，必须具有无参构造函数</typeparam>
/// <remarks>
/// 该类封装了常用的数据库操作方法，包括查询、插入、更新、删除等。
/// 继承此类的 Service 可以直接使用这些方法，减少重复代码。
/// 数据库上下文 _db 通过 Autofac 属性注入方式自动赋值，无需构造函数。
/// </remarks>
/// <example>
/// <code>
/// public class UserService : BaseService&lt;User&gt;
/// {
///     // _db 自动注入，无需构造函数
///
///     // 直接使用基类方法
///     public async Task&lt;User?&gt; GetByName(string name)
///     {
///         return await GetFirstAsync(u => u.Name == name);
///     }
/// }
/// </code>
/// </example>
public class BaseService<T> where T : class, new()
{
    /// <summary>
    /// 数据库上下文（通过 Autofac 属性注入）
    /// </summary>
    /// <remarks>
    /// 使用 SqlSugar 的 ISqlSugarClient 接口，支持多种数据库操作。
    /// 通过 Autofac 的属性注入功能自动赋值，无需构造函数。
    /// 属性命名规则：以 "_" 开头的公共属性会自动注入。
    /// </remarks>
    public ISqlSugarClient _db { get; set; } = null!;

    /// <summary>
    /// 查询所有记录
    /// </summary>
    public virtual async Task<List<T>> GetListAsync()
    {
        return await _db.Queryable<T>().ToListAsync();
    }

    /// <summary>
    /// 根据条件查询记录列表
    /// </summary>
    public virtual async Task<List<T>> GetListAsync(Expression<Func<T, bool>> whereExpr)
    {
        return await _db.Queryable<T>().Where(whereExpr).ToListAsync();
    }

    /// <summary>
    /// 分页查询记录列表
    /// </summary>
    public virtual async Task<PageResponse<T>> GetPageListAsync(
        int pageIndex,
        int pageSize,
        Expression<Func<T, bool>>? whereExpr = null,
        Expression<Func<T, object>>? orderBy = null,
        bool isAsc = false)
    {
        var query = _db.Queryable<T>();

        if (whereExpr != null)
            query = query.Where(whereExpr);

        if (orderBy != null)
            query = isAsc ? query.OrderBy(orderBy) : query.OrderByDescending(orderBy);

        RefAsync<int> total = 0;
        var items = await query.ToPageListAsync(pageIndex, pageSize, total);

        return PageResponse<T>.Create(items, total.Value, pageIndex, pageSize);
    }

    /// <summary>
    /// 根据 ID 查询单条记录
    /// </summary>
    public virtual async Task<T?> GetByIdAsync(Guid id)
    {
        return await _db.Queryable<T>().InSingleAsync(id);
    }

    /// <summary>
    /// 根据条件查询单条记录
    /// </summary>
    public virtual async Task<T?> GetFirstAsync(Expression<Func<T, bool>> whereExpr)
    {
        return await _db.Queryable<T>().Where(whereExpr).FirstAsync();
    }

    /// <summary>
    /// 插入新记录
    /// </summary>
    public virtual async Task<bool> InsertAsync(T entity)
    {
        return await _db.Insertable(entity).ExecuteCommandIdentityIntoEntityAsync();
    }

    /// <summary>
    /// 批量插入记录
    /// </summary>
    public virtual async Task<int> InsertRangeAsync(List<T> entities)
    {
        return await _db.Insertable(entities).ExecuteCommandAsync();
    }

    /// <summary>
    /// 更新记录
    /// </summary>
    public virtual async Task<int> UpdateAsync(T entity)
    {
        return await _db.Updateable(entity).ExecuteCommandAsync();
    }

    /// <summary>
    /// 更新指定字段
    /// </summary>
    public virtual async Task<int> UpdateAsync(T entity, Expression<Func<T, object>> columns)
    {
        return await _db.Updateable(entity).UpdateColumns(columns).ExecuteCommandAsync();
    }

    /// <summary>
    /// 根据 ID 删除记录
    /// </summary>
    public virtual async Task<int> DeleteAsync(Guid id)
    {
        return await _db.Deleteable<T>().In(id).ExecuteCommandAsync();
    }

    /// <summary>
    /// 根据条件删除记录
    /// </summary>
    public virtual async Task<int> DeleteAsync(Expression<Func<T, bool>> whereExpr)
    {
        return await _db.Deleteable<T>().Where(whereExpr).ExecuteCommandAsync();
    }

    /// <summary>
    /// 根据 ID 列表批量删除记录
    /// </summary>
    public virtual async Task<int> DeleteRangeAsync(List<Guid> ids)
    {
        return await _db.Deleteable<T>().In(ids).ExecuteCommandAsync();
    }

    /// <summary>
    /// 判断是否存在满足条件的记录
    /// </summary>
    public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> whereExpr)
    {
        return await _db.Queryable<T>().AnyAsync(whereExpr);
    }

    /// <summary>
    /// 统计满足条件的记录数量
    /// </summary>
    public virtual async Task<int> CountAsync(Expression<Func<T, bool>>? whereExpr = null)
    {
        var query = _db.Queryable<T>();
        if (whereExpr != null)
            query = query.Where(whereExpr);
        return await query.CountAsync();
    }

    /// <summary>
    /// 在事务中执行多个操作
    /// </summary>
    public virtual async Task<bool> ExecuteTransactionAsync(Func<Task> action)
    {
        try
        {
            _db.Ado.BeginTran();
            await action();
            _db.Ado.CommitTran();
            return true;
        }
        catch
        {
            _db.Ado.RollbackTran();
            throw;
        }
    }
}
