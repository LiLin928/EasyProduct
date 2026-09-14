using System;
using System.Threading.Tasks;
using EasyProduct.Common.Base;
using EasyProduct.Models.Dto.Workflow.Definition;

namespace EasyProduct.Business.Workflow
{
    /// <summary>
    /// 流程定义服务接口
    /// </summary>
    public interface IWorkflowDefinitionService
    {
        /// <summary>
        /// 获取流程定义列表（分页）
        /// </summary>
        /// <param name="query">查询参数</param>
        /// <returns>流程定义分页结果</returns>
        Task<PageResponse<DefinitionDto>> GetListAsync(DefinitionQuery query);

        /// <summary>
        /// 获取流程定义详情
        /// </summary>
        /// <param name="id">流程定义ID</param>
        /// <returns>流程定义详情</returns>
        Task<DefinitionDto> GetByIdAsync(Guid id);

        /// <summary>
        /// 创建流程定义
        /// </summary>
        /// <param name="dto">创建参数</param>
        /// <returns>新流程定义ID</returns>
        Task<string> CreateAsync(DefinitionCreateDto dto);

        /// <summary>
        /// 更新流程定义
        /// </summary>
        /// <param name="id">流程定义ID</param>
        /// <param name="dto">更新参数</param>
        /// <returns>是否成功</returns>
        Task<bool> UpdateAsync(Guid id, DefinitionUpdateDto dto);

        /// <summary>
        /// 删除流程定义
        /// </summary>
        /// <param name="id">流程定义ID</param>
        /// <returns>是否成功</returns>
        Task<bool> DeleteAsync(Guid id);

        /// <summary>
        /// 发布流程定义
        /// </summary>
        /// <param name="id">流程定义ID</param>
        /// <returns>是否成功</returns>
        Task<bool> PublishAsync(Guid id);
    }
}