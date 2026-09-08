using EasyProduct.Business.Basic;
using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Basic.Dict;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EasyProduct.Web.Controllers.Admin.Basic;

/// <summary>
/// 字典管理控制器
/// </summary>
/// <remarks>
/// 提供字典类型和字典数据的增删改查功能
/// 管理端接口，需要 Admin JWT 认证
/// </remarks>
[ApiController]
[Route("api/admin/basic/dict")]
[Authorize(AuthenticationSchemes = "AdminJwt")]
public class DictController : BaseController
{
    private readonly IDictService _dictService;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="dictService">字典服务</param>
    public DictController(IDictService dictService)
    {
        _dictService = dictService;
    }

    #region 字典类型管理

    /// <summary>
    /// 获取字典类型列表
    /// </summary>
    /// <param name="dictName">字典名称（模糊搜索）</param>
    /// <param name="dictType">字典类型（模糊搜索）</param>
    /// <param name="status">状态：0=禁用，1=启用</param>
    /// <returns>字典类型列表</returns>
    /// <remarks>
    /// 获取字典类型的列表，支持按名称、类型、状态筛选
    /// </remarks>
    [HttpGet("type/list")]
    public async Task<ApiResponse<List<DictTypeDto>>> GetDictTypeList(
        [FromQuery] string? dictName,
        [FromQuery] string? dictType,
        [FromQuery] int? status)
    {
        var query = new DictTypeQueryDto
        {
            DictName = dictName,
            DictType = dictType,
            Status = status
        };

        var result = await _dictService.GetDictTypeListAsync(query);
        return Success(result);
    }

    /// <summary>
    /// 获取字典类型详情
    /// </summary>
    /// <param name="id">字典类型ID</param>
    /// <returns>字典类型详情</returns>
    /// <remarks>
    /// 根据ID获取字典类型的详细信息
    /// </remarks>
    [HttpGet("type/{id}")]
    public async Task<ApiResponse<DictTypeDto>> GetDictTypeById(string id)
    {
        var result = await _dictService.GetDictTypeByIdAsync(id);
        return Success(result!);
    }

    /// <summary>
    /// 创建字典类型
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新字典类型ID</returns>
    /// <remarks>
    /// 创建新的字典类型
    /// </remarks>
    [HttpPost("type")]
    public async Task<ApiResponse<string>> CreateDictType([FromBody] CreateDictTypeDto dto)
    {
        var result = await _dictService.CreateDictTypeAsync(dto);
        return Success(result, "字典类型创建成功");
    }

    /// <summary>
    /// 更新字典类型
    /// </summary>
    /// <param name="id">字典类型ID</param>
    /// <param name="dto">更新参数</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 更新字典类型信息
    /// </remarks>
    [HttpPut("type/{id}")]
    public async Task<ApiResponse<bool>> UpdateDictType(string id, [FromBody] UpdateDictTypeDto dto)
    {
        dto.Id = id;
        var result = await _dictService.UpdateDictTypeAsync(dto);
        return Success(result, "字典类型更新成功");
    }

    /// <summary>
    /// 删除字典类型
    /// </summary>
    /// <param name="id">字典类型ID</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 删除字典类型（软删除）
    /// 注意：如果字典类型下存在字典数据，则不能删除
    /// </remarks>
    [HttpDelete("type/{id}")]
    public async Task<ApiResponse<bool>> DeleteDictType(string id)
    {
        var result = await _dictService.DeleteDictTypeAsync(id);
        return Success(result, "字典类型删除成功");
    }

    #endregion

    #region 字典数据管理

    /// <summary>
    /// 获取字典数据列表
    /// </summary>
    /// <param name="dictType">字典类型（必填）</param>
    /// <param name="dictLabel">字典标签（模糊搜索）</param>
    /// <param name="status">状态：0=禁用，1=启用</param>
    /// <returns>字典数据列表</returns>
    /// <remarks>
    /// 获取指定字典类型下的字典数据列表
    /// </remarks>
    [HttpGet("data/list")]
    public async Task<ApiResponse<List<DictDataDto>>> GetDictDataList(
        [FromQuery] string dictType,
        [FromQuery] string? dictLabel,
        [FromQuery] int? status)
    {
        var query = new DictDataQueryDto
        {
            DictType = dictType,
            DictLabel = dictLabel,
            Status = status
        };

        var result = await _dictService.GetDictDataListAsync(query);
        return Success(result);
    }

    /// <summary>
    /// 根据字典类型获取字典数据（用于前端下拉框等）
    /// </summary>
    /// <param name="dictType">字典类型</param>
    /// <returns>字典数据列表（仅返回启用的数据）</returns>
    /// <remarks>
    /// 根据字典类型获取启用的字典数据，用于前端下拉框、单选框等场景
    /// </remarks>
    [HttpGet("data/type/{dictType}")]
    [AllowAnonymous] // 允许匿名访问，前端需要获取字典数据
    public async Task<ApiResponse<List<DictDataDto>>> GetDictDataByType(string dictType)
    {
        var result = await _dictService.GetDictDataByTypeAsync(dictType);
        return Success(result);
    }

    /// <summary>
    /// 获取所有字典类型及数据（用于前端缓存）
    /// </summary>
    /// <returns>字典类型及数据列表</returns>
    /// <remarks>
    /// 获取所有启用的字典类型及数据，用于前端初始化缓存
    /// </remarks>
    [HttpGet("data/all")]
    [AllowAnonymous] // 允许匿名访问，前端需要获取字典数据
    public async Task<ApiResponse<List<DictTypeWithDataDto>>> GetAllDictWithData()
    {
        var result = await _dictService.GetAllDictWithDataAsync();
        return Success(result);
    }

    /// <summary>
    /// 获取字典数据详情
    /// </summary>
    /// <param name="id">字典数据ID</param>
    /// <returns>字典数据详情</returns>
    /// <remarks>
    /// 根据ID获取字典数据的详细信息
    /// </remarks>
    [HttpGet("data/{id}")]
    public async Task<ApiResponse<DictDataDto>> GetDictDataById(string id)
    {
        var result = await _dictService.GetDictDataByIdAsync(id);
        return Success(result!);
    }

    /// <summary>
    /// 创建字典数据
    /// </summary>
    /// <param name="dto">创建参数</param>
    /// <returns>新字典数据ID</returns>
    /// <remarks>
    /// 创建新的字典数据
    /// </remarks>
    [HttpPost("data")]
    public async Task<ApiResponse<string>> CreateDictData([FromBody] CreateDictDataDto dto)
    {
        var result = await _dictService.CreateDictDataAsync(dto);
        return Success(result, "字典数据创建成功");
    }

    /// <summary>
    /// 更新字典数据
    /// </summary>
    /// <param name="id">字典数据ID</param>
    /// <param name="dto">更新参数</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 更新字典数据信息
    /// </remarks>
    [HttpPut("data/{id}")]
    public async Task<ApiResponse<bool>> UpdateDictData(string id, [FromBody] UpdateDictDataDto dto)
    {
        dto.Id = id;
        var result = await _dictService.UpdateDictDataAsync(dto);
        return Success(result, "字典数据更新成功");
    }

    /// <summary>
    /// 删除字典数据
    /// </summary>
    /// <param name="id">字典数据ID</param>
    /// <returns>是否成功</returns>
    /// <remarks>
    /// 删除字典数据（软删除）
    /// </remarks>
    [HttpDelete("data/{id}")]
    public async Task<ApiResponse<bool>> DeleteDictData(string id)
    {
        var result = await _dictService.DeleteDictDataAsync(id);
        return Success(result, "字典数据删除成功");
    }

    #endregion
}