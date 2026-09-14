-- ============================================
-- CRM 主数据管理模块建表脚本
-- 数据库：MySQL 8
-- 字符集：utf8mb4
-- 排序规则：utf8mb4_unicode_ci
-- 创建时间：2026-09-14
-- ============================================

USE easyproduct;

-- ============================================
-- 1. 客户管理
-- ============================================

-- -------------------------------------------
-- 1.1 客户表（crm_customer）
-- -------------------------------------------
DROP TABLE IF EXISTS `crm_customer`;
CREATE TABLE `crm_customer` (
    `id` VARCHAR(36) NOT NULL COMMENT '主键ID（GUID）',
    `code` VARCHAR(20) NOT NULL COMMENT '客户编码（格式：C + yyyyMM + 4位序号，如：C2026090001）',
    `name` VARCHAR(100) NOT NULL COMMENT '客户名称',
    `type` INT NOT NULL DEFAULT 1 COMMENT '客户类型：1=B2B（企业客户），2=零售（个人客户）',
    `source` INT NOT NULL DEFAULT 3 COMMENT '客户来源：1=询价转化，2=注册建档，3=手动建档',
    `contact_name` VARCHAR(50) DEFAULT NULL COMMENT '联系人姓名',
    `contact_phone` VARCHAR(20) DEFAULT NULL COMMENT '联系电话',
    `contact_email` VARCHAR(100) DEFAULT NULL COMMENT '联系邮箱',
    `address` VARCHAR(500) DEFAULT NULL COMMENT '地址',
    `business_user_id` VARCHAR(36) DEFAULT NULL COMMENT '归属业务员ID（关联 basic_user）',
    `remark` VARCHAR(500) DEFAULT NULL COMMENT '备注',
    `status` INT NOT NULL DEFAULT 1 COMMENT '状态：0=禁用，1=启用',
    `is_deleted` INT NOT NULL DEFAULT 0 COMMENT '是否删除：0=否，1=是',
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    `updated_at` DATETIME DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
    `created_by` VARCHAR(36) DEFAULT NULL COMMENT '创建人ID',
    `updated_by` VARCHAR(36) DEFAULT NULL COMMENT '更新人ID',
    PRIMARY KEY (`id`),
    UNIQUE KEY `uk_code` (`code`),
    KEY `idx_name` (`name`),
    KEY `idx_type` (`type`),
    KEY `idx_source` (`source`),
    KEY `idx_status` (`status`),
    KEY `idx_business_user_id` (`business_user_id`),
    KEY `idx_created_at` (`created_at`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='客户表';

-- -------------------------------------------
-- 1.2 客户联系人表（crm_customer_contact）
-- -------------------------------------------
DROP TABLE IF EXISTS `crm_customer_contact`;
CREATE TABLE `crm_customer_contact` (
    `id` VARCHAR(36) NOT NULL COMMENT '主键ID（GUID）',
    `customer_id` VARCHAR(36) NOT NULL COMMENT '客户ID',
    `name` VARCHAR(50) NOT NULL COMMENT '联系人姓名',
    `phone` VARCHAR(20) DEFAULT NULL COMMENT '联系电话',
    `email` VARCHAR(100) DEFAULT NULL COMMENT '邮箱',
    `position` VARCHAR(50) DEFAULT NULL COMMENT '职位',
    `is_primary` INT NOT NULL DEFAULT 0 COMMENT '是否主要联系人：0=否，1=是',
    `status` INT NOT NULL DEFAULT 1 COMMENT '状态：0=禁用，1=启用',
    `is_deleted` INT NOT NULL DEFAULT 0 COMMENT '是否删除：0=否，1=是',
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    `updated_at` DATETIME DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
    `created_by` VARCHAR(36) DEFAULT NULL COMMENT '创建人ID',
    `updated_by` VARCHAR(36) DEFAULT NULL COMMENT '更新人ID',
    PRIMARY KEY (`id`),
    KEY `idx_customer_id` (`customer_id`),
    KEY `idx_is_primary` (`is_primary`),
    KEY `idx_created_at` (`created_at`),
    CONSTRAINT `fk_contact_customer` FOREIGN KEY (`customer_id`) REFERENCES `crm_customer` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='客户联系人表';

-- -------------------------------------------
-- 1.3 客户地址表（crm_customer_address）
-- -------------------------------------------
DROP TABLE IF EXISTS `crm_customer_address`;
CREATE TABLE `crm_customer_address` (
    `id` VARCHAR(36) NOT NULL COMMENT '主键ID（GUID）',
    `customer_id` VARCHAR(36) NOT NULL COMMENT '客户ID',
    `receiver_name` VARCHAR(50) NOT NULL COMMENT '收货人姓名',
    `phone` VARCHAR(20) NOT NULL COMMENT '联系电话',
    `province` VARCHAR(50) NOT NULL COMMENT '省份',
    `city` VARCHAR(50) NOT NULL COMMENT '城市',
    `district` VARCHAR(50) DEFAULT NULL COMMENT '区县',
    `detail_address` VARCHAR(200) NOT NULL COMMENT '详细地址',
    `is_default` INT NOT NULL DEFAULT 0 COMMENT '是否默认地址：0=否，1=是',
    `status` INT NOT NULL DEFAULT 1 COMMENT '状态：0=禁用，1=启用',
    `is_deleted` INT NOT NULL DEFAULT 0 COMMENT '是否删除：0=否，1=是',
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    `updated_at` DATETIME DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
    `created_by` VARCHAR(36) DEFAULT NULL COMMENT '创建人ID',
    `updated_by` VARCHAR(36) DEFAULT NULL COMMENT '更新人ID',
    PRIMARY KEY (`id`),
    KEY `idx_customer_id` (`customer_id`),
    KEY `idx_is_default` (`is_default`),
    KEY `idx_created_at` (`created_at`),
    CONSTRAINT `fk_address_customer` FOREIGN KEY (`customer_id`) REFERENCES `crm_customer` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='客户地址表';

-- ============================================
-- 2. 供应商管理
-- ============================================

-- -------------------------------------------
-- 2.1 供应商表（crm_supplier）
-- -------------------------------------------
DROP TABLE IF EXISTS `crm_supplier`;
CREATE TABLE `crm_supplier` (
    `id` VARCHAR(36) NOT NULL COMMENT '主键ID（GUID）',
    `code` VARCHAR(20) NOT NULL COMMENT '供应商编码（格式：S + yyyyMM + 4位序号，如：S2026090001）',
    `name` VARCHAR(100) NOT NULL COMMENT '供应商名称',
    `contact_name` VARCHAR(50) DEFAULT NULL COMMENT '联系人姓名',
    `contact_phone` VARCHAR(20) DEFAULT NULL COMMENT '联系电话',
    `contact_email` VARCHAR(100) DEFAULT NULL COMMENT '联系邮箱',
    `address` VARCHAR(500) DEFAULT NULL COMMENT '地址',
    `bank_name` VARCHAR(100) DEFAULT NULL COMMENT '开户行',
    `bank_account` VARCHAR(50) DEFAULT NULL COMMENT '银行账号',
    `remark` VARCHAR(500) DEFAULT NULL COMMENT '备注',
    `status` INT NOT NULL DEFAULT 1 COMMENT '状态：0=禁用，1=启用',
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
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='供应商表';

-- -------------------------------------------
-- 2.2 供应商资质表（crm_supplier_qualification）
-- -------------------------------------------
DROP TABLE IF EXISTS `crm_supplier_qualification`;
CREATE TABLE `crm_supplier_qualification` (
    `id` VARCHAR(36) NOT NULL COMMENT '主键ID（GUID）',
    `supplier_id` VARCHAR(36) NOT NULL COMMENT '供应商ID',
    `type` INT NOT NULL DEFAULT 1 COMMENT '资质类型：1=营业执照，2=生产许可证，3=质量认证',
    `name` VARCHAR(100) NOT NULL COMMENT '资质名称',
    `certificate_no` VARCHAR(50) DEFAULT NULL COMMENT '证书编号',
    `issue_date` DATE DEFAULT NULL COMMENT '发证日期',
    `expire_date` DATE DEFAULT NULL COMMENT '有效期',
    `image_url` VARCHAR(500) DEFAULT NULL COMMENT '证件图片URL',
    `qualification_status` INT NOT NULL DEFAULT 1 COMMENT '资质状态：0=过期，1=有效',
    `status` INT NOT NULL DEFAULT 1 COMMENT '状态：0=禁用，1=启用',
    `is_deleted` INT NOT NULL DEFAULT 0 COMMENT '是否删除：0=否，1=是',
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    `updated_at` DATETIME DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
    `created_by` VARCHAR(36) DEFAULT NULL COMMENT '创建人ID',
    `updated_by` VARCHAR(36) DEFAULT NULL COMMENT '更新人ID',
    PRIMARY KEY (`id`),
    KEY `idx_supplier_id` (`supplier_id`),
    KEY `idx_type` (`type`),
    KEY `idx_qualification_status` (`qualification_status`),
    KEY `idx_expire_date` (`expire_date`),
    KEY `idx_created_at` (`created_at`),
    CONSTRAINT `fk_qualification_supplier` FOREIGN KEY (`supplier_id`) REFERENCES `crm_supplier` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='供应商资质表';

-- ============================================
-- 3. 币种管理
-- ============================================

-- -------------------------------------------
-- 3.1 币种表（crm_currency）
-- -------------------------------------------
DROP TABLE IF EXISTS `crm_currency`;
CREATE TABLE `crm_currency` (
    `id` VARCHAR(36) NOT NULL COMMENT '主键ID（GUID）',
    `code` VARCHAR(10) NOT NULL COMMENT '币种代码（如：CNY、USD、EUR）',
    `name` VARCHAR(50) NOT NULL COMMENT '币种名称（如：人民币、美元、欧元）',
    `symbol` VARCHAR(10) NOT NULL COMMENT '符号（如：¥、$、€）',
    `exchange_rate` DECIMAL(18, 6) NOT NULL DEFAULT 1.000000 COMMENT '汇率（对人民币）',
    `is_default` INT NOT NULL DEFAULT 0 COMMENT '是否默认币种：0=否，1=是',
    `status` INT NOT NULL DEFAULT 1 COMMENT '状态：0=禁用，1=启用',
    `is_deleted` INT NOT NULL DEFAULT 0 COMMENT '是否删除：0=否，1=是',
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    `updated_at` DATETIME DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
    `created_by` VARCHAR(36) DEFAULT NULL COMMENT '创建人ID',
    `updated_by` VARCHAR(36) DEFAULT NULL COMMENT '更新人ID',
    PRIMARY KEY (`id`),
    UNIQUE KEY `uk_code` (`code`),
    KEY `idx_name` (`name`),
    KEY `idx_is_default` (`is_default`),
    KEY `idx_status` (`status`),
    KEY `idx_created_at` (`created_at`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='币种表';

-- ============================================
-- 4. 税率管理
-- ============================================

-- -------------------------------------------
-- 4.1 税率表（crm_tax_rate）
-- -------------------------------------------
DROP TABLE IF EXISTS `crm_tax_rate`;
CREATE TABLE `crm_tax_rate` (
    `id` VARCHAR(36) NOT NULL COMMENT '主键ID（GUID）',
    `name` VARCHAR(50) NOT NULL COMMENT '税率名称（如：13% 税率、9% 税率）',
    `rate` DECIMAL(5, 4) NOT NULL COMMENT '税率（如：0.13 表示 13%）',
    `description` VARCHAR(200) DEFAULT NULL COMMENT '描述',
    `status` INT NOT NULL DEFAULT 1 COMMENT '状态：0=禁用，1=启用',
    `is_deleted` INT NOT NULL DEFAULT 0 COMMENT '是否删除：0=否，1=是',
    `created_at` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    `updated_at` DATETIME DEFAULT NULL ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
    `created_by` VARCHAR(36) DEFAULT NULL COMMENT '创建人ID',
    `updated_by` VARCHAR(36) DEFAULT NULL COMMENT '更新人ID',
    PRIMARY KEY (`id`),
    KEY `idx_name` (`name`),
    KEY `idx_rate` (`rate`),
    KEY `idx_status` (`status`),
    KEY `idx_created_at` (`created_at`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='税率表';

-- ============================================
-- 5. 初始数据
-- ============================================

-- -------------------------------------------
-- 5.1 币种初始数据
-- -------------------------------------------
INSERT INTO `crm_currency` (`id`, `code`, `name`, `symbol`, `exchange_rate`, `is_default`, `status`, `created_at`)
VALUES
    (UUID(), 'CNY', '人民币', '¥', 1.000000, 1, 1, NOW()),
    (UUID(), 'USD', '美元', '$', 7.200000, 0, 1, NOW()),
    (UUID(), 'EUR', '欧元', '€', 7.800000, 0, 1, NOW()),
    (UUID(), 'GBP', '英镑', '£', 9.100000, 0, 1, NOW()),
    (UUID(), 'JPY', '日元', '¥', 0.048000, 0, 1, NOW());

-- -------------------------------------------
-- 5.2 税率初始数据
-- -------------------------------------------
INSERT INTO `crm_tax_rate` (`id`, `name`, `rate`, `description`, `status`, `created_at`)
VALUES
    (UUID(), '13% 税率', 0.1300, '适用于一般货物、加工、修理修配劳务、有形动产租赁服务', 1, NOW()),
    (UUID(), '9% 税率', 0.0900, '适用于农产品、图书、报纸、杂志、饲料、化肥、农药、农机等', 1, NOW()),
    (UUID(), '6% 税率', 0.0600, '适用于现代服务、生活服务、无形资产等', 1, NOW()),
    (UUID(), '免税', 0.0000, '适用于出口货物、免税货物等', 1, NOW()),
    (UUID(), '3% 征收率', 0.0300, '适用于小规模纳税人', 1, NOW());

-- ============================================
-- 说明
-- ============================================
--
-- 表设计规范：
-- 1. 主键：GUID（VARCHAR(36)），避免自增ID的分布式问题
-- 2. 编码：业务编码唯一，如客户编码（C + yyyyMM + 序号）、供应商编码（S + yyyyMM + 序号）
-- 3. 状态：使用 INT 类型，0=禁用，1=启用
-- 4. 软删除：is_deleted 字段，0=未删除，1=已删除
-- 5. 时间戳：created_at（创建时间）、updated_at（更新时间）
-- 6. 创建人：created_by、updated_by（GUID 字符串）
-- 7. 字符集：utf8mb4（支持emoji等特殊字符）
-- 8. 排序规则：utf8mb4_unicode_ci（支持多语言排序）
-- 9. 外键约束：级联删除（ON DELETE CASCADE）
-- 10. 索引：主键、唯一键、常用查询字段、外键字段
--
-- 客户管理说明：
-- - 客户编码：自动生成，格式为 C + yyyyMM + 4位序号
-- - 客户类型：B2B（企业客户）、零售（个人客户）
-- - 客户来源：询价转化、注册建档、手动建档
-- - 联系人：支持多个联系人，可设置主要联系人
-- - 地址：支持多个地址，可设置默认地址
--
-- 供应商管理说明：
-- - 供应商编码：自动生成，格式为 S + yyyyMM + 4位序号
-- - 资质管理：支持营业执照、生产许可证、质量认证等
-- - 资质状态：根据有效期自动判断（有效/过期）
--
-- 币种管理说明：
-- - 默认币种：人民币（CNY）
-- - 汇率：对人民币的汇率
-- - 唯一约束：币种代码唯一
-- - 默认币种互斥：系统只能有一个默认币种
--
-- 税率管理说明：
-- - 税率值：DECIMAL(5,4)，如 0.1300 表示 13%
-- - 常见税率：13%（一般货物）、9%（农产品）、6%（服务）、0%（免税）
--
-- ============================================