using EasyProduct.Models.Dto.Basic.Dict;

namespace EasyProduct.Business.Basic;

/// <summary>
/// 字典服务接口
/// </summary>
/// <remarks>
/// 提供字典类型和字典数据的增删改查、缓存管理等功能
/// </remarks>
public interface IDictService
{
    #region 字典类型管理

    /// <summary>
    /// 获取字典类型列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>字典类型列表</returns>
    Task<List<DictTypeDto>> GetDictTypeListAsync(DictTypeQueryDto query);

    /// <summary>
    /// 获取字典类型详情
    /// </summary>
    /// <param name="id">字典类型ID</param>
    /// <returns>字典类型详情</returns>
    Task<DictTypeDto> GetDictTypeByIdAsync(string id);

    /// <summary>
    /// 创建字典类型
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新字典类型ID</returns>
    Task<string> CreateDictTypeAsync(CreateDictTypeDto dto);

    /// <summary>
    /// 更新字典类型
    /// </summary>
    /// <param name="dto">更新参数</param>
    /// <returns>是否成功</returns>
    Task<bool> UpdateDictTypeAsync(UpdateDictTypeDto dto);

    /// <summary>
    /// 删除字典类型
    /// </summary>
    /// <param name="id">字典类型ID</param>
    /// <returns>是否成功</returns>
    Task<bool> DeleteDictTypeAsync(string id);

    #endregion

    #region 字典数据管理

    /// <summary>
    /// 获取字典数据列表
    /// </summary>
    /// <param name="query">查询参数</param>
    /// <returns>字典数据列表</returns>
    Task<List<DictDataDto>> GetDictDataListAsync(DictDataQueryDto query);

    /// <summary>
    /// 根据字典类型获取字典数据（用于前端下拉框等）
    /// </summary>
    /// <param name="dictType">字典类型</param>
    /// <returns>字典数据列表</returns>
    Task<List<DictDataDto>> GetDictDataByTypeAsync(string dictType);

    /// <summary>
    /// 获取所有字典类型及数据（用于前端缓存）
    /// </summary>
    /// <returns>字典类型及数据列表</returns>
    Task<List<DictTypeWithDataDto>> GetAllDictWithDataAsync();

    /// <summary>
    /// 获取字典数据详情
    /// </summary>
    /// <param name="id">字典数据ID</param>
    /// <returns>字典数据详情</returns>
    Task<DictDataDto> GetDictDataByIdAsync(string id);

    /// <summary>
    /// 创建字典数据
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新字典数据ID</returns>
    Task<string> CreateDictDataAsync(CreateDictDataDto dto);

    /// <summary>
    /// 更新字典数据
    /// </summary>
    /// <param name="dto">更新参数</param>
    /// <returns>是否成功</returns>
    Task<bool> UpdateDictDataAsync(UpdateDictDataDto dto);

    /// <summary>
    /// 删除字典数据
    /// </summary>
    /// <param name="id">字典数据ID</param>
    /// <returns>是否成功</returns>
    Task<bool> DeleteDictDataAsync(string id);

    #endregion
}