-- ============================================
-- P4.3 采购订单模块建表脚本
-- 数据库：MySQL 8
-- 字符集：utf8mb4
-- 排序规则：utf8mb4_unicode_ci
-- 创建时间：2026-09-14
-- ============================================

USE easyproduct;

-- ============================================
-- 1. 采购订单主表（crm_purchase_order）
-- ============================================
DROP TABLE IF EXISTS `crm_purchase_order`;
CREATE TABLE `crm_purchase_order` (
    `id` VARCHAR(36) NOT NULL COMMENT '主键ID（GUID）',
    `order_no` VARCHAR(20) NOT NULL COMMENT '订单编号（格式：PO-{year}-{sequence:04d}，如：PO-2026-0001）',
    `supplier_id` VARCHAR(36) NOT NULL COMMENT '供应商ID',
    `supplier_name` VARCHAR(100) NOT NULL COMMENT '供应商名称（冗余字段）',
    `buyer_name` VARCHAR(50) DEFAULT NULL COMMENT '采购员姓名',
    `currency_code` VARCHAR(10) NOT NULL DEFAULT 'CNY' COMMENT '币种代码',
    `currency_symbol` VARCHAR(10) NOT NULL DEFAULT '¥' COMMENT '币种符号',
    `payment_terms` VARCHAR(50) NOT NULL DEFAULT 'Net 30' COMMENT '付款条款',
    `delivery_date` DATE NOT NULL COMMENT '交货日期',
    `order_status` VARCHAR(20) NOT NULL DEFAULT 'Draft' COMMENT '订单状态：Draft=草稿，Confirmed=已确认，Received=已入库，Completed=已完成，Cancelled=已取消',
    `subtotal_amount` DECIMAL(18, 2) NOT NULL DEFAULT 0.00 COMMENT '小计金额',
    `tax_amount` DECIMAL(18, 2) NOT NULL DEFAULT 0.00 COMMENT '税额',
    `total_amount` DECIMAL(18, 2) NOT NULL DEFAULT 0.00 COMMENT '总金额',
    `remark` VARCHAR(500) DEFAULT NULL COMMENT '备注',
    `status` INT NOT NULL DEFAULT 1 COMMENT '状态：0=禁用，1=启用',
    `is_deleted` INT NOT NULL DEFAULT 0 COMMENT '是否删除：0=否，1=是',
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    `updated_at` DATETIME DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
    `created_by` VARCHAR(36) DEFAULT NULL COMMENT '创建人ID',
    `updated_by` VARCHAR(36) DEFAULT NULL COMMENT '更新人ID',
    PRIMARY KEY (`id`),
    UNIQUE KEY `uk_order_no` (`order_no`),
    KEY `idx_supplier_id` (`supplier_id`),
    KEY `idx_order_status` (`order_status`),
    KEY `idx_delivery_date` (`delivery_date`),
    KEY `idx_created_at` (`created_at`),
    CONSTRAINT `fk_purchase_order_supplier` FOREIGN KEY (`supplier_id`) REFERENCES `crm_supplier` (`id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='采购订单表';

-- ============================================
-- 2. 采购订单明细表（crm_purchase_order_item）
-- ============================================
DROP TABLE IF EXISTS `crm_purchase_order_item`;
CREATE TABLE `crm_purchase_order_item` (
    `id` VARCHAR(36) NOT NULL COMMENT '主键ID（GUID）',
    `order_id` VARCHAR(36) NOT NULL COMMENT '订单ID',
    `sku_code` VARCHAR(50) NOT NULL COMMENT 'SKU编码',
    `sku_name` VARCHAR(200) NOT NULL COMMENT 'SKU名称',
    `product_name` VARCHAR(200) NOT NULL COMMENT '产品名称',
    `spec` VARCHAR(200) DEFAULT NULL COMMENT '规格',
    `price` DECIMAL(18, 2) NOT NULL COMMENT '单价',
    `quantity` INT NOT NULL COMMENT '数量',
    `tax_rate_code` VARCHAR(10) NOT NULL COMMENT '税率代码',
    `tax_rate` DECIMAL(5, 2) NOT NULL COMMENT '税率（百分比）',
    `amount` DECIMAL(18, 2) NOT NULL COMMENT '金额（price × quantity）',
    `tax_amount` DECIMAL(18, 2) NOT NULL COMMENT '税额（amount × taxRate / 100）',
    `total_amount` DECIMAL(18, 2) NOT NULL COMMENT '总金额（amount + taxAmount）',
    `warehouse_id` VARCHAR(36) NOT NULL COMMENT '仓库ID',
    `status` INT NOT NULL DEFAULT 1 COMMENT '状态：0=禁用，1=启用',
    `is_deleted` INT NOT NULL DEFAULT 0 COMMENT '是否删除：0=否，1=是',
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    `updated_at` DATETIME DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
    `created_by` VARCHAR(36) DEFAULT NULL COMMENT '创建人ID',
    `updated_by` VARCHAR(36) DEFAULT NULL COMMENT '更新人ID',
    PRIMARY KEY (`id`),
    KEY `idx_order_id` (`order_id`),
    KEY `idx_sku_code` (`sku_code`),
    KEY `idx_warehouse_id` (`warehouse_id`),
    CONSTRAINT `fk_purchase_order_item_order` FOREIGN KEY (`order_id`) REFERENCES `crm_purchase_order` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='采购订单明细表';

-- ============================================
-- 说明
-- ============================================
--
-- 表设计规范：
-- 1. 主键：GUID（VARCHAR(36)）
-- 2. 订单编号：业务编码唯一（PO-{year}-{sequence:04d}）
-- 3. 状态：使用字符串枚举（Draft/Confirmed/Received/Completed/Cancelled）
-- 4. 软删除：is_deleted 字段
-- 5. 时间戳：created_at、updated_at
-- 6. 字符集：utf8mb4
-- 7. 排序规则：utf8mb4_unicode_ci
-- 8. 外键约束：供应商表使用 RESTRICT，订单明细表使用 CASCADE
-- 9. 索引：主键、唯一键、外键、常用查询字段
--
-- 状态流转规则：
-- - draft（草稿）→ confirmed（已确认）或 cancelled（已取消）
-- - confirmed（已确认）→ received（已入库）或 cancelled（已取消）
-- - received（已入库）→ completed（已完成）
-- - completed（已完成）→ 终态
-- - cancelled（已取消）→ 终态
--
-- 金额计算：
-- 明细金额：
-- - amount = price × quantity
-- - taxAmount = amount × taxRate / 100
-- - totalAmount = amount + taxAmount
--
-- 订单金额：
-- - subtotalAmount = sum(items.amount)
-- - taxAmount = sum(items.taxAmount)
-- - totalAmount = subtotalAmount + taxAmount
--
-- 业务规则：
-- 1. 仅草稿状态可修改、删除
-- 2. 入库时自动创建入库记录并增加库存
-- 3. 供应商名称冗余存储，避免频繁关联查询
--
-- ============================================