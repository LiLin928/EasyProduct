using System;
using System.Threading.Tasks;
using EasyProduct.Business.Workflow;
using EasyProduct.Models.Dto.Workflow.Instance;
using Xunit;

namespace EasyProduct.Tests.Workflow
{
    /// <summary>
    /// 流程实例服务测试
    /// </summary>
    public class WorkflowInstanceServiceTests
    {
        /// <summary>
        /// 测试发起流程
        /// </summary>
        [Fact]
        public async Task Start_ValidRequest_ReturnsId()
        {
            // TODO: 实现测试
            await Task.CompletedTask;
        }

        /// <summary>
        /// 测试撤销流程
        /// </summary>
        [Fact]
        public async Task Cancel_RunningInstance_ReturnsTrue()
        {
            // TODO: 实现测试
            await Task.CompletedTask;
        }
    }
}
