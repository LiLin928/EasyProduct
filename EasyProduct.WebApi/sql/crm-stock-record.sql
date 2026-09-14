-- ============================================
-- P4.4.3 出入库流水建表脚本
-- 数据库：MySQL 8
-- 字符集：utf8mb4
-- 排序规则：utf8mb4_unicode_ci
-- 创建时间：2026-09-14
-- ============================================

USE easyproduct;

-- ============================================
-- 出入库流水表（crm_stock_record）
-- ============================================
DROP TABLE IF EXISTS `crm_stock_record`;
CREATE TABLE `crm_stock_record` (
    `id` VARCHAR(36) NOT NULL COMMENT '主键ID（GUID）',
    `warehouse_id` VARCHAR(36) NOT NULL COMMENT '仓库ID',
    `warehouse_name` VARCHAR(100) NOT NULL COMMENT '仓库名称（冗余字段）',
    `sku_code` VARCHAR(50) NOT NULL COMMENT 'SKU编码',
    `sku_name` VARCHAR(200) NOT NULL COMMENT 'SKU名称（冗余字段）',
    `spec` VARCHAR(200) NOT NULL DEFAULT '' COMMENT '规格',
    `unit` VARCHAR(20) NOT NULL DEFAULT '个' COMMENT '单位',
    `type` VARCHAR(20) NOT NULL COMMENT '出入库类型：In=入库，Out=出库',
    `source_type` VARCHAR(50) NOT NULL COMMENT '流水来源类型：PurchaseIn=采购入库，SalesOut=销售出库，MallOut=商城出库，CheckAdjust=盘点调整，ReversalReturn=冲销退回',
    `source_order_no` VARCHAR(50) NOT NULL COMMENT '来源单据编号',
    `quantity` INT NOT NULL COMMENT '数量',
    `operator` VARCHAR(50) NOT NULL COMMENT '操作人',
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
    KEY `idx_type` (`type`),
    KEY `idx_source_type` (`source_type`),
    KEY `idx_source_order_no` (`source_order_no`),
    KEY `idx_created_at` (`created_at`),
    CONSTRAINT `fk_stock_record_warehouse` FOREIGN KEY (`warehouse_id`) REFERENCES `crm_warehouse` (`id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='出入库流水表';

-- ============================================
-- 说明
-- ============================================
--
-- 表设计规范：
-- 1. 主键：GUID（VARCHAR(36)）
-- 2. 状态：使用 VARCHAR 枚举（In/Out）
-- 3. 软删除：is_deleted 字段
-- 4. 时间戳：created_at、updated_at
-- 5. 字符集：utf8mb4
-- 6. 排序规则：utf8mb4_unicode_ci
-- 7. 外键约束：仓库表使用 RESTRICT
-- 8. 索引：主键、外键、常用查询字段
--
-- 业务规则：
-- 1. 流水记录不可修改、不可删除（审计追溯）
-- 2. 每次库存变动都会创建一条流水记录
-- 3. type 字段标识入库或出库
-- 4. source_type 字段标识流水来源
-- 5. source_order_no 字段记录来源单据编号
--
-- 数据关联：
-- - 仓库（crm_warehouse）：通过 warehouse_id 关联
-- - SKU（product_sku）：通过 sku_code 关联
-- - 库存账面（crm_stock）：流水记录影响库存数量
--
-- 执行说明：
-- 1. 确保数据库 easyproduct 已创建
-- 2. 确保 crm_warehouse 表已创建
-- 3. 确保字符集为 utf8mb4
-- 4. 执行此脚本创建表
-- 5. 检查表和索引是否正确创建