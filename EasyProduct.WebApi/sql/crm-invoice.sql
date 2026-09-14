-- ============================================
-- P4.5.1 发票管理建表脚本
-- ============================================

USE easyproduct;

DROP TABLE IF EXISTS `crm_invoice`;
CREATE TABLE `crm_invoice` (
    `id` VARCHAR(36) NOT NULL COMMENT '主键ID（GUID）',
    `invoice_no` VARCHAR(20) NOT NULL COMMENT '发票编号',
    `type` VARCHAR(20) NOT NULL COMMENT '发票类型：Output=销项，Input=进项',
    `order_type` VARCHAR(20) NOT NULL COMMENT '订单类型：Sales=销售订单，Purchase=采购订单',
    `order_no` VARCHAR(20) NOT NULL COMMENT '订单编号',
    `party_name` VARCHAR(200) NOT NULL COMMENT '往来单位名称',
    `amount` DECIMAL(18, 2) NOT NULL DEFAULT 0.00 COMMENT '未税金额',
    `tax_rate` DECIMAL(5, 2) NOT NULL DEFAULT 0.00 COMMENT '税率',
    `tax_amount` DECIMAL(18, 2) NOT NULL DEFAULT 0.00 COMMENT '税额',
    `total` DECIMAL(18, 2) NOT NULL DEFAULT 0.00 COMMENT '价税合计',
    `issue_date` DATE NOT NULL COMMENT '开票日期',
    `status` VARCHAR(20) NOT NULL DEFAULT 'Draft' COMMENT '发票状态：Draft=草稿，Issued=已开具，Voided=已作废',
    `remark` VARCHAR(500) DEFAULT NULL COMMENT '备注',
    `status` INT NOT NULL DEFAULT 1 COMMENT '状态：0=禁用，1=启用',
    `is_deleted` INT NOT NULL DEFAULT 0 COMMENT '是否删除：0=否，1=是',
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    `updated_at` DATETIME DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
    `created_by` VARCHAR(36) DEFAULT NULL COMMENT '创建人ID',
    `updated_by` VARCHAR(36) DEFAULT NULL COMMENT '更新人ID',
    PRIMARY KEY (`id`),
    UNIQUE KEY `uk_invoice_no` (`invoice_no`),
    KEY `idx_type` (`type`),
    KEY `idx_order_type` (`order_type`),
    KEY `idx_order_no` (`order_no`),
    KEY `idx_party_name` (`party_name`),
    KEY `idx_status` (`status`),
    KEY `idx_issue_date` (`issue_date`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='发票表';