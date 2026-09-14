-- ============================================
-- P4.4.1 仓库管理建表脚本
-- 数据库：MySQL 8
-- 字符集：utf8mb4
-- 排序规则：utf8mb4_unicode_ci
-- 创建时间：2026-09-14
-- ============================================

USE easyproduct;

-- ============================================
-- 仓库表（crm_warehouse）
-- ============================================
DROP TABLE IF EXISTS `crm_warehouse`;
CREATE TABLE `crm_warehouse` (
    `id` VARCHAR(36) NOT NULL COMMENT '主键ID（GUID）',
    `code` VARCHAR(20) NOT NULL COMMENT '仓库编码（唯一，如：WH001）',
    `name` VARCHAR(100) NOT NULL COMMENT '仓库名称',
    `address` VARCHAR(200) NOT NULL COMMENT '仓库地址',
    `manager` VARCHAR(50) NOT NULL COMMENT '负责人姓名',
    `phone` VARCHAR(20) NOT NULL COMMENT '联系电话',
    `remark` VARCHAR(500) DEFAULT NULL COMMENT '备注',
    `status` INT NOT NULL DEFAULT 1 COMMENT '状态：0=停用，1=启用',
    `is_deleted` INT NOT NULL DEFAULT 0 COMMENT '是否删除：0=否，1=是',
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    `updated_at` DATETIME DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
    `created_by` VARCHAR(36) DEFAULT NULL COMMENT '创建人ID',
    `updated_by` VARCHAR(36) DEFAULT NULL COMMENT '更新人ID',
    PRIMARY KEY (`id`),
    UNIQUE KEY `uk_code` (`code`),
    KEY `idx_name` (`name`),
    KEY `idx_status` (`status`),
    KEY `idx_created_at` (`created_at`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='仓库表';

-- ============================================
-- 说明
-- ============================================
--
-- 表设计规范：
-- 1. 主键：GUID（VARCHAR(36)）
-- 2. 仓库编码：业务编码唯一（WH001、WH002...）
-- 3. 状态：使用 INT 枚举（0=停用，1=启用）
-- 4. 软删除：is_deleted 字段
-- 5. 时间戳：created_at、updated_at
-- 6. 字符集：utf8mb4
-- 7. 排序规则：utf8mb4_unicode_ci
-- 8. 索引：主键、唯一键、常用查询字段
--
-- 业务规则：
-- 1. 仓库编码必须唯一，不可重复
-- 2. 仓库可以关联库存账面、出入库流水、盘点单等
-- 3. 删除仓库前需要检查是否有关联数据
-- 4. 仅启用状态的仓库会出现在下拉选项中
--
-- 数据关联：
-- - 库存账面（crm_stock）：一个仓库可以有多个库存记录
-- - 出入库流水（crm_stock_record）：记录仓库的出入库历史
-- - 盘点单（crm_stock_check）：对仓库库存进行盘点
--
-- 执行说明：
-- 1. 确保数据库 easyproduct 已创建
-- 2. 确保字符集为 utf8mb4
-- 3. 执行此脚本创建表
-- 4. 检查表和索引是否正确创建