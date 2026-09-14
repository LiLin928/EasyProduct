-- ============================================
-- P4.4.4 盘点管理建表脚本
-- 数据库：MySQL 8
-- 字符集：utf8mb4
-- 排序规则：utf8mb4_unicode_ci
-- 创建时间：2026-09-14
-- ============================================

USE easyproduct;

-- ============================================
-- 盘点单主表（crm_stock_check）
-- ============================================
DROP TABLE IF EXISTS `crm_stock_check`;
CREATE TABLE `crm_stock_check` (
    `id` VARCHAR(36) NOT NULL COMMENT '主键ID（GUID）',
    `check_no` VARCHAR(20) NOT NULL COMMENT '盘点单编号（格式：SC-{year}-{sequence:04d}，如：SC-2026-0001）',
    `warehouse_id` VARCHAR(36) NOT NULL COMMENT '仓库ID',
    `warehouse_name` VARCHAR(100) NOT NULL COMMENT '仓库名称（冗余字段）',
    `checker` VARCHAR(50) NOT NULL COMMENT '盘点人',
    `check_date` DATE NOT NULL COMMENT '盘点日期',
    `status` VARCHAR(20) NOT NULL DEFAULT 'Draft' COMMENT '盘点状态：Draft=草稿，Counting=盘点中，Completed=已完成',
    `remark` VARCHAR(500) DEFAULT NULL COMMENT '备注',
    `status` INT NOT NULL DEFAULT 1 COMMENT '状态：0=禁用，1=启用',
    `is_deleted` INT NOT NULL DEFAULT 0 COMMENT '是否删除：0=否，1=是',
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    `updated_at` DATETIME DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
    `created_by` VARCHAR(36) DEFAULT NULL COMMENT '创建人ID',
    `updated_by` VARCHAR(36) DEFAULT NULL COMMENT '更新人ID',
    PRIMARY KEY (`id`),
    UNIQUE KEY `uk_check_no` (`check_no`),
    KEY `idx_warehouse_id` (`warehouse_id`),
    KEY `idx_status` (`status`),
    KEY `idx_check_date` (`check_date`),
    KEY `idx_created_at` (`created_at`),
    CONSTRAINT `fk_stock_check_warehouse` FOREIGN KEY (`warehouse_id`) REFERENCES `crm_warehouse` (`id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='盘点单表';

-- ============================================
-- 盘点明细表（crm_stock_check_item）
-- ============================================
DROP TABLE IF EXISTS `crm_stock_check_item`;
CREATE TABLE `crm_stock_check_item` (
    `id` VARCHAR(36) NOT NULL COMMENT '主键ID（GUID）',
    `check_id` VARCHAR(36) NOT NULL COMMENT '盘点单ID',
    `sku_code` VARCHAR(50) NOT NULL COMMENT 'SKU编码',
    `sku_name` VARCHAR(200) NOT NULL COMMENT 'SKU名称（冗余字段）',
    `spec` VARCHAR(200) NOT NULL DEFAULT '' COMMENT '规格',
    `unit` VARCHAR(20) NOT NULL DEFAULT '个' COMMENT '单位',
    `system_qty` INT NOT NULL DEFAULT 0 COMMENT '系统数量',
    `counted_qty` INT NOT NULL DEFAULT 0 COMMENT '实盘数量',
    `diff` INT NOT NULL DEFAULT 0 COMMENT '盘点差异（counted_qty - system_qty）',
    `status` INT NOT NULL DEFAULT 1 COMMENT '状态：0=禁用，1=启用',
    `is_deleted` INT NOT NULL DEFAULT 0 COMMENT '是否删除：0=否，1=是',
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    `updated_at` DATETIME DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
    `created_by` VARCHAR(36) DEFAULT NULL COMMENT '创建人ID',
    `updated_by` VARCHAR(36) DEFAULT NULL COMMENT '更新人ID',
    PRIMARY KEY (`id`),
    KEY `idx_check_id` (`check_id`),
    KEY `idx_sku_code` (`sku_code`),
    CONSTRAINT `fk_stock_check_item_check` FOREIGN KEY (`check_id`) REFERENCES `crm_stock_check` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='盘点明细表';

-- ============================================
-- 说明
-- ============================================
--
-- 表设计规范：
-- 1. 主键：GUID（VARCHAR(36)）
-- 2. 盘点单编号：业务编码唯一（SC-{year}-{sequence:04d}）
-- 3. 状态：使用 VARCHAR 枚举（Draft/Counting/Completed）
-- 4. 软删除：is_deleted 字段
-- 5. 时间戳：created_at、updated_at
-- 6. 字符集：utf8mb4
-- 7. 排序规则：utf8mb4_unicode_ci
-- 8. 外键约束：仓库表使用 RESTRICT，盘点明细表使用 CASCADE
-- 9. 索引：主键、唯一键、外键、常用查询字段
--
-- 状态流转规则：
-- - draft（草稿）→ counting（盘点中）
-- - counting（盘点中）→ completed（已完成）
-- - completed（已完成）→ 终态
--
-- 业务规则：
-- 1. 盘点单编号格式：SC-{year}-{sequence:04d}
-- 2. 创建盘点单时自动生成明细（从库存账面）
-- 3. 完成盘点时根据差异调整库存
-- 4. 仅草稿状态可删除
--
-- 数据关联：
-- - 仓库（crm_warehouse）：通过 warehouse_id 关联
-- - SKU（product_sku）：通过 sku_code 关联
-- - 库存账面（crm_stock）：完成盘点时调整库存
-- - 出入库流水（crm_stock_record）：完成盘点时创建流水
--
-- 执行说明：
-- 1. 确保数据库 easyproduct 已创建
-- 2. 确保 crm_warehouse 表已创建
-- 3. 确保字符集为 utf8mb4
-- 4. 执行此脚本创建表
-- 5. 检查表和索引是否正确创建