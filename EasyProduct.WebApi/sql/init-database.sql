-- ============================================
-- EasyProduct 数据库初始化脚本
-- 数据库：MySQL 8
-- 字符集：utf8mb4
-- 排序规则：utf8mb4_unicode_ci
-- ============================================

-- 创建数据库（如果不存在）
CREATE DATABASE IF NOT EXISTS easyproduct
DEFAULT CHARACTER SET utf8mb4
COLLATE utf8mb4_unicode_ci;

USE easyproduct;

-- ============================================
-- Site 模块表（批次 3）
-- ============================================

-- -------------------------------------------
-- 1. 询价单表（site_inquiry）
-- -------------------------------------------
DROP TABLE IF EXISTS `site_inquiry`;
CREATE TABLE `site_inquiry` (
    `id` VARCHAR(36) NOT NULL COMMENT '主键ID（GUID）',
    `inquiry_no` VARCHAR(20) NOT NULL COMMENT '询价单号（格式：INQ + yyyyMMdd + 4位序号，如：INQ202609110001）',
    `company_name` VARCHAR(200) NOT NULL COMMENT '公司名称',
    `company_name_en` VARCHAR(200) DEFAULT NULL COMMENT '公司名称（英文）',
    `contact_name` VARCHAR(50) NOT NULL COMMENT '联系人姓名',
    `contact_name_en` VARCHAR(50) DEFAULT NULL COMMENT '联系人姓名（英文）',
    `phone` VARCHAR(20) NOT NULL COMMENT '联系电话',
    `email` VARCHAR(100) DEFAULT NULL COMMENT '电子邮箱',
    `country` VARCHAR(50) DEFAULT NULL COMMENT '国家',
    `province` VARCHAR(50) DEFAULT NULL COMMENT '省份',
    `city` VARCHAR(50) DEFAULT NULL COMMENT '城市',
    `address` VARCHAR(500) DEFAULT NULL COMMENT '详细地址',
    `address_en` VARCHAR(500) DEFAULT NULL COMMENT '详细地址（英文）',
    `remark` TEXT DEFAULT NULL COMMENT '备注信息',
    `status` INT NOT NULL DEFAULT 0 COMMENT '状态：0=待处理，1=已跟进，2=已转客户，3=已关闭',
    `customer_id` VARCHAR(36) DEFAULT NULL COMMENT '客户ID（转客户后关联 crm_customer）',
    `followed_at` DATETIME DEFAULT NULL COMMENT '跟进时间',
    `followed_by` VARCHAR(36) DEFAULT NULL COMMENT '跟进人ID',
    `converted_at` DATETIME DEFAULT NULL COMMENT '转客户时间',
    `converted_by` VARCHAR(36) DEFAULT NULL COMMENT '转客户操作人ID',
    `closed_at` DATETIME DEFAULT NULL COMMENT '关闭时间',
    `closed_by` VARCHAR(36) DEFAULT NULL COMMENT '关闭操作人ID',
    `close_reason` VARCHAR(500) DEFAULT NULL COMMENT '关闭原因',
    `is_deleted` INT NOT NULL DEFAULT 0 COMMENT '是否删除：0=否，1=是',
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    `updated_at` DATETIME DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
    `created_by` VARCHAR(36) DEFAULT NULL COMMENT '创建人ID',
    `updated_by` VARCHAR(36) DEFAULT NULL COMMENT '更新人ID',
    PRIMARY KEY (`id`),
    UNIQUE KEY `uk_inquiry_no` (`inquiry_no`),
    KEY `idx_status` (`status`),
    KEY `idx_customer_id` (`customer_id`),
    KEY `idx_created_at` (`created_at`),
    KEY `idx_phone` (`phone`),
    KEY `idx_email` (`email`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='询价单表';

-- -------------------------------------------
-- 2. 询价明细表（site_inquiry_item）
-- -------------------------------------------
DROP TABLE IF EXISTS `site_inquiry_item`;
CREATE TABLE `site_inquiry_item` (
    `id` VARCHAR(36) NOT NULL COMMENT '主键ID（GUID）',
    `inquiry_id` VARCHAR(36) NOT NULL COMMENT '询价单ID',
    `inquiry_no` VARCHAR(20) NOT NULL COMMENT '询价单号（冗余字段，方便查询）',
    `product_name` VARCHAR(200) NOT NULL COMMENT '产品名称',
    `product_name_en` VARCHAR(200) DEFAULT NULL COMMENT '产品名称（英文）',
    `product_code` VARCHAR(50) DEFAULT NULL COMMENT '产品编码',
    `specification` VARCHAR(200) DEFAULT NULL COMMENT '规格型号',
    `quantity` INT DEFAULT NULL COMMENT '数量',
    `unit` VARCHAR(20) DEFAULT NULL COMMENT '单位',
    `remark` TEXT DEFAULT NULL COMMENT '备注信息',
    `is_deleted` INT NOT NULL DEFAULT 0 COMMENT '是否删除：0=否，1=是',
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    `updated_at` DATETIME DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
    PRIMARY KEY (`id`),
    KEY `idx_inquiry_id` (`inquiry_id`),
    KEY `idx_inquiry_no` (`inquiry_no`),
    CONSTRAINT `fk_inquiry_item_inquiry` FOREIGN KEY (`inquiry_id`) REFERENCES `site_inquiry` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='询价明细表';

-- -------------------------------------------
-- 3. 留言表（site_contact）
-- -------------------------------------------
DROP TABLE IF EXISTS `site_contact`;
CREATE TABLE `site_contact` (
    `id` VARCHAR(36) NOT NULL COMMENT '主键ID（GUID）',
    `name` VARCHAR(50) NOT NULL COMMENT '姓名',
    `name_en` VARCHAR(50) DEFAULT NULL COMMENT '姓名（英文）',
    `phone` VARCHAR(20) DEFAULT NULL COMMENT '联系电话',
    `email` VARCHAR(100) DEFAULT NULL COMMENT '电子邮箱',
    `company` VARCHAR(200) DEFAULT NULL COMMENT '公司名称',
    `company_en` VARCHAR(200) DEFAULT NULL COMMENT '公司名称（英文）',
    `subject` VARCHAR(200) DEFAULT NULL COMMENT '主题',
    `subject_en` VARCHAR(200) DEFAULT NULL COMMENT '主题（英文）',
    `message` TEXT NOT NULL COMMENT '留言内容',
    `message_en` TEXT DEFAULT NULL COMMENT '留言内容（英文）',
    `status` INT NOT NULL DEFAULT 0 COMMENT '状态：0=未读，1=已读，2=已回复',
    `reply` TEXT DEFAULT NULL COMMENT '回复内容',
    `reply_en` TEXT DEFAULT NULL COMMENT '回复内容（英文）',
    `replied_at` DATETIME DEFAULT NULL COMMENT '回复时间',
    `replied_by` VARCHAR(36) DEFAULT NULL COMMENT '回复人ID',
    `is_deleted` INT NOT NULL DEFAULT 0 COMMENT '是否删除：0=否，1=是',
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    `updated_at` DATETIME DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
    PRIMARY KEY (`id`),
    KEY `idx_status` (`status`),
    KEY `idx_created_at` (`created_at`),
    KEY `idx_phone` (`phone`),
    KEY `idx_email` (`email`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='留言表';