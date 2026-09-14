-- ============================================
-- P4.4.5 库存预警建表脚本
-- 数据库：MySQL 8
-- 字符集：utf8mb4
-- 排序规则：utf8mb4_unicode_ci
-- 创建时间：2026-09-14
-- ============================================

USE easyproduct;

-- ============================================
-- 库存预警表（crm_stock_alert）
-- ============================================
DROP TABLE IF EXISTS `crm_stock_alert`;
CREATE TABLE `crm_stock_alert` (
    `id` VARCHAR(36) NOT NULL COMMENT '主键ID（GUID）',
    `warehouse_id` VARCHAR(36) NOT NULL COMMENT '仓库ID',
    `warehouse_name` VARCHAR(100) NOT NULL COMMENT '仓库名称（冗余字段）',
    `sku_code` VARCHAR(50) NOT NULL COMMENT 'SKU编码',
    `sku_name` VARCHAR(200) NOT NULL COMMENT 'SKU名称（冗余字段）',
    `spec` VARCHAR(200) NOT NULL DEFAULT '' COMMENT '规格',
    `available` INT NOT NULL DEFAULT 0 COMMENT '可用数量',
    `min_limit` INT NOT NULL DEFAULT 0 COMMENT '库存下限',
    `max_limit` INT NOT NULL DEFAULT 0 COMMENT '库存上限',
    `alert_type` VARCHAR(20) NOT NULL COMMENT '预警类型：Low=低库存，High=高库存',
    `status` VARCHAR(20) NOT NULL DEFAULT 'Pending' COMMENT '预警状态：Pending=待处理，Resolved=已解决',
    `resolved_at` DATETIME DEFAULT NULL COMMENT '解决时间',
    `remark` VARCHAR(500) DEFAULT NULL COMMENT '备注',
    `status` INT NOT NULL DEFAULT 1 COMMENT '状态：0=禁用，1=启用',
    `is_deleted` INT NOT NULL DEFAULT 0 COMMENT '是否删除：0=否，1=是',
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    `updated_at` DATETIME DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
    `created_by` VARCHAR(36) DEFAULT NULL COMMENT '创建人ID',
    `updated_by` VARCHAR(36) DEFAULT NULL COMMENT '更新人ID',
    PRIMARY KEY (`id`),
    KEY `idx_warehouse_id` (`warehouse_id`),
    KEY `idx_sku_code` (`sku_code`),
    KEY `idx_alert_type` (`alert_type`),
    KEY `idx_status` (`status`),
    KEY `idx_created_at` (`created_at`),
    CONSTRAINT `fk_stock_alert_warehouse` FOREIGN KEY (`warehouse_id`) REFERENCES `crm_warehouse` (`id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='库存预警表';

-- ============================================
-- 说明
-- ============================================
--
-- 表设计规范：
-- 1. 主键：GUID（VARCHAR(36)）
-- 2. 状态：使用 VARCHAR 枚举（Pending/Resolved）
-- 3. 软删除：is_deleted 字段
-- 4. 时间戳：created_at、updated_at、resolved_at
-- 5. 字符集：utf8mb4
-- 6. 排序规则：utf8mb4_unicode_ci
-- 7. 外键约束：仓库表使用 RESTRICT
-- 8. 索引：主键、外键、常用查询字段
--
-- 业务规则：
-- 1. 当库存低于下限或高于上限时自动创建预警
-- 2. 预警类型：Low（低库存）、High（高库存）
-- 3. 预警状态：Pending（待处理）、Resolved（已解决）
-- 4. 解决预警后记录解决时间
-- 5. 同一仓库同一 SKU 同类型预警只保留一条待处理记录
--
-- 数据关联：
-- - 仓库（crm_warehouse）：通过 warehouse_id 关联
-- - SKU（product_sku）：通过 sku_code 关联
-- - 库存账面（crm_stock）：预警来源于库存监控
--
-- 执行说明：
-- 1. 确保数据库 easyproduct 已创建
-- 2. 确保 crm_warehouse 表已创建
-- 3. 确保字符集为 utf8mb4
-- 4. 执行此脚本创建表
-- 5. 检查表和索引是否正确创建