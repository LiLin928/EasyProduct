-- ========================================
-- EasyProduct 数据库初始化脚本
-- 创建日期: 2026-09-07
-- 数据库: easyproduct
-- 字符集: utf8mb4
-- ========================================

-- 创建数据库
CREATE DATABASE IF NOT EXISTS `easyproduct`
DEFAULT CHARACTER SET utf8mb4
COLLATE utf8mb4_unicode_ci;

USE `easyproduct`;

-- ========================================
-- Basic 模块表
-- ========================================

-- ----------------------------
-- 1. 部门表 (basic_dept)
-- ----------------------------
DROP TABLE IF EXISTS `basic_dept`;
CREATE TABLE `basic_dept` (
  `id` CHAR(36) NOT NULL COMMENT '主键ID',
  `parent_id` CHAR(36) DEFAULT NULL COMMENT '父部门ID',
  `name` VARCHAR(50) NOT NULL COMMENT '部门名称',
  `code` VARCHAR(50) DEFAULT NULL COMMENT '部门编码',
  `sort` INT DEFAULT 0 COMMENT '排序',
  `status` INT DEFAULT 1 COMMENT '状态：0=禁用，1=启用',
  `create_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `update_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  `create_by` VARCHAR(50) DEFAULT NULL COMMENT '创建人',
  `is_deleted` TINYINT(1) DEFAULT 0 COMMENT '软删除标记：0=未删除，1=已删除',
  PRIMARY KEY (`id`),
  KEY `idx_parent_id` (`parent_id`),
  KEY `idx_status` (`status`),
  KEY `idx_is_deleted` (`is_deleted`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='部门表';

-- ----------------------------
-- 2. 用户表 (basic_user)
-- ----------------------------
DROP TABLE IF EXISTS `basic_user`;
CREATE TABLE `basic_user` (
  `id` CHAR(36) NOT NULL COMMENT '主键ID',
  `dept_id` CHAR(36) DEFAULT NULL COMMENT '部门ID',
  `username` VARCHAR(50) NOT NULL COMMENT '用户名',
  `password` VARCHAR(255) NOT NULL COMMENT '密码（BCrypt加密）',
  `real_name` VARCHAR(50) DEFAULT NULL COMMENT '真实姓名',
  `email` VARCHAR(100) DEFAULT NULL COMMENT '邮箱',
  `phone` VARCHAR(20) DEFAULT NULL COMMENT '手机号',
  `avatar` VARCHAR(255) DEFAULT NULL COMMENT '头像URL',
  `status` INT DEFAULT 1 COMMENT '状态：0=禁用，1=启用',
  `create_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `update_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  `create_by` VARCHAR(50) DEFAULT NULL COMMENT '创建人',
  `is_deleted` TINYINT(1) DEFAULT 0 COMMENT '软删除标记：0=未删除，1=已删除',
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_username` (`username`),
  KEY `idx_dept_id` (`dept_id`),
  KEY `idx_status` (`status`),
  KEY `idx_is_deleted` (`is_deleted`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='用户表';

-- ----------------------------
-- 3. 角色表 (basic_role)
-- ----------------------------
DROP TABLE IF EXISTS `basic_role`;
CREATE TABLE `basic_role` (
  `id` CHAR(36) NOT NULL COMMENT '主键ID',
  `name` VARCHAR(50) NOT NULL COMMENT '角色名称',
  `code` VARCHAR(50) NOT NULL COMMENT '角色编码',
  `description` VARCHAR(255) DEFAULT NULL COMMENT '描述',
  `status` INT DEFAULT 1 COMMENT '状态：0=禁用，1=启用',
  `create_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `update_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  `create_by` VARCHAR(50) DEFAULT NULL COMMENT '创建人',
  `is_deleted` TINYINT(1) DEFAULT 0 COMMENT '软删除标记：0=未删除，1=已删除',
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_code` (`code`),
  KEY `idx_status` (`status`),
  KEY `idx_is_deleted` (`is_deleted`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='角色表';

-- ----------------------------
-- 4. 用户角色关联表 (basic_user_role)
-- ----------------------------
DROP TABLE IF EXISTS `basic_user_role`;
CREATE TABLE `basic_user_role` (
  `id` CHAR(36) NOT NULL COMMENT '主键ID',
  `user_id` CHAR(36) NOT NULL COMMENT '用户ID',
  `role_id` CHAR(36) NOT NULL COMMENT '角色ID',
  `create_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `update_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  `create_by` VARCHAR(50) DEFAULT NULL COMMENT '创建人',
  `is_deleted` TINYINT(1) DEFAULT 0 COMMENT '软删除标记：0=未删除，1=已删除',
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_user_role` (`user_id`, `role_id`),
  KEY `idx_user_id` (`user_id`),
  KEY `idx_role_id` (`role_id`),
  KEY `idx_is_deleted` (`is_deleted`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='用户角色关联表';

-- ----------------------------
-- 5. 菜单表 (basic_menu)
-- ----------------------------
DROP TABLE IF EXISTS `basic_menu`;
CREATE TABLE `basic_menu` (
  `id` CHAR(36) NOT NULL COMMENT '主键ID',
  `parent_id` CHAR(36) DEFAULT '0' COMMENT '父菜单ID（0表示根菜单）',
  `name` VARCHAR(50) NOT NULL COMMENT '菜单名称（路由name）',
  `path` VARCHAR(255) DEFAULT NULL COMMENT '路由路径',
  `component` VARCHAR(255) DEFAULT NULL COMMENT '组件路径',
  `title_key` VARCHAR(100) DEFAULT NULL COMMENT '标题i18n key',
  `icon` VARCHAR(50) DEFAULT NULL COMMENT '图标名称',
  `sort` INT DEFAULT 0 COMMENT '排序',
  `visible` INT DEFAULT 1 COMMENT '是否可见：0=否，1=是',
  `status` INT DEFAULT 1 COMMENT '状态：0=禁用，1=启用',
  `permission` VARCHAR(100) DEFAULT NULL COMMENT '权限标识',
  `create_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `update_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  `create_by` VARCHAR(50) DEFAULT NULL COMMENT '创建人',
  `is_deleted` TINYINT(1) DEFAULT 0 COMMENT '软删除标记：0=未删除，1=已删除',
  PRIMARY KEY (`id`),
  KEY `idx_parent_id` (`parent_id`),
  KEY `idx_status` (`status`),
  KEY `idx_visible` (`visible`),
  KEY `idx_is_deleted` (`is_deleted`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='菜单表';

-- ----------------------------
-- 6. 角色菜单关联表 (basic_role_menu)
-- ----------------------------
DROP TABLE IF EXISTS `basic_role_menu`;
CREATE TABLE `basic_role_menu` (
  `id` CHAR(36) NOT NULL COMMENT '主键ID',
  `role_id` CHAR(36) NOT NULL COMMENT '角色ID',
  `menu_id` CHAR(36) NOT NULL COMMENT '菜单ID',
  `create_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `update_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  `create_by` VARCHAR(50) DEFAULT NULL COMMENT '创建人',
  `is_deleted` TINYINT(1) DEFAULT 0 COMMENT '软删除标记：0=未删除，1=已删除',
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_role_menu` (`role_id`, `menu_id`),
  KEY `idx_role_id` (`role_id`),
  KEY `idx_menu_id` (`menu_id`),
  KEY `idx_is_deleted` (`is_deleted`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='角色菜单关联表';

-- ========================================
-- Ops 模块表
-- ========================================

-- ----------------------------
-- 7. 操作日志表 (ops_operate_log)
-- ----------------------------
DROP TABLE IF EXISTS `ops_operate_log`;
CREATE TABLE `ops_operate_log` (
  `id` CHAR(36) NOT NULL COMMENT '主键ID',
  `module` VARCHAR(50) DEFAULT NULL COMMENT '模块名称',
  `target` VARCHAR(100) DEFAULT NULL COMMENT '操作对象',
  `action` VARCHAR(50) DEFAULT NULL COMMENT '操作动作',
  `method` VARCHAR(10) DEFAULT NULL COMMENT 'HTTP方法',
  `url` VARCHAR(500) DEFAULT NULL COMMENT '请求URL',
  `params` TEXT COMMENT '请求参数',
  `ip` VARCHAR(50) DEFAULT NULL COMMENT 'IP地址',
  `user_agent` VARCHAR(500) DEFAULT NULL COMMENT '用户代理',
  `user_id` CHAR(36) DEFAULT NULL COMMENT '用户ID',
  `user_name` VARCHAR(50) DEFAULT NULL COMMENT '用户名',
  `status` INT DEFAULT 1 COMMENT '操作状态：0=失败，1=成功',
  `error_msg` TEXT COMMENT '错误信息',
  `duration` INT DEFAULT NULL COMMENT '耗时（毫秒）',
  `trace_id` VARCHAR(50) DEFAULT NULL COMMENT '追踪ID',
  `create_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `update_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  `create_by` VARCHAR(50) DEFAULT NULL COMMENT '创建人',
  `is_deleted` TINYINT(1) DEFAULT 0 COMMENT '软删除标记：0=未删除，1=已删除',
  PRIMARY KEY (`id`),
  KEY `idx_module` (`module`),
  KEY `idx_user_id` (`user_id`),
  KEY `idx_status` (`status`),
  KEY `idx_create_time` (`create_time`),
  KEY `idx_is_deleted` (`is_deleted`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='操作日志表';

-- ----------------------------
-- 8. 登录日志表 (ops_login_log)
-- ----------------------------
DROP TABLE IF EXISTS `ops_login_log`;
CREATE TABLE `ops_login_log` (
  `id` CHAR(36) NOT NULL COMMENT '主键ID',
  `user_id` CHAR(36) DEFAULT NULL COMMENT '用户ID',
  `user_name` VARCHAR(50) DEFAULT NULL COMMENT '用户名',
  `login_type` VARCHAR(20) DEFAULT NULL COMMENT '登录类型：login/logout',
  `ip` VARCHAR(50) DEFAULT NULL COMMENT 'IP地址',
  `location` VARCHAR(100) DEFAULT NULL COMMENT '登录地点',
  `browser` VARCHAR(100) DEFAULT NULL COMMENT '浏览器',
  `os` VARCHAR(100) DEFAULT NULL COMMENT '操作系统',
  `status` INT DEFAULT 1 COMMENT '登录状态：0=失败，1=成功',
  `error_msg` TEXT COMMENT '错误信息',
  `trace_id` VARCHAR(50) DEFAULT NULL COMMENT '追踪ID',
  `create_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `update_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  `create_by` VARCHAR(50) DEFAULT NULL COMMENT '创建人',
  `is_deleted` TINYINT(1) DEFAULT 0 COMMENT '软删除标记：0=未删除，1=已删除',
  PRIMARY KEY (`id`),
  KEY `idx_user_id` (`user_id`),
  KEY `idx_status` (`status`),
  KEY `idx_create_time` (`create_time`),
  KEY `idx_is_deleted` (`is_deleted`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='登录日志表';

-- ========================================
-- Product 模块表
-- ========================================

-- ----------------------------
-- 商品分类表 (product_category)
-- ----------------------------
DROP TABLE IF EXISTS `product_category`;
CREATE TABLE `product_category` (
  `id` CHAR(36) NOT NULL COMMENT '主键ID',
  `parent_id` CHAR(36) DEFAULT '0' COMMENT '父分类ID（根节点为0）',
  `category_name` VARCHAR(100) NOT NULL COMMENT '分类名称',
  `category_code` VARCHAR(50) DEFAULT NULL COMMENT '分类编码',
  `icon` VARCHAR(255) DEFAULT NULL COMMENT '分类图标',
  `image` VARCHAR(500) DEFAULT NULL COMMENT '分类图片',
  `sort` INT DEFAULT 0 COMMENT '排序',
  `level` INT DEFAULT 1 COMMENT '层级',
  `full_path` VARCHAR(500) DEFAULT NULL COMMENT '完整路径',
  `show_in_nav` INT DEFAULT 1 COMMENT '是否显示在导航：0=否，1=是',
  `status` INT DEFAULT 1 COMMENT '状态：0=禁用，1=启用',
  `create_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `update_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  `create_by` VARCHAR(50) DEFAULT NULL COMMENT '创建人',
  `is_deleted` TINYINT(1) DEFAULT 0 COMMENT '软删除标记：0=未删除，1=已删除',
  PRIMARY KEY (`id`),
  KEY `idx_parent_id` (`parent_id`),
  KEY `idx_sort` (`sort`),
  KEY `idx_status` (`status`),
  KEY `idx_is_deleted` (`is_deleted`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='商品分类表';

-- ----------------------------
-- 商品主档表 (product_spu)
-- ----------------------------
DROP TABLE IF EXISTS `product_spu`;
CREATE TABLE `product_spu` (
  `id` CHAR(36) NOT NULL COMMENT '主键ID',
  `spu_name` VARCHAR(200) NOT NULL COMMENT '商品名称',
  `spu_code` VARCHAR(50) DEFAULT NULL COMMENT '商品编码',
  `category_id` CHAR(36) DEFAULT NULL COMMENT '分类ID',
  `main_image` VARCHAR(500) DEFAULT NULL COMMENT '主图URL',
  `images` TEXT COMMENT '商品图集（JSON数组）',
  `description` TEXT COMMENT '商品描述（富文本）',
  `unit` VARCHAR(20) DEFAULT '件' COMMENT '计量单位',
  `spu_type` INT DEFAULT 1 COMMENT '商品类型：1=实物，2=虚拟，3=票品',
  `brand` VARCHAR(100) DEFAULT NULL COMMENT '品牌',
  `spec_template` TEXT COMMENT '规格模板（JSON）',
  `status` INT DEFAULT 0 COMMENT '状态：0=下架，1=上架',
  `sort` INT DEFAULT 0 COMMENT '排序',
  `create_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `update_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  `create_by` VARCHAR(50) DEFAULT NULL COMMENT '创建人',
  `is_deleted` TINYINT(1) DEFAULT 0 COMMENT '软删除标记：0=未删除，1=已删除',
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_spu_code` (`spu_code`),
  KEY `idx_category_id` (`category_id`),
  KEY `idx_spu_type` (`spu_type`),
  KEY `idx_status` (`status`),
  KEY `idx_is_deleted` (`is_deleted`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='商品主档表';

-- ----------------------------
-- 商品SKU表 (product_sku)
-- ----------------------------
DROP TABLE IF EXISTS `product_sku`;
CREATE TABLE `product_sku` (
  `id` CHAR(36) NOT NULL COMMENT '主键ID',
  `spu_id` CHAR(36) NOT NULL COMMENT '商品SPU ID',
  `sku_name` VARCHAR(200) NOT NULL COMMENT 'SKU名称',
  `sku_code` VARCHAR(50) DEFAULT NULL COMMENT 'SKU编码',
  `barcode` VARCHAR(50) DEFAULT NULL COMMENT '条码',
  `spec_json` TEXT COMMENT '规格组合JSON',
  `price` DECIMAL(18,2) NOT NULL COMMENT '零售价',
  `member_price` DECIMAL(18,2) DEFAULT NULL COMMENT '会员价',
  `wholesale_price` DECIMAL(18,2) DEFAULT NULL COMMENT '批发价',
  `cost_price` DECIMAL(18,2) DEFAULT NULL COMMENT '成本价',
  `stock` INT DEFAULT 0 COMMENT '库存数量',
  `status` INT DEFAULT 1 COMMENT '状态：0=禁用，1=启用',
  `create_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `update_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  `create_by` VARCHAR(50) DEFAULT NULL COMMENT '创建人',
  `is_deleted` TINYINT(1) DEFAULT 0 COMMENT '软删除标记：0=未删除，1=已删除',
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_sku_code` (`sku_code`),
  KEY `idx_spu_id` (`spu_id`),
  KEY `idx_status` (`status`),
  KEY `idx_is_deleted` (`is_deleted`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='商品SKU表';

-- ----------------------------
-- 商品图集表 (product_image)
-- ----------------------------
DROP TABLE IF EXISTS `product_image`;
CREATE TABLE `product_image` (
  `id` CHAR(36) NOT NULL COMMENT '主键ID',
  `spu_id` CHAR(36) NOT NULL COMMENT '商品ID',
  `image_url` VARCHAR(500) NOT NULL COMMENT '图片URL',
  `thumbnail_url` VARCHAR(500) DEFAULT NULL COMMENT '缩略图URL',
  `image_name` VARCHAR(200) DEFAULT NULL COMMENT '图片名称',
  `image_size` INT DEFAULT NULL COMMENT '图片大小（字节）',
  `image_type` VARCHAR(50) DEFAULT NULL COMMENT '图片类型（jpg/png/webp等）',
  `sort` INT DEFAULT 0 COMMENT '排序',
  `is_main` TINYINT(1) DEFAULT 0 COMMENT '是否主图：0=否，1=是',
  `create_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `update_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  `create_by` VARCHAR(50) DEFAULT NULL COMMENT '创建人',
  `is_deleted` TINYINT(1) DEFAULT 0 COMMENT '软删除标记：0=未删除，1=已删除',
  PRIMARY KEY (`id`),
  KEY `idx_spu_id` (`spu_id`),
  KEY `idx_sort` (`sort`),
  KEY `idx_is_main` (`is_main`),
  KEY `idx_is_deleted` (`is_deleted`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='商品图集表';

-- ----------------------------
-- 商品渠道发布表 (product_channel)
-- ----------------------------
DROP TABLE IF EXISTS `product_channel`;
CREATE TABLE `product_channel` (
  `id` CHAR(36) NOT NULL COMMENT '主键ID',
  `spu_id` CHAR(36) NOT NULL COMMENT '商品ID',
  `channel_code` VARCHAR(20) NOT NULL COMMENT '渠道编码：site=官网，miniapp=小程序，b2b=B2B',
  `status` INT DEFAULT 1 COMMENT '上架状态：0=下架，1=上架',
  `sort` INT DEFAULT 0 COMMENT '渠道排序',
  `price` DECIMAL(18,2) DEFAULT NULL COMMENT '渠道价格（可选，为空则使用SKU价格）',
  `show_price` TINYINT(1) DEFAULT 1 COMMENT '是否显示价格：0=否，1=是',
  `show_stock` TINYINT(1) DEFAULT 1 COMMENT '是否显示库存：0=否，1=是',
  `publish_time` DATETIME DEFAULT NULL COMMENT '发布时间',
  `unpublish_time` DATETIME DEFAULT NULL COMMENT '下架时间',
  `create_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `update_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  `create_by` VARCHAR(50) DEFAULT NULL COMMENT '创建人',
  `is_deleted` TINYINT(1) DEFAULT 0 COMMENT '软删除标记：0=未删除，1=已删除',
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_spu_channel` (`spu_id`, `channel_code`),
  KEY `idx_channel_code` (`channel_code`),
  KEY `idx_status` (`status`),
  KEY `idx_is_deleted` (`is_deleted`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='商品渠道发布表';

-- ========================================
-- Basic 扩展表（字典、公告、系统参数）
-- ========================================

-- ----------------------------
-- 9. 字典类型表 (basic_dict_type)
-- ----------------------------
DROP TABLE IF EXISTS `basic_dict_type`;
CREATE TABLE `basic_dict_type` (
  `id` CHAR(36) NOT NULL COMMENT '主键ID',
  `dict_name` VARCHAR(100) NOT NULL COMMENT '字典名称',
  `dict_type` VARCHAR(100) NOT NULL COMMENT '字典类型（唯一标识）',
  `status` INT DEFAULT 1 COMMENT '状态：0=禁用，1=启用',
  `remark` VARCHAR(500) DEFAULT NULL COMMENT '备注',
  `create_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `update_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  `create_by` VARCHAR(50) DEFAULT NULL COMMENT '创建人',
  `is_deleted` TINYINT(1) DEFAULT 0 COMMENT '软删除标记：0=未删除，1=已删除',
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_dict_type` (`dict_type`),
  KEY `idx_status` (`status`),
  KEY `idx_is_deleted` (`is_deleted`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='字典类型表';

-- ----------------------------
-- 10. 字典数据表 (basic_dict_data)
-- ----------------------------
DROP TABLE IF EXISTS `basic_dict_data`;
CREATE TABLE `basic_dict_data` (
  `id` CHAR(36) NOT NULL COMMENT '主键ID',
  `dict_type` VARCHAR(100) NOT NULL COMMENT '字典类型',
  `dict_label` VARCHAR(100) NOT NULL COMMENT '字典标签',
  `dict_value` VARCHAR(100) NOT NULL COMMENT '字典值',
  `sort` INT DEFAULT 0 COMMENT '排序',
  `status` INT DEFAULT 1 COMMENT '状态：0=禁用，1=启用',
  `remark` VARCHAR(500) DEFAULT NULL COMMENT '备注',
  `create_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `update_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  `create_by` VARCHAR(50) DEFAULT NULL COMMENT '创建人',
  `is_deleted` TINYINT(1) DEFAULT 0 COMMENT '软删除标记：0=未删除，1=已删除',
  PRIMARY KEY (`id`),
  KEY `idx_dict_type` (`dict_type`),
  KEY `idx_sort` (`sort`),
  KEY `idx_status` (`status`),
  KEY `idx_is_deleted` (`is_deleted`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='字典数据表';

-- ----------------------------
-- 11. 公告表 (basic_notice)
-- ----------------------------
DROP TABLE IF EXISTS `basic_notice`;
CREATE TABLE `basic_notice` (
  `id` CHAR(36) NOT NULL COMMENT '主键ID',
  `notice_title` VARCHAR(200) NOT NULL COMMENT '公告标题',
  `notice_content` TEXT NOT NULL COMMENT '公告内容（富文本）',
  `notice_type` INT NOT NULL DEFAULT 1 COMMENT '公告类型：1=通知，2=公告',
  `status` INT DEFAULT 1 COMMENT '状态：0=禁用，1=启用',
  `top_flag` INT DEFAULT 0 COMMENT '是否置顶：0=否，1=是',
  `publish_time` DATETIME DEFAULT NULL COMMENT '发布时间',
  `create_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `update_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  `create_by` VARCHAR(50) DEFAULT NULL COMMENT '创建人',
  `is_deleted` TINYINT(1) DEFAULT 0 COMMENT '软删除标记：0=未删除，1=已删除',
  PRIMARY KEY (`id`),
  KEY `idx_notice_type` (`notice_type`),
  KEY `idx_status` (`status`),
  KEY `idx_top_flag` (`top_flag`),
  KEY `idx_publish_time` (`publish_time`),
  KEY `idx_is_deleted` (`is_deleted`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='公告表';

-- ----------------------------
-- 12. 系统参数表 (basic_config)
-- ----------------------------
DROP TABLE IF EXISTS `basic_config`;
CREATE TABLE `basic_config` (
  `id` CHAR(36) NOT NULL COMMENT '主键ID',
  `config_name` VARCHAR(100) NOT NULL COMMENT '参数名称',
  `config_key` VARCHAR(100) NOT NULL COMMENT '参数键名',
  `config_value` VARCHAR(500) NOT NULL COMMENT '参数键值',
  `config_type` INT DEFAULT 1 COMMENT '系统内置：0=否，1=是',
  `remark` VARCHAR(500) DEFAULT NULL COMMENT '备注',
  `create_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `update_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  `create_by` VARCHAR(50) DEFAULT NULL COMMENT '创建人',
  `is_deleted` TINYINT(1) DEFAULT 0 COMMENT '软删除标记：0=未删除，1=已删除',
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_config_key` (`config_key`),
  KEY `idx_config_type` (`config_type`),
  KEY `idx_is_deleted` (`is_deleted`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='系统参数表';

-- ========================================
-- 完成提示
-- ========================================
SELECT '数据库初始化完成' AS message;

-- ========================================
-- Mall 模块表 - 会员相关
-- ========================================

-- ----------------------------
-- 会员等级表 (mall_member_level)
-- ----------------------------
DROP TABLE IF EXISTS `mall_member_level`;
CREATE TABLE `mall_member_level` (
  `id` CHAR(36) NOT NULL COMMENT '主键ID',
  `level_name` VARCHAR(50) NOT NULL COMMENT '等级名称',
  `level_code` VARCHAR(50) NOT NULL COMMENT '等级编码',
  `level` INT NOT NULL COMMENT '等级数值',
  `min_points` INT DEFAULT 0 COMMENT '最低积分要求',
  `max_points` INT DEFAULT NULL COMMENT '最高积分上限（NULL表示无上限）',
  `discount_rate` DECIMAL(3,2) DEFAULT 1.00 COMMENT '折扣率(0.00-1.00)',
  `icon` VARCHAR(255) DEFAULT NULL COMMENT '等级图标',
  `status` INT DEFAULT 1 COMMENT '状态：0=禁用，1=启用',
  `create_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `update_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  `create_by` VARCHAR(50) DEFAULT NULL COMMENT '创建人',
  `is_deleted` TINYINT(1) DEFAULT 0 COMMENT '软删除标记：0=未删除，1=已删除',
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_level_code` (`level_code`),
  KEY `idx_level` (`level`),
  KEY `idx_status` (`status`),
  KEY `idx_is_deleted` (`is_deleted`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='会员等级表';

-- ----------------------------
-- 会员主表 (mall_member)
-- ----------------------------
DROP TABLE IF EXISTS `mall_member`;
CREATE TABLE `mall_member` (
  `id` CHAR(36) NOT NULL COMMENT '主键ID',
  `openid` VARCHAR(100) DEFAULT NULL COMMENT '微信OpenID',
  `unionid` VARCHAR(100) DEFAULT NULL COMMENT '微信UnionID',
  `nickname` VARCHAR(100) DEFAULT NULL COMMENT '昵称',
  `avatar` VARCHAR(500) DEFAULT NULL COMMENT '头像URL',
  `gender` INT DEFAULT 0 COMMENT '性别：0=未知，1=男，2=女',
  `phone` VARCHAR(20) DEFAULT NULL COMMENT '手机号',
  `real_name` VARCHAR(50) DEFAULT NULL COMMENT '真实姓名',
  `birthday` DATE DEFAULT NULL COMMENT '生日',
  `email` VARCHAR(100) DEFAULT NULL COMMENT '邮箱',
  `level_id` CHAR(36) DEFAULT NULL COMMENT '会员等级ID',
  `points` INT DEFAULT 0 COMMENT '当前积分',
  `total_points` INT DEFAULT 0 COMMENT '累计积分',
  `balance` DECIMAL(18,2) DEFAULT 0.00 COMMENT '账户余额',
  `status` INT DEFAULT 1 COMMENT '状态：0=禁用，1=启用',
  `last_login_time` DATETIME DEFAULT NULL COMMENT '最后登录时间',
  `last_login_ip` VARCHAR(50) DEFAULT NULL COMMENT '最后登录IP',
  `create_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `update_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  `create_by` VARCHAR(50) DEFAULT NULL COMMENT '创建人',
  `is_deleted` TINYINT(1) DEFAULT 0 COMMENT '软删除标记：0=未删除，1=已删除',
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_openid` (`openid`),
  KEY `idx_unionid` (`unionid`),
  KEY `idx_phone` (`phone`),
  KEY `idx_level_id` (`level_id`),
  KEY `idx_status` (`status`),
  KEY `idx_is_deleted` (`is_deleted`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='会员主表';

-- ----------------------------
-- 积分记录表 (mall_points_record)
-- ----------------------------
DROP TABLE IF EXISTS `mall_points_record`;
CREATE TABLE `mall_points_record` (
  `id` CHAR(36) NOT NULL COMMENT '主键ID',
  `member_id` CHAR(36) NOT NULL COMMENT '会员ID',
  `points_type` INT NOT NULL COMMENT '积分类型：1=消费获得，2=订单使用，3=后台调整，4=签到，5=注册赠送',
  `points` INT NOT NULL COMMENT '积分变动(正数为获得，负数为使用)',
  `balance` INT NOT NULL COMMENT '变动后余额',
  `order_no` VARCHAR(50) DEFAULT NULL COMMENT '关联订单号',
  `remark` VARCHAR(255) DEFAULT NULL COMMENT '备注',
  `create_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `update_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  `create_by` VARCHAR(50) DEFAULT NULL COMMENT '创建人',
  `is_deleted` TINYINT(1) DEFAULT 0 COMMENT '软删除标记：0=未删除，1=已删除',
  PRIMARY KEY (`id`),
  KEY `idx_member_id` (`member_id`),
  KEY `idx_points_type` (`points_type`),
  KEY `idx_order_no` (`order_no`),
  KEY `idx_create_time` (`create_time`),
  KEY `idx_is_deleted` (`is_deleted`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='积分记录表';

-- ----------------------------
-- 收藏表 (mall_favorite)
-- ----------------------------
DROP TABLE IF EXISTS `mall_favorite`;
CREATE TABLE `mall_favorite` (
  `id` CHAR(36) NOT NULL COMMENT '主键ID',
  `member_id` CHAR(36) NOT NULL COMMENT '会员ID',
  `spu_id` CHAR(36) NOT NULL COMMENT '商品SPU ID',
  `create_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `update_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  `create_by` VARCHAR(50) DEFAULT NULL COMMENT '创建人',
  `is_deleted` TINYINT(1) DEFAULT 0 COMMENT '软删除标记：0=未删除，1=已删除',
  PRIMARY KEY (`id`),
  UNIQUE KEY `uk_member_spu` (`member_id`, `spu_id`),
  KEY `idx_spu_id` (`spu_id`),
  KEY `idx_is_deleted` (`is_deleted`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='收藏表';

-- ========================================
-- Site 模块表
-- ========================================

-- ----------------------------
-- 新闻分类表 (site_news_category)
-- ----------------------------
DROP TABLE IF EXISTS `site_news_category`;
CREATE TABLE `site_news_category` (
  `id` CHAR(36) NOT NULL COMMENT '主键ID',
  `category_name` VARCHAR(100) NOT NULL COMMENT '分类名称',
  `category_code` VARCHAR(50) DEFAULT NULL COMMENT '分类编码',
  `sort` INT DEFAULT 0 COMMENT '排序',
  `status` INT DEFAULT 1 COMMENT '状态：0=禁用，1=启用',
  `create_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `update_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  `create_by` VARCHAR(50) DEFAULT NULL COMMENT '创建人',
  `is_deleted` TINYINT(1) DEFAULT 0 COMMENT '软删除标记：0=未删除，1=已删除',
  PRIMARY KEY (`id`),
  KEY `idx_sort` (`sort`),
  KEY `idx_status` (`status`),
  KEY `idx_is_deleted` (`is_deleted`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='新闻分类表';

-- ----------------------------
-- 新闻表 (site_news)
-- ----------------------------
DROP TABLE IF EXISTS `site_news`;
CREATE TABLE `site_news` (
  `id` CHAR(36) NOT NULL COMMENT '主键ID',
  `category_id` CHAR(36) DEFAULT NULL COMMENT '分类ID',
  `title` VARCHAR(200) NOT NULL COMMENT '新闻标题',
  `title_en` VARCHAR(200) DEFAULT NULL COMMENT '新闻标题（英文）',
  `summary` VARCHAR(500) DEFAULT NULL COMMENT '摘要',
  `summary_en` VARCHAR(500) DEFAULT NULL COMMENT '摘要（英文）',
  `content` LONGTEXT COMMENT '内容（富文本）',
  `content_en` LONGTEXT COMMENT '内容（英文，富文本）',
  `cover_image` VARCHAR(500) DEFAULT NULL COMMENT '封面图片URL',
  `author` VARCHAR(50) DEFAULT NULL COMMENT '作者',
  `source` VARCHAR(100) DEFAULT NULL COMMENT '来源',
  `view_count` INT DEFAULT 0 COMMENT '浏览次数',
  `is_top` INT DEFAULT 0 COMMENT '是否置顶：0=否，1=是',
  `publish_time` DATETIME DEFAULT NULL COMMENT '发布时间',
  `status` INT DEFAULT 1 COMMENT '状态：0=禁用，1=启用',
  `create_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `update_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  `create_by` VARCHAR(50) DEFAULT NULL COMMENT '创建人',
  `is_deleted` TINYINT(1) DEFAULT 0 COMMENT '软删除标记：0=未删除，1=已删除',
  PRIMARY KEY (`id`),
  KEY `idx_category_id` (`category_id`),
  KEY `idx_status` (`status`),
  KEY `idx_is_top` (`is_top`),
  KEY `idx_publish_time` (`publish_time`),
  KEY `idx_is_deleted` (`is_deleted`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='新闻表';

-- ----------------------------
-- Banner表 (site_banner)
-- ----------------------------
DROP TABLE IF EXISTS `site_banner`;
CREATE TABLE `site_banner` (
  `id` CHAR(36) NOT NULL COMMENT '主键ID',
  `title` VARCHAR(100) DEFAULT NULL COMMENT 'Banner标题',
  `title_en` VARCHAR(100) DEFAULT NULL COMMENT 'Banner标题（英文）',
  `image_url` VARCHAR(500) NOT NULL COMMENT '图片URL',
  `link_url` VARCHAR(500) DEFAULT NULL COMMENT '跳转链接',
  `target` VARCHAR(10) DEFAULT '_self' COMMENT '打开方式：_self/_blank',
  `position` VARCHAR(50) DEFAULT 'home' COMMENT '位置：home=首页，product=产品页等',
  `sort` INT DEFAULT 0 COMMENT '排序',
  `status` INT DEFAULT 1 COMMENT '状态：0=禁用，1=启用',
  `start_time` DATETIME DEFAULT NULL COMMENT '开始时间',
  `end_time` DATETIME DEFAULT NULL COMMENT '结束时间',
  `create_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `update_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  `create_by` VARCHAR(50) DEFAULT NULL COMMENT '创建人',
  `is_deleted` TINYINT(1) DEFAULT 0 COMMENT '软删除标记：0=未删除，1=已删除',
  PRIMARY KEY (`id`),
  KEY `idx_position` (`position`),
  KEY `idx_sort` (`sort`),
  KEY `idx_status` (`status`),
  KEY `idx_time_range` (`start_time`, `end_time`),
  KEY `idx_is_deleted` (`is_deleted`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='Banner表';

-- ----------------------------
-- 关于我们表 (site_about)
-- ----------------------------
DROP TABLE IF EXISTS `site_about`;
CREATE TABLE `site_about` (
  `id` CHAR(36) NOT NULL COMMENT '主键ID',
  `title` VARCHAR(200) NOT NULL COMMENT '标题',
  `title_en` VARCHAR(200) DEFAULT NULL COMMENT '标题（英文）',
  `subtitle` VARCHAR(500) DEFAULT NULL COMMENT '副标题',
  `subtitle_en` VARCHAR(500) DEFAULT NULL COMMENT '副标题（英文）',
  `content` LONGTEXT COMMENT '内容（富文本）',
  `content_en` LONGTEXT COMMENT '内容（英文，富文本）',
  `cover_image` VARCHAR(500) DEFAULT NULL COMMENT '封面图片URL',
  `keywords` VARCHAR(200) DEFAULT NULL COMMENT 'SEO关键词',
  `description` VARCHAR(500) DEFAULT NULL COMMENT 'SEO描述',
  `status` INT DEFAULT 1 COMMENT '状态：0=禁用，1=启用',
  `create_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `update_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  `create_by` VARCHAR(50) DEFAULT NULL COMMENT '创建人',
  `is_deleted` TINYINT(1) DEFAULT 0 COMMENT '软删除标记：0=未删除，1=已删除',
  PRIMARY KEY (`id`),
  KEY `idx_status` (`status`),
  KEY `idx_is_deleted` (`is_deleted`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='关于我们表';

-- ----------------------------
-- 下载管理表 (site_download)
-- ----------------------------
DROP TABLE IF EXISTS `site_download`;
CREATE TABLE `site_download` (
  `id` CHAR(36) NOT NULL COMMENT '主键ID',
  `title` VARCHAR(200) NOT NULL COMMENT '下载标题',
  `title_en` VARCHAR(200) DEFAULT NULL COMMENT '下载标题（英文）',
  `description` VARCHAR(500) DEFAULT NULL COMMENT '描述',
  `description_en` VARCHAR(500) DEFAULT NULL COMMENT '描述（英文）',
  `file_url` VARCHAR(500) NOT NULL COMMENT '文件URL',
  `file_name` VARCHAR(200) DEFAULT NULL COMMENT '文件名称',
  `file_size` BIGINT DEFAULT 0 COMMENT '文件大小（字节）',
  `file_type` VARCHAR(50) DEFAULT NULL COMMENT '文件类型（扩展名）',
  `download_count` INT DEFAULT 0 COMMENT '下载次数',
  `category` VARCHAR(50) DEFAULT NULL COMMENT '分类',
  `sort` INT DEFAULT 0 COMMENT '排序',
  `status` INT DEFAULT 1 COMMENT '状态：0=禁用，1=启用',
  `create_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `update_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  `create_by` VARCHAR(50) DEFAULT NULL COMMENT '创建人',
  `is_deleted` TINYINT(1) DEFAULT 0 COMMENT '软删除标记：0=未删除，1=已删除',
  PRIMARY KEY (`id`),
  KEY `idx_category` (`category`),
  KEY `idx_status` (`status`),
  KEY `idx_sort` (`sort`),
  KEY `idx_is_deleted` (`is_deleted`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='下载管理表';

-- ----------------------------
-- 视频管理表 (site_video)
-- ----------------------------
DROP TABLE IF EXISTS `site_video`;
CREATE TABLE `site_video` (
  `id` CHAR(36) NOT NULL COMMENT '主键ID',
  `title` VARCHAR(200) NOT NULL COMMENT '视频标题',
  `title_en` VARCHAR(200) DEFAULT NULL COMMENT '视频标题（英文）',
  `description` VARCHAR(500) DEFAULT NULL COMMENT '描述',
  `description_en` VARCHAR(500) DEFAULT NULL COMMENT '描述（英文）',
  `cover_image` VARCHAR(500) DEFAULT NULL COMMENT '封面图片URL',
  `video_url` VARCHAR(500) NOT NULL COMMENT '视频URL',
  `video_type` VARCHAR(20) DEFAULT 'mp4' COMMENT '视频类型：mp4/webm/external',
  `duration` INT DEFAULT 0 COMMENT '视频时长（秒）',
  `play_count` INT DEFAULT 0 COMMENT '播放次数',
  `category` VARCHAR(50) DEFAULT NULL COMMENT '分类',
  `sort` INT DEFAULT 0 COMMENT '排序',
  `status` INT DEFAULT 1 COMMENT '状态：0=禁用，1=启用',
  `create_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `update_time` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  `create_by` VARCHAR(50) DEFAULT NULL COMMENT '创建人',
  `is_deleted` TINYINT(1) DEFAULT 0 COMMENT '软删除标记：0=未删除，1=已删除',
  PRIMARY KEY (`id`),
  KEY `idx_category` (`category`),
  KEY `idx_status` (`status`),
  KEY `idx_sort` (`sort`),
  KEY `idx_is_deleted` (`is_deleted`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COMMENT='视频管理表';