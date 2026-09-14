-- ============================================
-- P4.4.2 库存账面建表脚本
-- 数据库：MySQL 8
-- 字符集：utf8mb4
-- 排序规则：utf8mb4_unicode_ci
-- 创建时间：2026-09-14
-- ============================================

USE easyproduct;

-- ============================================
-- 库存账面表（crm_stock）
-- ============================================
DROP TABLE IF EXISTS `crm_stock`;
CREATE TABLE `crm_stock` (
    `id` VARCHAR(36) NOT NULL COMMENT '主键ID（GUID）',
    `warehouse_id` VARCHAR(36) NOT NULL COMMENT '仓库ID',
    `warehouse_name` VARCHAR(100) NOT NULL COMMENT '仓库名称（冗余字段）',
    `sku_code` VARCHAR(50) NOT NULL COMMENT 'SKU编码',
    `sku_name` VARCHAR(200) NOT NULL COMMENT 'SKU名称（冗余字段）',
    `spec` VARCHAR(200) NOT NULL DEFAULT '' COMMENT '规格',
    `unit` VARCHAR(20) NOT NULL DEFAULT '个' COMMENT '单位',
    `available` INT NOT NULL DEFAULT 0 COMMENT '可用数量',
    `locked` INT NOT NULL DEFAULT 0 COMMENT '锁定数量',
    `total` INT NOT NULL DEFAULT 0 COMMENT '总数量',
    `min_limit` INT NOT NULL DEFAULT 0 COMMENT '库存下限',
    `max_limit` INT NOT NULL DEFAULT 0 COMMENT '库存上限',
    `status` INT NOT NULL DEFAULT 1 COMMENT '状态：0=禁用，1=启用',
    `is_deleted` INT NOT NULL DEFAULT 0 COMMENT '是否删除：0=否，1=是',
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    `updated_at` DATETIME DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
    `created_by` VARCHAR(36) DEFAULT NULL COMMENT '创建人ID',
    `updated_by` VARCHAR(36) DEFAULT NULL COMMENT '更新人ID',
    PRIMARY KEY (`id`),
    UNIQUE KEY `uk_warehouse_sku` (`warehouse_id`, `sku_code`),
    KEY `idx_warehouse_id` (`warehouse_id`),
    KEY `idx_sku_code` (`sku_code`),
    KEY `idx_available` (`available`),
    KEY `idx_updated_at` (`updated_at`),
    CONSTRAINT `fk_stock_warehouse` FOREIGN KEY (`warehouse_id`) REFERENCES `crm_warehouse` (`id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='库存账面表';

-- ============================================
-- 说明
-- ============================================
--
-- 表设计规范：
-- 1. 主键：GUID（VARCHAR(36)）
-- 2. 唯一约束：同一仓库同一 SKU 只有一条库存记录
-- 3. 状态：使用 INT 枚举（0=禁用，1=启用）
-- 4. 软删除：is_deleted 字段
-- 5. 时间戳：created_at、updated_at
-- 6. 字符集：utf8mb4
-- 7. 排序规则：utf8mb4_unicode_ci
-- 8. 外键约束：仓库表使用 RESTRICT
-- 9. 索引：主键、唯一键、外键、常用查询字段
--
-- 业务规则：
-- 1. total = available + locked（总数量 = 可用数量 + 锁定数量）
-- 2. 库存预警：available 低于 min_limit 或高于 max_limit 时触发预警
-- 3. 库存调整：通过盘点或其他方式调整库存时更新此表
-- 4. 同一仓库同一 SKU 只能有一条库存记录（唯一约束）
--
-- 数据关联：
-- - 仓库（crm_warehouse）：通过 warehouse_id 关联
-- - SKU（product_sku）：通过 sku_code 关联
-- - 出入库流水（crm_stock_record）：记录库存变动历史
-- - 盘点明细（crm_stock_check_item）：盘点时对比库存
--
-- 执行说明：
-- 1. 确保数据库 easyproduct 已创建
-- 2. 确保 crm_warehouse 表已创建
-- 3. 确保字符集为 utf8mb4
-- 4. 执行此脚本创建表
-- 5. 检查表和索引是否正确创建